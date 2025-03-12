using SchoolManagementApp.MVC.Models;

public interface ICourseMaterialService
{
    Task<CourseMaterial> UploadMaterialAsync(IFormFile file, int courseId, string title, string description, int uploaderId);
    Task<CourseMaterial> GetMaterialAsync(int materialId);
    Task<IEnumerable<CourseMaterial>> GetCourseMaterialsAsync(int courseId);
    Task<IEnumerable<CourseMaterialDownload>> GetDownloadHistoryAsync(int materialId);
    Task RecordDownloadAsync(int materialId, int studentId);
    Task UpdateMaterialAsync(int materialId, string title, string description);
    Task DeleteMaterialAsync(int materialId);
    Task <IEnumerable<CourseMaterial>> GetMaterialsByUploaderAsync(int uploaderId);
    Task <IEnumerable<CourseMaterial>> GetMaterialsByStudentAsync(int studentId);
    Task <IEnumerable<CourseMaterialDownload>> GetStudentsDownloadHistoryAsync(int userId);


}