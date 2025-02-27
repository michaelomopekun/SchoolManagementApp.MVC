
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Repository;


namespace SchoolManagementApp.MVC.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public CourseController(ICourseService courseService, ICourseRepository courseRepository, IStudentRepository studentRepository,IEnrollmentRepository enrollmentRepository)
        {
            _courseService = courseService;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        [HttpGet("CourseList")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> CourseList()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View(courses);
        }

        [Authorize(Roles = "Admin,Lecturer")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Course course)
        {
            if(ModelState.IsValid)
            {
                await _courseService.AddCourseAsync(course);
                return RedirectToAction(nameof(CourseList));
            }
            return View(course);
        }

        [HttpPost("edit/{Id}")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Edit(int Id, Course course)
        {
            if(Id != course.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                await _courseService.UpdateCourseAsync(course);
                return RedirectToAction(nameof(CourseList));
            }
            return  View(course);
        }
        [HttpGet("edit/{Id}")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Edit(int Id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(Id);
            if(course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Delete(int Id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(Id);
            if(course == null)
            {
                return NotFound();
            }
            return  View(course);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            // await 
            await _courseService.DeleteCourseAsync(Id);
            return RedirectToAction(nameof(CourseList));
            
        }

        public async Task<IActionResult> EnrollCourse(int courseId)
        {
        try{
            var user = await _studentRepository.GetUserByUsernameAsync(User.Identity.Name);

            if(await _enrollmentRepository.IsEnrolledAsync(user.Id, courseId))
            {
                TempData["Error"] = "You are already enrolled in this course.";
                return RedirectToAction(nameof(CourseList));
            }

            var enrollment = new UserCourse
            {
                UserId = user.Id,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Active
            };

            await _enrollmentRepository.EnrollAsync(enrollment);
            TempData["Success"] = "You have enrolled in this course. ";
            return RedirectToAction(nameof(MyEnrollments));
        }catch(Exception ex)
        {
            TempData["Error"] = $"Failed to enroll in the course. Please try again :{ex}";
            return RedirectToAction(nameof(CourseList));
        }
        }

        public async Task<IActionResult> MyEnrollments()
        {
            var user = await _studentRepository.GetUserByUsernameAsync(User.Identity.Name);
            var enrollments = await _enrollmentRepository.GetUserEnrollmentsAsync(user.Id);

            return View(enrollments);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> WithdrawCourse(int courseId)
        {
            try{
            var user = await _studentRepository.GetUserByUsernameAsync(User.Identity.Name);
            // var usercourse = await _context.UserCourses.FindAsync(user.Id);
            
            var enrolled = await _enrollmentRepository.GetEnrollmentAsync(user.Id, courseId);

            if (enrolled == null)
            {
                TempData["Error"] = "you are not enrolled in this course.";
                return RedirectToAction(nameof(MyEnrollments));
            }

            enrolled.Status = EnrollmentStatus.Withdrawn;
            enrolled.WithdrawalDate = DateTime.UtcNow;
            await _enrollmentRepository.UpdateEnrollmentAsync(enrolled);
            // await _enrollmentRepository.WithdrawAsync(user.Id,courseId);
            
            TempData["Success"] = "Successfully withdrawn from the course.";
            return RedirectToAction(nameof(MyEnrollments));

            }catch{
                TempData["Error"] = "Failed to withdraw from the course. please try again.";
                return RedirectToAction(nameof(MyEnrollments));
            }
        }
    }
}