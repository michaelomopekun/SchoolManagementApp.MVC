using Microsoft.EntityFrameworkCore;

public class StudentRepository:IStudentRepository{
    private readonly SchoolManagementAppDbContext _context;
    public StudentRepository(SchoolManagementAppDbContext context)
    {
        _context = context;
    }
    public async Task<Student> GetStudentByUsernameAsync(string username){

    try
        {
            Console.WriteLine("🔍 Checking database for user: " + username); // Debug log

            // ✅ Ensure query is executed on the database directly
            var userQuery = _context.Student.AsQueryable();
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
        
        // return await _context.Student.FirstOrDefaultAsync(u=> u.Username == username);
    }


    public async Task AddAsync(Student student){
        if(student == null)
        {
            Console.WriteLine("❌student is null");
            throw new ArgumentNullException(nameof(student));
        }

        Console.WriteLine($"✅ Adding student: {student.Username}");
        _context.Student.Add(student);
        await _context.SaveChangesAsync();
        Console.WriteLine($"✅ Student saved to the database: {student.Username}");
    }
    public async Task<bool> ExistsAsync(string username){
        return await _context.Student.AnyAsync(u=> u.Username == username);
    }
}