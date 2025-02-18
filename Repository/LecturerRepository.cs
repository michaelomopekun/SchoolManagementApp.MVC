
// using Microsoft.EntityFrameworkCore;

// public class LecturerRepository : ILecturerRepository
// {
//     private readonly SchoolManagementAppDbContext _context;
//     public async Task AddAsync(Lecturer lecturer)
//     {
//          if(lecturer == null)
//         {
//             Console.WriteLine("❌student is null");
//             throw new ArgumentNullException(nameof(lecturer));
//         }

//         Console.WriteLine($"✅ Adding Lecturer: {lecturer.Username}");
//         _context.Lecturer.Add(lecturer);
//         await _context.SaveChangesAsync();
//         Console.WriteLine($"✅ Lecturer saved to the database: {lecturer.Username}");
//     }

//     public async Task<bool> ExistsAsync(string username)
//     {
//         return await _context.Lecturer.AnyAsync(u=> u.Username == username);

//     }


//     public async Task<Lecturer> GetLecturerById(int id)
//     {
//         try
//         {
//             Console.WriteLine("🔍 Checking database for user with Id : " + id); // Debug log

//             // ✅ Ensure query is executed on the database directly
//             var userQuery = _context.Lecturer.AsQueryable();
//             var user = await userQuery.FirstOrDefaultAsync(u => u.Id == id);

//             Console.WriteLine(user != null 
//                 ? $"✅ Found user: {user.Id}" 
//                 : "❌ No user found in database.");

//             return user;
//         }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"❌ Database error: {ex.Message}");
//         return null;
//     }
//     }

//     public async Task<Lecturer> GetLecturerByUsernameAsync(string username)
//     {
//         if (string.IsNullOrEmpty(username))
//         {
//             Console.WriteLine("❌ Username is null or empty");
//             return null;
//         }

//         try
//         {
//             Console.WriteLine($"🔍 Searching for lecturer with username: {username}"); // Debug log

//             if (_context.Lecturer == null)
//             {
//                 Console.WriteLine("❌ Lecturer DbSet is null");
//                 return null;
//             }

//             var Lecturer = await _context.Lecturer
//                 .Where(u => u.Username == username)
//                 .FirstOrDefaultAsync();

//             if(Lecturer == null)
//             {
//                 Console.WriteLine($"❌ No lecturer found with username: {username}");
//                 return null;
//             }
//             // ✅ Ensure query is executed on the database directly
//             // var userQuery = _context.Lecturer.AsQueryable();

//             Console.WriteLine($"✅ found lecturer: {Lecturer.Username}"); // Debug log
//             return Lecturer;

//         }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"❌❌ Database error while searching for lecturer: {ex.Message}");
//         if(ex.InnerException != null)
//         {
//             Console.WriteLine($"❌❌❌ Inner Exception: {ex.InnerException.Message}");
//         }
//         throw;
//     }
//     }
// }
