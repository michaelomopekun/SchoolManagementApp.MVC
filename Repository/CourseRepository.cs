using Microsoft.EntityFrameworkCore;


public class CourseRepository : ICourseRepository
{
    private readonly SchoolManagementAppDbContext _context;

public CourseRepository(SchoolManagementAppDbContext context){
    _context = context;
}
    public async Task AddAsync(Course course)
    {
        _context.Course.Add(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int Id)
    {
        var course = await GetCourseByCodeAsync(Id);
        if(course != null)
        {
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await _context.Course.ToListAsync();    
    }

    public async Task<Course> GetCourseByCodeAsync(int Id)
    {
        return await _context.Course.FirstOrDefaultAsync(c=>c.Id == Id);
    }

    public async Task UpdateAsync(Course course)
    {
            _context.Course.Update(course);
            await _context.SaveChangesAsync();
    }
}
