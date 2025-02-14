
public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    public async Task AddCourseAsync(Course course)
    {
        if(course.Name==null)
        {
        throw new ArgumentException("Course Name can not be null");
        
        }
        await _courseRepository.AddAsync(course);
    }

    public async Task DeleteCourseAsync(int Id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(Id);
        if (course == null)
        {
            throw new ArgumentException("Course not found");
        }
        await _courseRepository.DeleteAsync(Id);
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        return await _courseRepository.GetAllAsync();
    }

    public async Task<Course> GetCourseAsync(int Id)
    {
        return await _courseRepository.GetCourseByIdAsync(Id);
    }

    public async Task UpdateCourseAsync(Course course)
    {
        await _courseRepository.UpdateAsync(course);
    }
}