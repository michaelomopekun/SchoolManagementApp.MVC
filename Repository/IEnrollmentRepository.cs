using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Repository
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<UserCourse>> GetUserEnrollmentsAsync(int userId);
        Task<bool> IsEnrolledAsync(int userId, int courseId);
        Task EnrollAsync(UserCourse enrollment);
        Task WithdrawAsync(int userId, int courseId);
        Task<UserCourse?> GetEnrollmentAsync(int userId, int courseId);
    }
}