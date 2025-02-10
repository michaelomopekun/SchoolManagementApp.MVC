
using SchoolManagementApp.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


public class AuthenticationService : IAuthService
{
     private readonly SchoolManagementAppDbContext _context;
     private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentRepository _studentRepository;

     public AuthenticationService(IStudentRepository studentRepository,IHttpContextAccessor httpContextAccessor)
     {
        //  _context = context;
        _studentRepository = studentRepository;
        _httpContextAccessor = httpContextAccessor;

     }
    public async Task<Student> Login(string username, string password)
    {

            var user = await _studentRepository.GetStudentByUsernameAsync(username);
            // System.Console.WriteLine($"username: {user.Username} password: {user.Password}");


        if (user == null)
        {
            Console.WriteLine("❌ Authentication failed: User does not exist.");
            return null; // ✅ Reject login if user is not found
        }

        if (!VerifyPassword(password, user.Password))
        {
            Console.WriteLine("❌ Authentication failed: Incorrect password.");
            return null; // ✅ Reject if password is wrong
        }

        Console.WriteLine($"✅ Authentication successful for user: {user.Username}");
        return user;
    }

    private bool VerifyPassword(string password, string userPassword){
        Console.WriteLine($"password: {password} userPassword: {userPassword}");
        // System.Threading.Thread.Sleep(20000);
        return password == userPassword;
    }
    public async Task Logout()
    {
        if (_httpContextAccessor.HttpContext!= null)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public async Task<Student> Register(RegisterViewModel model)
    {
            // checks if the user already exists
        var userExists = await _studentRepository.ExistsAsync(model.Username);

            // checks if the user already exists
        if(userExists)
            {
            Console.WriteLine($"❌ [DEBUG] User {model.Username} already exists.");
                return null;
            }

            // creates a new user
            var user = new Student
            {
                Username = model.Username,
                Password = model.Password
            };

            try{
                Console.WriteLine($"❌username: {user.Username} password: {user.Password}");
                // adds user then saves the change to the DB
                await _studentRepository.AddAsync(user);
                return user;

            }catch(Exception ex){
                Console.WriteLine($"❌could not save changes to the DataBase:{ex.Message}");
                return null;
            }

            // return user;
    }

}