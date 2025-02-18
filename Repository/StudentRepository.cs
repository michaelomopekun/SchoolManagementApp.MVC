using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;

public class StudentRepository:IStudentRepository
{
    private readonly SchoolManagementAppDbContext _context;
    public StudentRepository(SchoolManagementAppDbContext context)
    {
        _context = context;
    }
    public async Task<User> GetUserByUsernameAsync(string username){

    try
        {
            Console.WriteLine("🔍 Checking database for user: " + username); // Debug log

            // ✅ Ensure query is executed on the database directly
            var userQuery = _context.Users.AsQueryable();
            var user = await userQuery.FirstOrDefaultAsync(u => u.Username == username);

            Console.WriteLine(user != null 
                ? $"✅ Found user: {user.Username}" 
                : "❌ No user found in database.");

            return user;
        }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database error: {ex.Message}");
        return null;
    }
        
    }


    public async Task AddAsync(User user){
        if(user == null)
        {
            Console.WriteLine("❌student is null");
            throw new ArgumentNullException(nameof(user));
        }

        Console.WriteLine($"✅ Adding student: {user.Username}");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        Console.WriteLine($"✅ Student saved to the database: {user.Username}");
    }
    public async Task<bool> ExistsAsync(string username){
        // var user = await _context.Users.AnyAsync(u=> u.Username == username);
        return await _context.Users.AnyAsync(u=> (u.Username == username));
    }
}