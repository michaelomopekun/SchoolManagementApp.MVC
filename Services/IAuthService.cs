using SchoolManagementApp.MVC.Models;

public interface IAuthService
{
    public Task<Student> Register(RegisterViewModel model);
    public Task<string> Login(string username, string password);
}