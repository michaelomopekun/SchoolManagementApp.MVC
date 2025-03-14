using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SchoolManagementApp.MVC.Services
{
    public class AuthenticationService : IAuthService
    {
        private readonly JwtService _jwtService;
        private readonly SchoolManagementAppDbContext _context;
        private readonly IStudentRepository _studentRepository;

        public AuthenticationService(
            IStudentRepository studentRepository,
            JwtService jwtService,
            SchoolManagementAppDbContext context)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> Login(string username, string password, UserRole role)
        {
            var user = await _studentRepository.GetUserByUsernameAsync(username);
            
            if (user != null && VerifyPassword(password, user.Password, role.ToString(), user.Role.ToString()))
            {
                var permissions = await GetPermissionsForRole(user.Role.ToString());
                var token = _jwtService.GenerateToken(
                    user.Id, 
                    user.Username, 
                    user.Role.ToString(),
                    permissions);
                    
                return (token);
            }
            
            return (null);
        }

        private bool VerifyPassword(string password, string userPassword, string userRole, string role)
        {
            return (password == userPassword);
        }
        
        public async Task<IUser> Register(RegisterViewModel model, UserRole role)
        {
            var existingUser = await _studentRepository.ExistsAsync(model.Username);
            
            if (existingUser)
            {
                return null;
            }

            var level = Level.LevelNoneStudent;

            if(role == UserRole.Student)
            {
                level = Level.Level100;
            }
            

            var newUser = new User
            {
                Username = model.Username,
                Password = model.Password, // TODO: Add password hashing
                Role = role,
                Level = level
                // CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _studentRepository.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");
                return null;
            }
        }

        public async Task<List<string>> GetPermissionsForRole(string role)
        {
            var permissions = await _context.Role_Permissions
                .Join(
                    _context.Roles,
                    rp => rp.role_id,
                    r => r.role_id,
                    (rp, r) => new { RolePermission = rp, Role = r }
                )
                .Join(
                    _context.Permissions,
                    joined => joined.RolePermission.permission_id,
                    p => p.permission_id,
                    (joined, p) => new { joined.Role, Permission = p }
                )
                .Where(joined => joined.Role.role_name.ToLower() == role.ToLower())
                .Select(joined => joined.Permission.permission_name)
                .ToListAsync();

            return permissions ?? new List<string>();
        }


    }
}


