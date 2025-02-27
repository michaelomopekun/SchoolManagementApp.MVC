using SchoolManagementApp.MVC.Models;

public interface ICourseService
{
    Task<Course> GetCourseAsync(int Id);
    Task AddCourseAsync(Course course);
    Task DeleteCourseAsync(int Id);
    Task <List<Course>> GetAllCoursesAsync();
    Task UpdateCourseAsync(Course course);
    Task <List<UserCourse>> GetUserEnrolledCourseAsync();
}