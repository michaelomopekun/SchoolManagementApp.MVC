using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Services
{
    public class StudentPromotionService : BackgroundService
    {
        private readonly ILogger<StudentPromotionService> _logger;
        private readonly SchoolManagementAppDbContext _context;
        private readonly IGradeService _gradeService;
        private readonly INotificationService _notificationService;

        public StudentPromotionService(
            ILogger<StudentPromotionService> logger,
            SchoolManagementAppDbContext context,
            IGradeService gradeService,
            INotificationService notificationService)
        {
            _logger = logger;
            _context = context;
            _gradeService = gradeService;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessStudentPromotions(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing student promotions");
                }

                // Wait for 24 hours before next check
                // await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task ProcessStudentPromotions(CancellationToken stoppingToken)
        {
            var students = await _context.Users
                .Where(u => u.Role == UserRole.Student)
                .Include(u => u.Grades)
                .ToListAsync(stoppingToken);

            foreach (var student in students)
            {
                try
                {
                    if (student.Grades != null && student.Grades.Any())
                    {
                        var gpa = await _gradeService.GenerateGradePointAverage(student.Id);
                        _logger.LogInformation("Student {StudentId} GPA: {GPA}", student.Id, gpa);

                        if (gpa >= 3)
                        {
                            var oldLevel = student.Level;
                            student.Level += 100;
                            _logger.LogInformation("Student {StudentId} promoted from level {OldLevel} to {NewLevel}",
                                student.Id, oldLevel, student.Level);

                            await _notificationService.AddNotificationAsync(new Notification
                            {
                                Title = "Academic Promotion",
                                Message = $"Congratulations! You have been promoted from Level {oldLevel} to Level {student.Level}",
                                RecipientIdId = student.Id,
                                GeneratedDate = DateTime.Now,
                                IsRead = false
                            });
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Student {StudentId} has no grades yet", student.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing promotion for student {StudentId}", student.Id);
                }
            }

            await _context.SaveChangesAsync(stoppingToken);
        }

        public async Task TriggerPromotionCheck()
        {
            try
            {
                _logger.LogInformation("Manual promotion check triggered");
                await ProcessStudentPromotions(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual promotion check");
                throw;
            }
        }
    }

}
