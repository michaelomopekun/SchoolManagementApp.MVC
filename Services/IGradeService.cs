namespace SchoolManagementApp.MVC.Models{
public interface IGradeService
{
    Task<IEnumerable<Grade>> GetUserGradesAsync(int userId);
    
    Task<Grade> GetGradeByIdAsync(int gradeId);

    Task AddGradeAsync(Grade grade);

    Task UpdateGradeAsync(Grade grade);

    Task DeleteGradeAsync(int gradeId);

    Task<IEnumerable<Grade>> GetCourseGradesAsync(int courseId);

    Task<bool> IsUserEnrolledInCourse(int userId, int courseId);

    Task<int> GetTotalGradedStudentsForLecturerAsync(int lecturerId);

    Task<IEnumerable<Grade>> GetRecentGradesForLecturerAsync(int lecturerId, int count);

    Task<List<Grade>> GetFilteredGradesAsync(GradeFilterViewModel filter); 
}
}