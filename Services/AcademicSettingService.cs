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

        public AcademicSettingService(SchoolManagementAppDbContext context)
        {
            _context = context;
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