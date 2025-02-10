public interface IStudentRepository{
    // Task<Student> GetStudentById(int id);
    Task<Student> GetStudentByUsernameAsync(string username);
    Task AddAsync(Student student);
    Task<bool> ExistsAsync(string username);
}