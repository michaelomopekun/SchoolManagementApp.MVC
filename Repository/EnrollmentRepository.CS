using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Repository;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly SchoolManagementAppDbContext _context;
    // private readonly ICourseService _courseService;
    public EnrollmentRepository(SchoolManagementAppDbContext context)
    {
        _context = context;
        // _courseService = courseService;
    }
    public async Task EnrollAsync(UserCourse enrollment)
    {
        var existingEnrollment = await _context.UserCourses
            .FirstOrDefaultAsync(u => u.UserId == enrollment.UserId && 
                                    u.CourseId == enrollment.CourseId);

        if (existingEnrollment != null)
        {
            // If enrollment exists but is inactive, reactivate it
            if (existingEnrollment.Status != EnrollmentStatus.Active)
            {
                existingEnrollment.Status = EnrollmentStatus.Active;
                existingEnrollment.EnrollmentDate = DateTime.Now;
                existingEnrollment.WithdrawalDate = DateTime.MinValue;
                existingEnrollment.LecturerId = enrollment.LecturerId;
                
                await UpdateEnrollmentAsync(existingEnrollment);
                return;
            }
            // If active enrollment exists, throw exception or handle accordingly
            throw new InvalidOperationException("Student is already enrolled in this course.");
        }

        var lecturerId = await _context.Course
            .Where(c => c.Id == enrollment.CourseId)
            .Select(c => c.LecturerId)
            .FirstOrDefaultAsync();

        enrollment.LecturerId = lecturerId;

        await _context.UserCourses.AddAsync(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task<UserCourse?> GetEnrollmentAsync(int userId, int courseId)
    {
        return await _context.UserCourses.FirstOrDefaultAsync(u => u.UserId == userId && u.CourseId == courseId && u.Status == EnrollmentStatus.Active);
    }

    public async Task<IEnumerable<UserCourse>> GetUserEnrollmentsAsync(int userId)
    {
        return await _context.UserCourses.Include(u => u.Course).Where(u => u.UserId  == userId && u.Status == EnrollmentStatus.Active).ToListAsync();
    }

    public async Task<bool> IsEnrolledAsync(int userId, int courseId)
    {
        return await _context.UserCourses.AnyAsync(u => u.UserId == userId && u.CourseId == courseId && u.Status == EnrollmentStatus.Active);
    }

    public async Task UpdateEnrollmentAsync(UserCourse enrollment)
    {
        _context.Entry(enrollment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task WithdrawAsync(int userId, int courseId)
    {
        var enrollment = await GetEnrollmentAsync(userId,courseId);
        if(enrollment != null)
        {
            _context.UserCourses.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}