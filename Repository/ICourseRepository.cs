public interface ICourseRepository
{
    Task<Course> GetCourseByIdAsync(int Id);
    Task AddAsync(Course course);
    Task DeleteAsync(int Id);
    Task UpdateAsync (Course course);
    Task<List<Course>> GetAllAsync();
    // Task DeleteAsync(Course course);
}