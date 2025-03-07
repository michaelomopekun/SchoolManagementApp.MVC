using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Services;

public class GradeReportService : IGradeReportService
{
    private readonly SchoolManagementAppDbContext _context;
    private readonly IGradeService _gradeService;

    public GradeReportService(SchoolManagementAppDbContext context, IGradeService gradeService)
    {
        _context = context;
        _gradeService = gradeService;
    }

    public async Task<byte[]> GenerateGradeReportPdf(int studentId, string academicSession, Semester semester)
    {   
        var grades = await GetStudentGrades(studentId, academicSession, semester);
        // var grades = await _context.Grades.FindAsync(studentId);
        var student = await _context.Users.FindAsync(studentId);

        using var ms = new MemoryStream();
        using var document = new Document(PageSize.A4, 25, 25, 30, 30);
        using var writer = PdfWriter.GetInstance(document, ms);

        document.Open();

        if(student != null)
        {
            document.Add(new Paragraph($"Student Grade Report for {academicSession}.{semester}"));
            document.Add(new Paragraph($"Name: {student.Username}"));
            document.Add(new Paragraph($"Student ID: {student.Id}"));
            document.Add(new Paragraph($"Academic Session: {academicSession}"));
            document.Add(new Paragraph($"Semester: {semester}"));
            document.Add(new Paragraph("\n\n\n"));

        }

        var table = new PdfPTable(6);
        table.AddCell("Course Code");
        table.AddCell("Course Title");
        table.AddCell("Credit Unit");
        table.AddCell("Score");
        table.AddCell("Grade");
        table.AddCell("Comment");

        foreach (var grade in grades)
        {
            table.AddCell(grade.CourseCode);
            table.AddCell(grade.CourseName);
            table.AddCell(grade.CreditHours);
            table.AddCell(grade.Score.ToString() ?? "N/A");
            table.AddCell(grade.LetterGrade);
            table.AddCell(grade.Comments);
        }

        document.Add(table);
        document.Close();

        return ms.ToArray();
    }

 public async Task<List<Grade>> GetStudentGrades(int userId, string academicSession, Semester semester)
{
    try
    {
        // Debug the input parameters
        Console.WriteLine($"🔍 Searching grades for: UserId={userId}, Session={academicSession}, Semester={semester}");

        // Check if grades exist without filters first
        var allGrades = await _context.Grades.ToListAsync();
        Console.WriteLine($"📊 Total grades in database: {allGrades.Count}");

        // Build query step by step to identify where data is getting filtered out
        var query = _context.Grades.AsQueryable();
        
        // Add user filter and check count
        query = query.Where(r => r.UserId == userId);
        var userGrades = await query.ToListAsync();
        Console.WriteLine($"📚 Grades for user {userId}: {userGrades.Count}");

        // Add academic session filter and check count
        query = query.Where(r => r.AcademicSession == academicSession);
        var sessionGrades = await query.ToListAsync();
        Console.WriteLine($"📅 Grades for session {academicSession}: {sessionGrades.Count}");

        // Add semester filter and check count
        query = query.Where(r => r.Semester == semester);
        var semesterGrades = await query.ToListAsync();
        Console.WriteLine($"🎓 Grades for semester {semester}: {semesterGrades.Count}");

        // Add includes
        query = query.Include(r => r.User)
                    .Include(r => r.Course);

        var reports = await query.ToListAsync();
        Console.WriteLine($"✅ Final grades count: {reports.Count}");

        return reports;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error in GetStudentGrades: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw;
    }
}
}