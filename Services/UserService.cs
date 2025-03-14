using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;

public class UserService : IUserService
{
    private readonly SchoolManagementAppDbContext _context;

    public UserService(SchoolManagementAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task UpdateUserAsync(User user)
    {
        try
        {
            var existingUser = await _context.Users
                .Include(u => u.EnrolledCourses)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            var level = Level.LevelNoneStudent;
            if(user.Role == UserRole.Student)
            {
                level = Level.Level100;
            }

            if (existingUser != null)
            {
                // Only update specific fields
                existingUser.Username = user.Username;
                existingUser.Role = user.Role;
                existingUser.Level = level;
                
                // Mark as modified
                _context.Entry(existingUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<User>> GetStudentsWithEnrollmentsAsync()
    {
        var students = await _context.Users
            .Where(u => u.Role == UserRole.Student)
            .Include(u => u.EnrolledCourses)
            .ThenInclude(ec => ec.Course)
            .Where(u => u.EnrolledCourses.Any(ec => ec.Status == EnrollmentStatus.Active))
            .AsNoTracking()
            .ToListAsync();

        // Debug logging
        foreach (var student in students)
        {
            var activeEnrollments = student.EnrolledCourses?
                .Count(e => e.Status == EnrollmentStatus.Active) ?? 0;
            Console.WriteLine($"🔍 Student {student.Username}: Total enrollments = {student.EnrolledCourses?.Count ?? 0}, Active = {activeEnrollments}");
            
            foreach (var enrollment in student.EnrolledCourses ?? Enumerable.Empty<UserCourse>())
            {
                Console.WriteLine($"  📚 Course: {enrollment.Course?.Name}, Status: {enrollment.Status}");
            }
        }

        return students;
    }

    public async Task<IEnumerable<User>> GetAllLecturerAsync()
    {
        return await _context.Users
            .Where(s=> s.Role == UserRole.Lecturer)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllStudentsAsync()
    {
        return await _context.Users 
            .Where(s=> s.Role == UserRole.Student)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAllAdminsAsync()
    {
        return await _context.Users
            .Where(s=> s.Role == UserRole.Admin)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetStudentsByCoursesAsync(int CourseId)
    {
        return await _context.Users
            .Where(c=> c.EnrolledCourses.Any(s=> s.Status == EnrollmentStatus.Active && s.CourseId == CourseId))
            .ToListAsync();
    }

    public Task GetCurrentUserAsync(ClaimsPrincipal user)
    {
        var currentUserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var currentUser = GetUserByIdAsync(currentUserId);
        
        return currentUser;
    }

    public async Task<int> GetTotalStudentsCountAsync()
    {
        var totalUserCount = await _context.Users
            .Where(u => u.Role == UserRole.Student)
            .CountAsync();

        return totalUserCount;
    }

    public async Task<int> GetTotalLecturersCountAsync()
    {
        var totalUserCount = await _context.Users
            .Where(u => u.Role == UserRole.Lecturer)
            .CountAsync();
            
        return totalUserCount;
    }

}