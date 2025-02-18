using SchoolManagementApp.MVC.Models;

public interface IStudentRepository{
    // Task<Student> GetStudentById(int id);
    Task<User> GetUserByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<bool> ExistsAsync(string username);
}