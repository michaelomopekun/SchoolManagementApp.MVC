using Microsoft.EntityFrameworkCore;

namespace SchoolManagementApp.MVC.Models{
public class GradeService : IGradeService
{
    private readonly SchoolManagementAppDbContext _context;

    public GradeService(SchoolManagementAppDbContext context)
    {
        _context = context;
    }

    public async Task AddGradeAsync(Grade grade)
    {
        try
        {
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
        return await _context.Grades
            .Include(g => g.User)
            .Include(g => g.Course)
            .Where(g => g.CourseId == courseId)
            .AsNoTracking()
            .ToListAsync();
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
}
}