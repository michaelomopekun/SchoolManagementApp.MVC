using SchoolManagementApp.MVC.Models;

public interface IAuthService
{
    public Task<IUser> Register(RegisterViewModel model, UserRole role);
    public Task<string> Login(string username, string password,UserRole role);
}