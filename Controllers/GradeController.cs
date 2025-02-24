using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Services;

namespace SchoolManagementApp.MVC.Controllers
{
    [Authorize]
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;

        public GradeController(IGradeService gradeService, ICourseService courseService,IUserService userService)
        {
            _gradeService = gradeService;
            _courseService = courseService;
            _userService = userService;
        }

        [Authorize(Roles = "Student")]
        [Route("Grade/MyGrade/{userId}")]
        public async Task<IActionResult> MyGrades(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
    
            // Security check: ensure users can only view their own grades
            if (currentUserId != userId)
            {
                TempData["Error"] = "You can only view your own grades.";
                return RedirectToAction("CourseList", "Course");
            }

            // Get user details
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("MyEnrollments", "Course");
            }

            // Get grades with course information
            var grades = await _gradeService.GetUserGradesAsync(userId);
            
            ViewBag.User = user;
            return View(grades);
        }

        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> ManageGrades(int courseId)
        {
            var students = await _userService.GetStudentsWithEnrollmentsAsync();
        ViewBag.Grades = await _gradeService.GetCourseGradesAsync(courseId);
        return View(students);
        }

        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddGrade(int courseId, int userid)
        {
            var isEnrolled = await _gradeService.IsUserEnrolledInCourse(userid, courseId);
            if (!isEnrolled)
            {
                TempData["Error"] = "Student is not enrolled in this course.";
                return RedirectToAction("ManageGrades", new { courseId });
            }
            var grade = new Grade
            {
                CourseId = courseId,
                UserId = userid,
                GradedDate = DateTime.UtcNow
            };
            return View(grade);
        }

        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> AddGrade(Grade grade)
        {
        if (ModelState.IsValid)
        {
        try
        {
            await _gradeService.AddGradeAsync(grade);
            TempData["Success"] = "Grade added successfully.";
            return RedirectToAction("ManageGrades", new { courseId = grade.CourseId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
            }
            return View(grade);
        }

        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> EditGrade(int id)
        {
            var grade = await _gradeService.GetGradeByIdAsync(id);
            if (grade == null)
            {
                return NotFound();
            }
            return View(grade);
        }

        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> EditGrade(Grade grade)
        {
            if (ModelState.IsValid)
            {
                await _gradeService.UpdateGradeAsync(grade);
                TempData["Success"] = "Grade updated successfully";
                return RedirectToAction(nameof(ManageGrades), new { courseId = grade.CourseId });
            }
            return View(grade);
        }

        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await _gradeService.GetGradeByIdAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            await _gradeService.DeleteGradeAsync(id);
            TempData["Success"] = "Grade deleted successfully";
            return RedirectToAction(nameof(ManageGrades), new { courseId = grade.CourseId });
        }
    }
}