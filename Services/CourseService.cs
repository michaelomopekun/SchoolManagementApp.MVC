using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Repository;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly SchoolManagementAppDbContext _context;
    private readonly IUserService _userService;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILogger<CourseService> _logger;

    public CourseService(ICourseRepository courseRepository, SchoolManagementAppDbContext context, IUserService userService, IEnrollmentRepository enrollmentRepository, ILogger<CourseService> logger)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _logger = logger;
        _userService = userService;
        _enrollmentRepository = enrollmentRepository;
        _context = context;
    }
    public async Task AddCourseAsync(Course course)
    {
        if (course.Name == null)
        {
            Console.WriteLine($"❌❌ AddCourseAsync Course name is null");
            throw new ArgumentException("Course Name can not be null");

        }

        Console.WriteLine($"✅✅ AddCourseAsync Got course name {course.Name}");
        await _courseRepository.AddAsync(course);
    }

    public async Task AssigeCourseToLecturerAsync(int lecturerId, List<int> couresIds)
    {
        var lecturer = await _context.Users.FindAsync(lecturerId);
        if (lecturer == null || UserRole.Lecturer != lecturer.Role)
        {
            throw new Exception("Lecturer not found");
        }

        var courses = await _context.Course.Where(c => couresIds.Contains(c.Id)).ToListAsync();
        foreach (var course in courses)
        {
            course.LecturerId = lecturerId;
            // await _courseRepository.UpdateAsync(course);
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCourseAsync(int Id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(Id);
        if (course == null)
        {
            throw new ArgumentException("Course not found");
        }

        if (course.Grades != null && course.Grades.Any())
        {
            throw new ArgumentException("Course has grades, cannot delete courses with existing grades");
        }

        await _courseRepository.DeleteAsync(Id);
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        return await _courseRepository.GetAllAsync();
    }


    public async Task<Course> GetCourseAsync(int courseId)
    {
        Console.WriteLine($"🔍 Fetching course with courseId: {courseId}");
        var course = await _context.Course
            .FirstOrDefaultAsync(c => c.Id == courseId);
        if (course == null)
        {
            Console.WriteLine($"❌❌ Course not found in database with courseId: {courseId}");
        }
        return course;
    }

    public async Task<IEnumerable<Course>> GetCoursesByLecturerIdAsync(int lecturerId)
    {
        var academicSession = await _context.AcademicSettings.FirstOrDefaultAsync();

        return await _context.Course
            .Where(c => c.LecturerId == lecturerId && c.Semester == academicSession.CurrentSemester)
            .ToListAsync();
    }

    public async Task<List<Course>> GetLecturerCoursesAsync(int lecturerId)
    {
        var academicSession = await _context.AcademicSettings.FirstOrDefaultAsync();

        var lecturerCourses = await _context.Course
        .Include(c => c.CourseMaterials)
        .Where(uc => uc.LecturerId == lecturerId && uc.Semester == academicSession.CurrentSemester)
        .ToListAsync();

        return lecturerCourses;
    }

    public async Task<List<UserCourse>> GetStudentEnrolledInCourseAsync(int courseId)
    {
        return await _context.UserCourses
            .Include(uc => uc.User)
            .Include(uc => uc.Course)
            .Where(uc => uc.CourseId == courseId && uc.Status == EnrollmentStatus.Active)
            .ToListAsync();
    }

    public async Task<int> GetTotalStudentsForLecturerAsync(int lecturerId)
    {
        try
        {
            return await _context.UserCourses
                .Include(uc => uc.Course)
                .Where(uc => uc.Course.LecturerId == lecturerId && uc.Status == EnrollmentStatus.Active)
                .Select(uc => uc.UserId)
                .Distinct()
                .CountAsync();
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error {ex} getting total students for lecturer {lecturerId}");
            // throw;
        }
    }

    public async Task<List<UserCourse>> GetUserEnrolledCourseAsync()
    {
        var students = await _userService.GetStudentsWithEnrollmentsAsync();
        var userEnrollments = new List<UserCourse>();

        foreach (var student in students)
        {
            var enrolledCourses = await _enrollmentRepository.GetUserEnrollmentsAsync(student.Id);
            userEnrollments.AddRange(enrolledCourses);
        }
        return userEnrollments;
    }

    public async Task UpdateCourseAsync(Course course)
    {
        await _courseRepository.UpdateAsync(course);
    }

    public async Task<List<Course>> GetStudentEnrolledInCoursewithMaterialAsync(int studentId)
    {
        var enrolledCourses = await _context.UserCourses
            .Include(uc => uc.Course)
            .ThenInclude(c => c.CourseMaterials)
            .Where(uc => uc.UserId == studentId && uc.Status == EnrollmentStatus.Active)
            .Select(uc => uc.Course)
            .ToListAsync();

        return enrolledCourses;
    }

    public Task<int> GetTotalCoursesCountAsync()
    {
        var totalCourses = _context.Course.CountAsync();

        return totalCourses;
    }

    public async Task<List<Course>> GetCoursesBySemesterAsync(Level Level, Semester CurrentSemester)
    {
        var courses = await _context.Course
            .Where(c => c.Level == Level && c.Semester == CurrentSemester)
            .ToListAsync();

        return courses;
    }

    public async Task WithdrawAllCourseAsync()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Starting bulk course withdrawal process");

            var activeEnrollments = await _context.UserCourses
                .Where(s => s.Status == EnrollmentStatus.Active)
                .ToListAsync();

            if (!activeEnrollments.Any())
            {
                _logger.LogInformation("No active enrollments found to withdraw");
                return;
            }

            foreach (var enrollment in activeEnrollments)
            {
                enrollment.Status = EnrollmentStatus.Completed;
                enrollment.EnrollmentDate = DateTime.UtcNow;
            }

            var count = await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Successfully completed {Count} course withdrawals", count);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error during bulk course withdrawal");
            throw new InvalidOperationException("Failed to withdraw courses. Please try again.", ex);
        }
    }
}