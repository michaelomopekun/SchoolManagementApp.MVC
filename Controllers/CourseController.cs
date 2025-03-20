
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Repository;
using SchoolManagementApp.MVC.Services;


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
        private readonly INotificationService _notificationService;
        private readonly IAcademicSettingService _academicSettingService;

        public CourseController(ICourseService courseService, ICourseRepository courseRepository, IStudentRepository studentRepository, IEnrollmentRepository enrollmentRepository, IUserService userService, INotificationService notificationService, IAcademicSettingService academicSettingService)
        {
            _courseService = courseService;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _userService = userService;
            _notificationService = notificationService;
            _academicSettingService = academicSettingService;
        }


        [HttpGet("CourseList")]
        [Authorize(Roles = "Student, Admin")]
        public async Task<IActionResult> CourseList()
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = int.Parse(userClaim?.Value ?? "0");
            var user = await _userService.GetUserByIdAsync(userId);
            var currentSemester = await _academicSettingService.GetCurrentSettingsAsync();

            var currentSemesterCourse = await _courseService.GetCoursesBySemesterAsync(user.Level, currentSemester.CurrentSemester);


            // var courses = await _courseService.GetAllCoursesAsync();
            // var CurrentSemester
            foreach (var course in currentSemesterCourse)
            {
                var lecturerId = course.LecturerId;
                var lecturer = await _userService.GetUserByIdAsync(lecturerId);
                ViewBag.Lecturer = lecturer;
            }

            return View(currentSemesterCourse);
        }


        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Create()
        {
            var Lecturer = await _userService.GetAllLecturerAsync();
            var semester = Enum.GetValues(typeof(Semester)).Cast<Semester>();
            var level = Enum.GetValues(typeof(Level)).Cast<Level>();

            ViewBag.Lecturer = new SelectList(Lecturer, "Id", "Username");
            ViewBag.Semester = new SelectList(semester);
            ViewBag.Level = new SelectList(level);
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
            await _notificationService.SendNotificationAsync($"Assigned Course {course.Code}", course.LecturerId.ToString(), $"You have been assigned to a new course by an admin.");
            TempData["Success"] = "Course added successfully";
            return RedirectToAction(nameof(CourseList));
        }


        [HttpGet("edit/{Id}")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Edit(int Id)
        {
            var Lecturer = await _userService.GetAllLecturerAsync();
            var semester = Enum.GetValues(typeof(Semester)).Cast<Semester>();
            var level = Enum.GetValues(typeof(Level)).Cast<Level>();

            ViewBag.Lecturer = new SelectList(Lecturer, "Id", "Username");
            ViewBag.Semester = new SelectList(semester);
            ViewBag.Level = new SelectList(level);

            var course = await _courseRepository.GetCourseByIdAsync(Id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }


        [HttpPost("edit/{Id}")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> Edit(int Id, Course course)
        {
            if (Id != course.Id)
            {
                return NotFound();
            }

            // var courseToEdit = await _courseService.GetCourseAsync(course.Id);
            // if (courseToEdit != null)
            // {
            //     courseToEdit.Code = course.Code;
            // }


            await _courseService.UpdateCourseAsync(course);
            return RedirectToAction(nameof(CourseList));
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int Id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(Id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
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
            try
            {
                var user = await _studentRepository.GetUserByUsernameAsync(User.Identity.Name);
                Console.WriteLine($"🔍🔍🔍🔍🔍🔍🔍🔍user is: {user}");

                var checkEnrollment = await _enrollmentRepository.IsEnrolledAsync(user.Id, courseId);

                if (checkEnrollment)
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
            }
            catch (Exception ex)
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
            try
            {
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

            }
            catch
            {
                TempData["Error"] = "Failed to withdraw from the course. please try again.";
                return RedirectToAction(nameof(MyEnrollments));
            }
        }
    }
}