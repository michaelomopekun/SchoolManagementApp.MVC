using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Authorization;
using SchoolManagementApp.MVC.Models;


namespace SchoolManagementApp.MVC.Controllers
{

    [Authorize(Policy = PolicyNames.RequireLecturer)]
    public class LecturerController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;
        private readonly IGradeService _gradeService;

        public LecturerController(ICourseService courseService, IUserService userService, IGradeService gradeService)
        {
            _courseService = courseService;
            _userService = userService;
            _gradeService = gradeService;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Lecturer")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public IActionResult ManageGrades()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> MyCourses() 
        {   
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                TempData["Error"] = "User ID claim not found.";
                return RedirectToAction("Index", "Home");
            }

            var LecturerId = userIdClaim.Value;
            Console.WriteLine($"⌚⌚⌚ found lecturer{LecturerId}");
            var courses = await _courseService.GetCoursesByLecturerIdAsync(int.Parse(LecturerId));
            
            return View("~/Views/Lecturer/MyCourses.cshtml", courses);
        }

        public async Task<IActionResult> CourseStudents(int courseId)
        {
            var students = await _userService.GetStudentsByCoursesAsync(courseId);
            // var enrollment = await _courseService.GetStudentEnrolledInCourseAsync(courseId);
            var grades = await _gradeService.GetCourseGradesAsync(courseId);
            var enrollments = students.Select(async student => new UserCourse
            {
                UserId = student.Id,
                CourseId = courseId,
                Course = await _courseService.GetCourseAsync(courseId),
                User = student,
                Status = EnrollmentStatus.Active
            }).ToList();
            ViewBag.Grades = grades;
            ViewBag.CourseId = courseId;

            return View("ManageGrades",enrollments);
        }
    }
}