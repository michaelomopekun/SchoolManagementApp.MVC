using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Repository;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly SchoolManagementAppDbContext _context;
    private readonly IUserService _userService;
    private readonly IEnrollmentRepository _enrollmentRepository;
    public CourseService(ICourseRepository courseRepository,SchoolManagementAppDbContext context, IUserService userService, IEnrollmentRepository enrollmentRepository)
    {
        _courseRepository = courseRepository;
        _context = context;
        _userService = userService;
        _enrollmentRepository = enrollmentRepository;
    }
    public async Task AddCourseAsync(Course course)
    {
        if(course.Name==null)
        {
        throw new ArgumentException("Course Name can not be null");
        
        }
        await _courseRepository.AddAsync(course);
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