using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Authorization;
using SchoolManagementApp.MVC.Models;

[Authorize(Policy = PolicyNames.RequireStudent)]
public class StudentController : Controller
{

    private readonly IGradeService _gradeService;
    private readonly IUserService _userService;
    private readonly ICourseService _courseService;
    private readonly ICourseMaterialService _materialService;

    public StudentController(IGradeService gradeService, IUserService userService, ICourseService courseService, ICourseMaterialService materialService)
    {
        _gradeService = gradeService;
        _userService = userService;
        _courseService = courseService;
        _materialService = materialService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Lecturer,Student")]
    public async Task<IActionResult> Dashboard()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        if (userId == null)
        {
            TempData["Error"] = "User ID Clam not found. ";
            return RedirectToAction("Index", "Home");
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "User not found. ";
            return RedirectToAction("Index", "Home");
        }
        var enrolledCourses = await _courseService.GetStudentEnrolledInCoursewithMaterialAsync(userId);
        var grades = await _gradeService.GetUserGradesAsync(userId);
        var materials = await _materialService.GetMaterialsByStudentAsync(userId);

        var viewModel = new DashboardViewModel
        {
            StudentName = user.Username,
            StudentId = user.Id,
            Program = "BSc",
            EnrolledCourses = enrolledCourses,
            TotalMaterials = materials.Count(),
            AverageGrade = (double?)grades.Average(g => g.Score),
            RecentActivities = await GetRecentActivities(userId)
        };

        return View("~/Views/Student/Dashboard.cshtml", viewModel);
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyGrades()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            TempData["Error"] = "User ID claim not found.";
            return RedirectToAction("Index", "Home");
        }

        var userId = int.Parse(userIdClaim.Value);
        var grades = await _gradeService.GetUserGradesAsync(userId);
        if(grades == null)
        {
            TempData["Error"] = "No Grades available.";
            return View(new List<Grade>());
        }
        return View("~/Views/Student/MyGrades.cshtml",grades);
    }

    [HttpGet]
    [Authorize(Roles = "Student")]

    public IActionResult EnrollCourse()
    {
        return View();
    }


    private async Task<IEnumerable<StudentActivity>> GetRecentActivities(int userId)
    {
        try{
        var activities = new List<StudentActivity>();
        
        var downloads = await _materialService.GetStudentsDownloadHistoryAsync(userId);
        activities.AddRange(downloads.Select(d => new StudentActivity
        {
            Date = d.DownloadDate,
            Description = $"Downloaded {d.CourseMaterial.Title}",
            ActivityType = "Download",
            RelatedItemId = d.CourseMaterialId.ToString()

        }));

        var grades = await _gradeService.GetUserGradesAsync(userId);
        activities.AddRange(grades.Select(g => new StudentActivity
        {
            Date = g.GradedDate,
            Description = $"Graded {g.CourseName}, you can check your grade.",
            ActivityType = "Grade",
            RelatedItemId = g.CourseId.ToString()
        }));

        var enrollments = await _courseService.GetStudentEnrolledInCourseAsync(userId);
        activities.AddRange(enrollments.Select(e => new StudentActivity
        {
            Date = e.EnrollmentDate,
            Description = $"Enrolled in {e.Course.Name}",
            ActivityType = "Enrollment",
            RelatedItemId = e.CourseId.ToString()
        }));

        return activities.OrderByDescending(a => a.Date).Take(3);
        
        }catch(Exception ex)
        {
            TempData["Error"] = $"Error getting recent activities: {ex.Message}";
            return new List<StudentActivity>();
        }
    }

}