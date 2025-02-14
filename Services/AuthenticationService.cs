
using SchoolManagementApp.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


public class AuthenticationService : IAuthService
{
        private readonly JwtService _jwtService;
        private readonly IStudentRepository _studentRepository;

     public AuthenticationService(IStudentRepository studentRepository,JwtService jwtService)
     {
        _studentRepository = studentRepository;
        _jwtService = jwtService;

     }
    public async Task<string> Login(string username, string password)
    {

            var user = await _studentRepository.GetStudentByUsernameAsync(username);

        if (user == null || !VerifyPassword(password, user.Password))
        {
            Console.WriteLine("❌ Authentication failed: User does not exist.");
            return null;
        }

        var token =  _jwtService.GenerateToken(user.Id.ToString(),user.Username);
        return token;
    }

    private bool VerifyPassword(string password, string userPassword){
        Console.WriteLine($"password: {password} userPassword: {userPassword}");
        return password == userPassword;
    }
    
    public async Task<Student> Register(RegisterViewModel model)
    {
            // checks if the user already exists
        // var userExists = await _studentRepository.ExistsAsync(model.Username);
        var user = await _studentRepository.GetStudentByUsernameAsync(model.Username);

        

            // checks if the user already exists
        if(user != null)
            {
            Console.WriteLine($"❌ [DEBUG] User {model.Username} already exists.");
                return user;
            }

            // creates a new user
            var student = new Student
            {
                Username = model.Username,
                Password = model.Password
            };

            try{
                Console.WriteLine($"❌username: {student.Username} password: {student.Password}");
                // adds user then saves the change to the DB
                await _studentRepository.AddAsync(student);
                return student;

            }catch(Exception ex){
                Console.WriteLine($"❌could not save changes to the DataBase:{ex.Message}");
                return null;
            }

            // return user;
    }

 
}