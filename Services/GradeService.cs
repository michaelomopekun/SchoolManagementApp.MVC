
using Microsoft.EntityFrameworkCore;

public class GradeService : IGradeService
{
    private readonly SchoolManagementAppDbContext _context;

    public GradeService(SchoolManagementAppDbContext context)
    {
        _context = context;
    }
    public async Task AddGradeAsync(Grade grade)
    {
        if(grade == null)
        {
            throw new ArgumentNullException(nameof(grade));
        }
        await _context.Grades.AddAsync(grade);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteGradeAsync(int gradeId)
    {
        var grade = await _context.Grades.FindAsync(gradeId);
        if(grade == null)
        {
            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Grade>> GetCourseGradesAsync(int courseId)
    {
        return await _context.Grades
            .Include(g=> g.Course)
            .Include(g=> g.User)
            .Where(g=> g.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<Grade?> GetGradeByIdAsync(int gradeId)
    {
        return await _context.Grades
            .Include(g=> g.Course)
            .Include(g => g.User)
            .FirstOrDefaultAsync(g => g.GradeId == gradeId);
        
    }

    public async Task<IEnumerable<Grade>> GetUserGradesAsync(int userId)
    {
        return await _context.Grades
            .Include(g=>g.Course)
            .Include(g=>g.User)
            .Where(g=> g.userId ==userId)
            .ToListAsync();
    }

    public async Task UpdateGradeAsync(Grade grade)
    {
        if(grade == null)
        {
            throw new ArgumentNullException(nameof(grade));
        }
        _context.Entry(grade).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsUserEnrolledInCourse(int userId, int courseId)
        {
            return await _context.Users
                .Include(u => u.EnrolledCourses)
                .AnyAsync(u => u.Id == userId && u.EnrolledCourses.Any(c => c.Id == courseId));
        }
}