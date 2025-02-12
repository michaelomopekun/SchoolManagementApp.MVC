using SchoolManagementApp.MVC.Models;

public interface IAuthService
{
    public Task<Student> Register(RegisterViewModel model);
    public Task<Student> Login(string username, string password);
    Task Logout();
}