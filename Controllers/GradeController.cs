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
        public async Task<IActionResult> MyGrades()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var grades = await _gradeService.GetUserGradesAsync(userId);
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
            var grade = new Grade
            {
                CourseId = courseId,
                userId = userid,
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
                await _gradeService.AddGradeAsync(grade);
                TempData["Success"] = "Grade added successfully";
                return RedirectToAction(nameof(ManageGrades), new { courseId = grade.CourseId });
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