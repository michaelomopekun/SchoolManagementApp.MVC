using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Services
{
    public interface IGradeReportService
    {
        Task<byte[]> GenerateGradeReportPdf(int studentId, string academicSession, Semester semester);
        Task<List<Grade>> GetStudentGrades(int studentId, string academicSession, Semester semester);
        // Task<Grade> CreateGradeReport(Grade gradeReport);
    }
}