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
            Console.WriteLine($"🔍 Received courseId: {courseId}");

            var students = await _userService.GetStudentsWithEnrollmentsAsync();
            var userEnrollments = await _courseService.GetUserEnrolledCourseAsync();

            // Debug logging
            Console.WriteLine($"🔍 Course ID: {courseId}");
            Console.WriteLine($"🔍 Total students: {students.Count()}");
            Console.WriteLine($"🔍 enrollments: {userEnrollments.Count()}");

            // var studentsInCourse = userEnrollments.Where(e =>
            //     e.Status == EnrollmentStatus.Active).ToList();

            // foreach(var student in userEnrollments)
            // {
            //     Console.WriteLine($"🔍🔍 Student: {student.User.Username}");
            // }

            var grades = await _gradeService.GetCourseGradesAsync(courseId);
            Console.WriteLine($"🔍 Grades for course: {grades.Count()}");

            ViewBag.Grades = grades;
            foreach (var grade in grades)
            {
                Console.WriteLine($"🔍🔍 Grade: {grade.Score}");
            }
            ViewBag.CourseId = courseId;
            Console.WriteLine($"🔍🔍the courseId is {courseId}");
            // ViewBag.CourseName = course.Name;

            return View(userEnrollments);
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

        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GradeList(int courseId)
        {
            var grade = await _gradeService.GetCourseGradesAsync(courseId);
            if (grade == null)
            {
                TempData["Error"] = "Course not graded.";
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