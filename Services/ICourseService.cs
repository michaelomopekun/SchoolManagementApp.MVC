using SchoolManagementApp.MVC.Models;

public interface ICourseService
{
    Task<Course> GetCourseAsync(int Id);

    Task AddCourseAsync(Course course);
    
    Task DeleteCourseAsync(int Id);
    
    Task UpdateCourseAsync(Course course);
    
    Task <List<UserCourse>> GetUserEnrolledCourseAsync();
    
    Task<List<UserCourse>> GetStudentEnrolledInCourseAsync(int courseId);
    
    Task<IEnumerable<Course>> GetCoursesByLecturerIdAsync(int lecturerId);
    
    Task <List<Course>> GetAllCoursesAsync();
    
    Task AssigeCourseToLecturerAsync(int lecturerId, List<int>couresIds);  
    
    Task<int> GetTotalStudentsForLecturerAsync(int lecturerId);
    
    // Task<IEnumerable<UserCourse>> GetStudentEnrolledInCourseAsync(int courseId);
    
    // Task<IEnumerable<Course>> GetAvailableCoursesAsync();
    // Task<IEnumerable<UserCourse>> GetLecturerEnrolledCoursesByLecturerIdAsync(int lecturerId, int courseId);
}