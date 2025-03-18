using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Authorization;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Services;

namespace SchoolManagementApp.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        private readonly ICourseMaterialService _materialService;
        private readonly IGradeService _gradeService;
        private readonly IAcademicSettingService _academicSettingService;


        public AdminController(IUserService userService, ICourseService courseService, ICourseMaterialService materialService, IGradeService gradeService, IAcademicSettingService academicSettingService)
        {
            _userService = userService;
            _courseService = courseService;
            _materialService = materialService;
            _gradeService = gradeService;
            _academicSettingService = academicSettingService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId == null)
            {
                TempData["Error"] = "User ID Clam not found. ";
                return RedirectToAction("Index", "Home");
            }

            var totalStudents = await _userService.GetTotalStudentsCountAsync();
            var totalLecturers = await _userService.GetTotalLecturersCountAsync();
            var totalCourses = await _courseService.GetTotalCoursesCountAsync();
            var recentActivities = await GetRecentActivities(userId);

            var viewModel = new DashboardViewModel
            {
                TotalStudents = totalStudents,
                TotalLecturers = totalLecturers,
                TotalCourses = totalCourses,
                RecentActivities = recentActivities,
                QuickActions = new List<QuickAction>
                {
                    new QuickAction { Title = "Manage Users", Action = "ManageUsers", Controller = "Admin", Icon = "fas fa-users" },
                    new QuickAction { Title = "Manage Courses", Action = "CourseList", Controller = "Course", Icon = "fas fa-book" },
                    new QuickAction { Title = "Settings", Action = "AcademicSettings", Controller = "Admin", Icon = "fas fa-cog" }
                }
            };

            return View("~/Views/Admin/Dashboard.cshtml", viewModel);
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Username = model.Username;
                user.Role = model.Role;

                await _userService.UpdateUserAsync(user);
                TempData["Success"] = "User updated successfully";
                return RedirectToAction(nameof(ManageUsers));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(ManageUsers));
        }

        public async Task<IActionResult> ManageStudents()
        {
            var students = await _userService.GetAllStudentsAsync();
            return View(students);
        }

        public async Task<IActionResult> ManageLecturers()
        {
            var lecturers = await _userService.GetAllLecturerAsync();
            return View(lecturers);
        }

        public async Task<IActionResult> ManageAdmin()
        {
            var lecturers = await _userService.GetAllAdminsAsync();
            return View(lecturers);
        }

        public async Task<IActionResult> AssignCourse(int LecturerId)
        {
            var courses = await _courseService.GetAllCoursesAsync();
            ViewBag.LecturerId = LecturerId;
            return View(courses);
        }

        public async Task<IActionResult> AssignCourse(int LecturerId, List<int> CourseId)
        {
            await _courseService.AssigeCourseToLecturerAsync(LecturerId, CourseId);
            TempData["Success"] = "Course assigned successfully";
            return RedirectToAction(nameof(ManageLecturers));
        }


        private async Task<IEnumerable<StudentActivity>> GetRecentActivities(int userId)
        {
            try
            {
                var activities = new List<StudentActivity>();

                // Get material downloads
                var downloads = await _materialService.GetStudentsDownloadHistoryAsync(userId);
                if (downloads != null)
                {
                    activities.AddRange(downloads.Select(d => new StudentActivity
                    {
                        Date = d.DownloadDate,
                        Description = $"Downloaded {d.CourseMaterial?.Title ?? "material"}",
                        ActivityType = "Download",
                        RelatedItemId = d.CourseMaterialId.ToString(),
                        IconClass = "fas fa-download text-primary"
                    }));
                }

                // Get recent grades
                var grades = await _gradeService.GetUserGradesAsync(userId);
                if (grades != null)
                {
                    activities.AddRange(grades
                        .Where(g => g.GradedDate >= DateTime.Now.AddDays(-30)) // Only last 30 days
                        .Select(g => new StudentActivity
                        {
                            Date = g.GradedDate,
                            Description = $"New grade posted for {g.CourseName}",
                            ActivityType = "Grade",
                            RelatedItemId = g.CourseId.ToString(),
                            IconClass = "fas fa-star text-warning"
                        }));
                }

                // Get course enrollments
                var enrollments = await _courseService.GetStudentEnrolledInCourseAsync(userId);
                if (enrollments != null)
                {
                    activities.AddRange(enrollments
                        .Where(e => e.EnrollmentDate >= DateTime.Now.AddDays(-30)) // Only last 30 days
                        .Select(e => new StudentActivity
                        {
                            Date = e.EnrollmentDate,
                            Description = $"Enrolled in {e.Course?.Name ?? "course"}",
                            ActivityType = "Enrollment",
                            RelatedItemId = e.CourseId.ToString(),
                            IconClass = "fas fa-user-plus text-success"
                        }));
                }

                // Order by date and take most recent 5
                return activities
                    .OrderByDescending(a => a.Date)
                    .Take(3)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in GetRecentActivities: {ex.Message}");
                TempData["Error"] = "Unable to load recent activities";
                return Enumerable.Empty<StudentActivity>();
            }
        }


        public async Task<IActionResult> AcademicSettings()
        {
            var settings = await _academicSettingService.GetCurrentSettingsAsync();
            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAcademicSettings(AcademicSetting model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _academicSettingService.UpdateSettingsAsync(model);
                    TempData["Success"] = "Academic settings updated successfully.";
                    return RedirectToAction(nameof(AcademicSettings));
                }
                return View("~/Views/Admin/AcademicSettings.cshtml", model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"error while updating academic settings.{ex.Message}";
                return View("~/Views/Admin/AcademicSettings.cshtml", model);
            }
        }
    }
}