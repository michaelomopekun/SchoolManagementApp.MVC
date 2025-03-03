using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Repository;
using SchoolManagementApp.MVC.Services;

namespace SchoolManagementApp.MVC.Controllers
{
    [Authorize]
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        // private readonly IEnrollmentRepository _enrollmentRepository;

        public GradeController(IGradeService gradeService, ICourseService courseService, IUserService userService)
        {
            _gradeService = gradeService;
            _courseService = courseService;
            _userService = userService;
            // _enrollmentRepository = enrollmentRepository;
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
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                Console.WriteLine($"🔍🔍 User ID claim not found.");
                TempData["Error"] = "User ID claim not found.";
                return RedirectToAction("Index", "Home");
            }

            var lecturerId = int.Parse(userIdClaim.Value);

            var courses = await _courseService.GetCoursesByLecturerIdAsync(lecturerId);

            if (courses == null || !courses.Any())
            {
                Console.WriteLine($"🔍🔍🔍🔍 course is null");

                TempData["Error"] = "No courses found for this lecturer.";
                return RedirectToAction("Index", "Home");
            }

            // var students = await _userService.GetStudentsByCoursesAsync(courseId);
            var students = await _courseService.GetStudentEnrolledInCourseAsync(courseId);
            if (students == null)
            {
                students = new List<UserCourse>();
            }
            var grades = await _gradeService.GetCourseGradesAsync(courseId);
            ViewBag.Grades = grades;
            ViewBag.CourseId = courseId;

            return View("~/Views/Lecturer/ManageGrades.cshtml",students);

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

        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await _gradeService.GetGradeByIdAsync(id);
            if (grade == null)
            {
                return NotFound();
            }
            Console.WriteLine($"🔍🔍🔍🔍preparing to delete grade: {id}");
            await _gradeService.DeleteGradeAsync(id);
            TempData["Success"] = "Grade deleted successfully";
            return RedirectToAction(nameof(ManageGrades), new { courseId = grade.CourseId });
        }

        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GradeList(int courseId)
        {
            var grade = await _gradeService.GetCourseGradesAsync(courseId);
            if (grade == null)
            {
                TempData["Error"] = "Course not yet graded.";
                return RedirectToAction("CourseList", "Course");
            }
            var viewModel = new GradeListViewModel
            {
                CourseId = courseId,
                Grades = grade.Select(g => new Grade
                {
                    GradeId = g.GradeId,
                    UserId = g.UserId,
                    Score = g.Score,
                    Comments = g.Comments,
                    GradedDate = g.GradedDate
                }).ToList()
            };
            return View(viewModel);
        }
    }
}