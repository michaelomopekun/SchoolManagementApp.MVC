using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

public async Task<IEnumerable<User>> GetEnrolledCourseLecturersAsync(int studentId)
{
    try
    {
        return await _context.UserCourses
            .Where(e => e.UserId == studentId && e.Status == EnrollmentStatus.Active)
            .Join(_context.Users,
                e => e.Course.LecturerId,
                u => u.Id,
                (e, u) => new
                {
                    Lecturer = u,
                    CourseName = e.Course.Name
                })
            .Distinct()
            .AsNoTracking()
            .Select(x => (x.Lecturer))
            .ToListAsync();
    }
    catch (Exception ex)
    {
        throw new Exception($"Error getting enrolled course lecturers for student {studentId}", ex);
    }
}

public async Task<IEnumerable<UserCourse>> GetLecturersStudentsAsync(int lecturerId)
{
    if(lecturerId <= 0)
    {
        throw new ArgumentException("Invalid Lecturer ID passed .", nameof(lecturerId));
    }

    try
    {
        return await _context.UserCourses
            .Where(e => e.LecturerId == lecturerId && e.Status == EnrollmentStatus.Active)
            .Include(e => e.User)
            // .Include(e => e.Course)
            .Select(e => new UserCourse
            {
                UserId = e.UserId,
                User = new User
                {
                    Username = e.User.Username
                }
            }).ToListAsync();
    }
    catch (Exception ex)
    {
        throw new Exception($"Error getting students for lecturer {lecturerId}", ex);
    }
}

}