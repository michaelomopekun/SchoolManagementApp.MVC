using SchoolManagementApp.MVC.Models;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task<IEnumerable<User>> GetStudentsWithEnrollmentsAsync();
    Task<IEnumerable<User>> GetAllLecturerAsync();
    Task<IEnumerable<User>> GetAllStudentsAsync();
    Task<IEnumerable<User>> GetAllAdminsAsync();
    Task<IEnumerable<User>> GetStudentsByCoursesAsync(int CourseId);
}