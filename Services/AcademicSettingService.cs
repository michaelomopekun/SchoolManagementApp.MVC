using Hangfire;
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
        private readonly StudentPromotionService _studentPromotionService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public AcademicSettingService(SchoolManagementAppDbContext context, StudentPromotionService studentPromotionService, IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _studentPromotionService = studentPromotionService;
            _backgroundJobClient = backgroundJobClient;

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

            // if(existing != null)
            var formattedCurrentSession = int.Parse(existing.CurrentSession.Replace("/", ""));
            var formattedSession = int.Parse(settings.CurrentSession.Replace("/", ""));

            if (existing != null)
            {

                if (formattedSession > formattedCurrentSession && existing.CurrentSemester == Semester.SecondSemester && settings.CurrentSemester == Semester.FirstSemester)
                {
                    if ((formattedSession - formattedCurrentSession) == 10001)
                    {
                        existing.CurrentSession = settings.CurrentSession;
                        existing.CurrentSemester = settings.CurrentSemester;
                        existing.LastUpdated = DateTime.Now;

                        await _context.SaveChangesAsync();
                        // _studentPromotionService.TriggerPromotionCheck();
                        _backgroundJobClient.Enqueue<StudentPromotionService>(
                            service => service.TriggerPromotionCheck());

                        // ("Promotion check triggered due to session change");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot update session to {settings.CurrentSession}, its not visible");
                    }
                }
                else if (formattedSession == formattedCurrentSession && settings.CurrentSemester == Semester.SecondSemester && existing.CurrentSemester == Semester.FirstSemester)
                {
                    existing.CurrentSemester = settings.CurrentSemester;
                    existing.LastUpdated = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException("Cannot update to previous semester/session");
                }


                // if (existing != null && formattedSession > formattedCurrentSession)
                // {
                //     if (existing.CurrentSemester == Semester.FirstSemester)
                //     {

                //     }

                // }

            }
            else
            {
                settings.LastUpdated = DateTime.Now;
                _context.AcademicSettings.Add(settings);

                await _context.SaveChangesAsync();
            }
        }
    }
}
