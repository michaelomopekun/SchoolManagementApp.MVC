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
        if(course.Name==null)
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
        if(lecturer == null || UserRole.Lecturer != lecturer.Role)
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
        return await _context.Course.Where(c => c.LecturerId == lecturerId).ToListAsync();
    }

    public async Task<List<Course>> GetLecturerCoursesAsync(int lecturerId)
    {
        var lecturerCourses = await _context.Course
        .Include(c => c.CourseMaterials)
        .Where(uc => uc.LecturerId == lecturerId)
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
        }catch (Exception ex)
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
}