using SchoolManagementApp.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


public class AuthenticationService : IAuthService
{
        private readonly JwtService _jwtService;
        private readonly IStudentRepository _studentRepository;
        // private readonly ILecturerRepository _lecturerRepository;

     public AuthenticationService(IStudentRepository studentRepository,JwtService jwtService)
     {
        _studentRepository = studentRepository;
        _jwtService = jwtService;
        // _lecturerRepository = lecturerRepository;
     }
    public async Task<string> Login(string username, string password, UserRole role)
    {
        // switch (role){
            // case UserRole.Lecturer:
            // var lecturer = await _lecturerRepository.GetLecturerByUsernameAsync(username);
            // if(lecturer != null && VerifyPassword(password, lecturer.Password))
            // {
            //     return _jwtService.GenerateToken(lecturer.Id,lecturer.Username);
            // }
            // break; 
            // case UserRole.Student:
            var User = await _studentRepository.GetUserByUsernameAsync(username);
            if (User != null && VerifyPassword(password, User.Password,role.ToString(),User.Role.ToString()))
            {
                return _jwtService. GenerateToken(User.Id,User.Username);
            }
            // break;
            return null;
        }
        // return null;
    // }
    

    private bool VerifyPassword(string password, string userPassword, string userRole, string role){
        Console.WriteLine($"password: {password} userPassword: {userPassword}");
        var verified = (password == userPassword) && (role == userRole);
        return verified;
    }
    
    public async Task<IUser> Register(RegisterViewModel model, UserRole role)
    {
        // switch (role)
        // {
            // case UserRole.Student:
                var existingUser = await _studentRepository.ExistsAsync(model.Username);
                // var existingUser2 = await _studentRepository.GetUserByUsernameAsync(model.Username);
                if( existingUser)
                {
                    Console.WriteLine($"❌ [DEBUG] User {model.Username} already exists.");
                    return null;
                }

                var newUser = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    Role = role
                };

                try
                {
                    Console.WriteLine($"✅ Adding {role}: {newUser.Username}");
                    await _studentRepository.AddAsync(newUser);
                    Console.WriteLine($"✅ Student saved to the database: {newUser.Username}");
                    return newUser;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"❌ Could not save changes to the database: {ex.Message}");
                    return null;
                }

            // case UserRole.Lecturer:
            //     var existingLecturer = await _lecturerRepository.GetLecturerByUsernameAsync(model.Username);
            //     if(existingLecturer != null)
            //     {
            //         Console.WriteLine($"❌ [DEBUG] User {model.Username} already exists.");
            //         return null;
            //     }

            //     var newLecturer = new Lecturer
            //     {
            //         Username = model.Username,
            //         Password = model.Password,
            //         Role = UserRole.Lecturer
            //     };

            //     try
            //     {
            //         Console.WriteLine($"✅ Adding Lecturer: {newLecturer.Username}");
            //         await _lecturerRepository.AddAsync(newLecturer);
            //         Console.WriteLine($"✅ Lecturer saved to the database: {newLecturer.Username}");
            //         return newLecturer;
            //     }
            //     catch(Exception ex)
            //     {
            //         Console.WriteLine($"❌ Could not save changes to the database: {ex.Message}");
            //         return null;
            //     }

        }
    }

 
