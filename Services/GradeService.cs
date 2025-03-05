using Microsoft.EntityFrameworkCore;

namespace SchoolManagementApp.MVC.Models{
public class GradeService : IGradeService
{
    private readonly SchoolManagementAppDbContext _context;
    private readonly ILogger<GradeService> _logger;

    public GradeService(SchoolManagementAppDbContext context, ILogger<GradeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddGradeAsync(Grade grade)
    {
        try
        {
            Console.WriteLine($"⌚⌚⌚⌚⌚⌚Adding grade of score {grade.Score} an comment {grade.Comments} for user {grade.UserId} in course {grade.CourseId}");
            if (grade == null)
                throw new ArgumentNullException(nameof(grade));

            // Verify student is enrolled
            var isEnrolled = await _context.UserCourses
                .AnyAsync(uc => uc.UserId == grade.UserId && 
                               uc.CourseId == grade.CourseId);

            if (!isEnrolled)
                throw new InvalidOperationException("Student is not enrolled in this course");

            var existingGrade = await _context.Grades
                .FirstOrDefaultAsync(g => g.UserId == grade.UserId && 
                                         g.CourseId == grade.CourseId);

            var existingEnrollment = await _context.UserCourses
                .FirstOrDefaultAsync(e => e.UserId == grade.UserId && e.CourseId == grade.CourseId);

            if (existingGrade != null)
            {
                // Update existing grade
                existingGrade.Score = grade.Score;
                existingGrade.Comments = grade.Comments;
                existingGrade.GradedDate = DateTime.UtcNow;


                _context.Entry(existingGrade).State = EntityState.Modified;

            }
            else
            {
                //update gradeStatus in the usercourse table
                existingEnrollment.gradeStatus = gradeStatus.Graded;
                _context.Entry(existingEnrollment).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Add new grade
                grade.GradedDate = DateTime.UtcNow;
                await _context.Grades.AddAsync(grade);
            }

            var saveResult = await _context.SaveChangesAsync();
            if (saveResult <= 0)
                throw new InvalidOperationException("Failed to save grade to database");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error managing grade: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteGradeAsync(int gradeId)
    {
        var grade = await _context.Grades.FindAsync(gradeId);
        if (grade != null)
        {
            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Grade>> GetCourseGradesAsync(int courseId)
    {
        try
            {
                return await _context.Grades
                    .Include(g => g.Course)
                    .Include(g => g.User)
                    .Where(g => g.CourseId == courseId)
                    .OrderByDescending(g => g.GradedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"🔍🔍🔍🔍🔍🔍🔍🔍 Error retrieving grades for user {UserId}");
                throw;
            }
    }

    public async Task<Grade?> GetGradeByIdAsync(int gradeId)
    {
        return await _context.Grades
            .Include(g => g.User)
            .Include(g => g.Course)
            .FirstOrDefaultAsync(g => g.GradeId == gradeId);
    }

    public async Task<IEnumerable<Grade>> GetUserGradesAsync(int userId)
    {
        return await _context.Grades
            .Include(g => g.Course)
            .Include(g=> g.User)
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.GradedDate)
            .ToListAsync();
    }

    public async Task UpdateGradeAsync(Grade grade)
    {
        try
        {
            var existingGrade = await _context.Grades
                .FirstOrDefaultAsync(g => g.GradeId == grade.GradeId);


            if (existingGrade != null)
            {
                existingGrade.Score = grade.Score;
                existingGrade.Comments = grade.Comments;
                existingGrade.GradedDate = DateTime.UtcNow;

                _context.Entry(existingGrade).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating grade: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> IsUserEnrolledInCourse(int userId, int courseId)
    {
        return await _context.UserCourses
            .AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
    }

    public async Task<int> GetTotalGradedStudentsForLecturerAsync(int lecturerId)
    {
        return await _context.Grades
            .Include(g => g.Course)
            .Where(g => g.Course.LecturerId == lecturerId)
            .Select(g => g.UserId)
            .Distinct()
            .CountAsync();
    }

    public async Task<IEnumerable<Grade>> GetRecentGradesForLecturerAsync(int lecturerId, int count)
    {
        return await _context.Grades
            .Include(g => g.Course)
            .Include(g => g.User)
            .Where(g => g.Course.LecturerId == lecturerId)
            .OrderByDescending(g => g.GradedDate)
            .Take(count)
            .ToListAsync();
}

        public async Task<List<Grade>> GetFilteredGradesAsync(GradeFilterViewModel filter)
        {
            var query = _context.Grades
            .Where(g => g.CourseId == filter.CourseId)
            .AsQueryable();

            if(filter.StudentId.HasValue)
            {
                query = query.Where(g => g.UserId == filter.StudentId);
            }

            if(filter.GradeStatus.HasValue)
            {
                if(filter.GradeStatus == GradeStatus.Graded)
                {
                    query = query.Where(g => g.Score.HasValue);
                }
                else if(filter.GradeStatus == GradeStatus.NotGraded)
                {
                    query = query.Where(g => !g.Score.HasValue);
                }
            }

            if(filter.MaxScore.HasValue)
            {
                query = query.Where(g => g.Score <= filter.MaxScore);
            }

            if(filter.MinScore.HasValue)
            {
                query = query.Where(g => g.Score >= filter.MinScore);
            }

            return await query.Select(g => new Grade
            {
                GradeId = g.GradeId,
                UserId = g.UserId,
                CourseId = g.CourseId,
                Score = g.Score,
                // Comments = g.Comments,
                // GradedDate = g.GradedDate
            }).ToListAsync();
        }
    }
}