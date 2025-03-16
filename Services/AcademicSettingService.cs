using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Services
{

    public interface IAcademicSettingService
    {
        Task<AcademicSetting> GetCurrentSettingsAsync();
        Task UpdateSettingsAsync(AcademicSetting settings);
    }

    public class AcademicSettingService : IAcademicSettingService
    {
        private readonly SchoolManagementAppDbContext _context;
        private readonly IGradeService _gradeService;

        public AcademicSettingService(SchoolManagementAppDbContext context, IGradeService gradeService)
        {
            _context = context;
            _gradeService = gradeService;
        }

        public async Task<AcademicSetting> GetCurrentSettingsAsync()
        {
            var settings = await _context.AcademicSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new AcademicSetting
                {
                    CurrentSession = "2024/2025",
                    CurrentSemester = Semester.FirstSemester,
                    LastUpdated = DateTime.Now
                };
                _context.AcademicSettings.Add(settings);
                await _context.SaveChangesAsync();
            }
            return settings;
        }

        public async Task UpdateSettingsAsync(AcademicSetting settings)
        {
            var existing = await _context.AcademicSettings.FirstOrDefaultAsync();

            if(existing != null && existing.CurrentSession != settings.CurrentSession)
            {
        var students = await _context.Users
            .Where(u => u.Role == UserRole.Student)
            .Include(u => u.Grades)  // Include grades to check if student has any
            .ToListAsync();
        
        foreach (var student in students)
        {
            try
            {
                // Only calculate GPA if student has grades
                if (student.Grades != null && student.Grades.Any())
                {
                    var gpa = await _gradeService.GenerateGradePointAverage(student.Id);
                    Console.WriteLine($"📚📚📚 Student {student.Id} GPA: {gpa:F2}");
                    
                    if(gpa >= 3)
                    {
                        student.Level += 100;
                        Console.WriteLine($"🎓 Student {student.Id} promoted to level {student.Level}");
                    }
                }
                else
                {
                    Console.WriteLine($"ℹ️ Student {student.Id} has no grades yet");
                }
            }
            catch (Exception ex)
            {
                // Log the error but continue processing other students
                Console.WriteLine($"❌ Error processing student {student.Id}: {ex.Message}");
                continue;
            }
        }
            }

            if (existing != null)
            {
                existing.CurrentSession = settings.CurrentSession;
                existing.CurrentSemester = settings.CurrentSemester;
                existing.LastUpdated = DateTime.Now;
            }
            else
            {
                settings.LastUpdated = DateTime.Now;
                _context.AcademicSettings.Add(settings);
            }
            await _context.SaveChangesAsync();
        }
    }
}