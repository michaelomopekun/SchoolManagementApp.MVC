using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Repository;

namespace SchoolManagementApp.MVC.Models
{
    public class GradeService : IGradeService
    {
        private readonly SchoolManagementAppDbContext _context;
        private readonly ILogger<GradeService> _logger;
        private readonly ICourseService _courseService;
        // private readonly IEnrollmentRepository _enrollmentRepository;
        // private readonly IGradeService _gradeService;

        public GradeService(SchoolManagementAppDbContext context, ILogger<GradeService> logger, ICourseService courseService)
        {
            _context = context;
            _logger = logger;
            _courseService = courseService;
            // _gradeService = gradeService;
            // _enrollmentRepository = enrollmentRepository;
        }

        public async Task AddGradeAsync(Grade grade)
        {
            try
            {
                Console.WriteLine($"⌚⌚⌚⌚⌚⌚Adding grade of score {grade.Score} an comment {grade.Comments} for user {grade.UserId} in course {grade.CourseId}");

                var course = await _courseService.GetCourseAsync(grade.CourseId);

                if (course == null)
                {
                    Console.WriteLine("🔍🔍🔍🔍Error generating Course in method AddGradeAsync in course service");
                }

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

                    var academicSession = await _context.AcademicSettings.FirstOrDefaultAsync();

                    grade.AcademicSession = academicSession.CurrentSession;
                    grade.CourseCode = course.Code;
                    grade.CreditHours = course.Credit;
                    grade.CourseName = course.Name;
                    grade.Semester = academicSession.CurrentSemester;
                    grade.GradePoint = GenerateGradePoint(grade.Score, course.Credit);

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
                _logger.LogError(ex, "🔍🔍🔍🔍🔍🔍🔍🔍 Error retrieving grades for user {UserId}");
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
            var user = await _context.Users.FindAsync(userId);
            var semester = await _context.AcademicSettings.FirstOrDefaultAsync();

            if (user == null || semester == null)
                throw new InvalidOperationException("User not found");

            var courses = await _courseService.GetCoursesBySemesterAsync(user.Level, semester.CurrentSemester);
            var currentSemesterCourses = courses.Select(c => c.Id).ToList();

            var grade = await _context.Grades
                .Include(g => g.Course)
                .Include(g => g.User)
                .Where(g => g.UserId == userId && currentSemesterCourses.Contains(g.CourseId))
                .OrderByDescending(g => g.GradedDate)
                .ToListAsync();

            return grade;
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
                    existingGrade.GradePoint = GenerateGradePoint(existingGrade.Score, existingGrade.CreditHours);

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

            if (filter.StudentId.HasValue)
            {
                query = query.Where(g => g.UserId == filter.StudentId);
            }

            if (filter.GradeStatus.HasValue)
            {
                if (filter.GradeStatus == GradeStatus.Graded)
                {
                    query = query.Where(g => g.Score.HasValue);
                }
                else if (filter.GradeStatus == GradeStatus.NotGraded)
                {
                    query = query.Where(g => !g.Score.HasValue);
                }
            }

            if (filter.MaxScore.HasValue)
            {
                query = query.Where(g => g.Score <= filter.MaxScore);
            }

            if (filter.MinScore.HasValue)
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

        public int GenerateGradePoint(decimal? score, string creditHours)
        {
            try
            {
                if (score != null && creditHours != null)
                {
                    if (score >= 80 && creditHours == "1") return 6;
                    if (score >= 60 && score < 80 && creditHours == "1") return 5;
                    if (score >= 50 && score < 60 && creditHours == "1") return 3;
                    if (score >= 45 && score < 50 && creditHours == "1") return 2;
                    if (score >= 40 && score < 45 && creditHours == "1") return 1;
                    if (score < 40 && creditHours == "1") return 0;

                    if (score >= 80 && creditHours == "2") return 10;
                    if (score >= 60 && score < 80 && creditHours == "2") return 8;
                    if (score >= 50 && score < 60 && creditHours == "2") return 6;
                    if (score >= 45 && score < 50 && creditHours == "2") return 4;
                    if (score >= 40 && score < 45 && creditHours == "2") return 2;
                    if (score < 40 && creditHours == "2") return 0;

                    if (score >= 80 && creditHours == "3") return 15;
                    if (score >= 60 && score < 80 && creditHours == "3") return 12;
                    if (score >= 50 && score < 60 && creditHours == "3") return 9;
                    if (score >= 45 && score < 50 && creditHours == "3") return 6;
                    if (score >= 40 && score < 45 && creditHours == "3") return 3;
                    if (score < 40 && creditHours == "3") return 0;
                }

                throw new InvalidOperationException("Score and credit hours cannot be null");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating grade point: {ex.Message}");
                throw;
            }
        }

        public async Task<decimal> GenerateGradePointAverage(int StudentId)
        {
            try
            {
                var grades = await _context.Grades
                    .Where(g => g.UserId == StudentId)
                    .ToListAsync();

                decimal studentsGradePoint = 0;
                decimal totalEarnablePoints = 0;
                decimal achivableGradePointAverage = 5;

                foreach (var grade in grades)
                {
                    studentsGradePoint += grade.GradePoint;
                    int Points = int.Parse(grade.CreditHours);

                    if (Points == 1)
                        totalEarnablePoints += 6;
                    if (Points == 2)
                        totalEarnablePoints += 10;
                    if (Points == 3)
                        totalEarnablePoints += 15;

                }

                if (totalEarnablePoints == 0)
                {
                    _logger.LogWarning("--------------------------------------------------------------------------------------------------------------------------------------------------------");
                    _logger.LogWarning($"GenerateGradePointAverage : Total earnable points is 0 for student {StudentId}. Returning GPA of 0.");
                    _logger.LogWarning("--------------------------------------------------------------------------------------------------------------------------------------------------------");

                    return 0;
                }

                var gpa = (studentsGradePoint / totalEarnablePoints) * achivableGradePointAverage;

                return decimal.Round(gpa, 2, MidpointRounding.AwayFromZero);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error generating grade point average: {ex.Message}");
            }
        }
    }
}