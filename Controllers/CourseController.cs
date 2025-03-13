
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
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
        private readonly IUserService _userService;

        public CourseController(ICourseService courseService, ICourseRepository courseRepository, IStudentRepository studentRepository,IEnrollmentRepository enrollmentRepository, IUserService userService)
        {
            _courseService = courseService;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _userService = userService;
        }

        [HttpGet("CourseList")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> CourseList()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            foreach( var course in courses)
            {
                var lecturerId = course.LecturerId;
                var lecturer = await _userService.GetUserByIdAsync(lecturerId);
                ViewBag.Lecturer = lecturer;
            }

            return View(courses);
        }

        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Create()
        {
            var Lecturer = await _userService.GetAllLecturerAsync();
            ViewBag.Lecturer = new SelectList(Lecturer, "Id", "Username");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Course course)
        {
            Console.WriteLine($"✅✅ Got course name {course.Name}");
            Console.WriteLine($"✅✅ model state is {ModelState.IsValid}");
            Console.WriteLine($"✅✅ lecturerId is {course.LecturerId}");

            // if (!ModelState.IsValid)
            // {
            //     var lecturer = await _userService.GetAllLecturerAsync();
            //     ViewBag.Lecturers = new SelectList(lecturer, "Id", "Username");
            //     return View(course);
            // }

                await _courseService.AddCourseAsync(course);
                return RedirectToAction(nameof(CourseList));
        }

        [HttpPost("edit/{Id}")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Edit(int Id, Course course)
        {
            if(Id != course.Id)
            {
                return NotFound();
            }
            await _courseService.UpdateCourseAsync(course);
            return RedirectToAction(nameof(CourseList));
        }
        [HttpGet("edit/{Id}")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Edit(int Id)
        {
            var Lecturer = await _userService.GetAllLecturerAsync();
            ViewBag.Lecturer = new SelectList(Lecturer, "Id", "Username");

            var course = await _courseRepository.GetCourseByIdAsync(Id);
            if(course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")] 
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(Id);
                TempData["Success"] = "Course deleted successfully";
                return RedirectToAction(nameof(CourseList));
            }
            catch (Exception)
            {
                    TempData["Error"] = "This course has grades and cannot be deleted.";
                    return RedirectToAction(nameof(CourseList));
            }
            // catch (Exception ex)
            // {
            //     TempData["Error"] = $"Failed to delete course. Please try again: {ex}";
            //     return RedirectToAction(nameof(CourseList));
            // }
            
        }

        public async Task<IActionResult> EnrollCourse(int courseId)
        {
        try{
            var user = await _studentRepository.GetUserByUsernameAsync(User.Identity.Name);
                Console.WriteLine($"🔍🔍🔍🔍🔍🔍🔍🔍user is: {user}");

            var checkEnrollment =await _enrollmentRepository.IsEnrolledAsync(user.Id, courseId);

            if(checkEnrollment)
            {
                TempData["Error"] = "You are already enrolled in this course.";
                return RedirectToAction(nameof(CourseList));
            }

            Console.WriteLine($"🔍🔍🔍🔍🔍🔍🔍🔍user enrollment NOT ADDED");

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
            Console.WriteLine($"Failed to enroll in the course. Please try again :{ex}");

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