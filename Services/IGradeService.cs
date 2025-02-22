public interface IGradeService
{
    Task<IEnumerable<Grade>> GetUserGradesAsync(int userId);
    Task<Grade> GetGradeByIdAsync(int gradeId);
    Task AddGradeAsync(Grade grade);
    Task UpdateGradeAsync(Grade grade);
    Task DeleteGradeAsync(int gradeId);
    Task<IEnumerable<Grade>> GetCourseGradesAsync(int courseId);
    Task<bool> IsUserEnrolledInCourse(int userId, int courseId);
}