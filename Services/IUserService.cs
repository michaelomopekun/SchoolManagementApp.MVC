public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task<IEnumerable<User>> GetStudentsWithEnrollmentsAsync();
}