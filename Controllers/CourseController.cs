
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace SchoolManagementApp.MVC.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    // private readonly ICourseService _courseService;
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseService courseService, ICourseRepository courseRepository)
        {
            _courseService = courseService;
            _courseRepository = courseRepository;
        }

        [HttpGet("CourseList")]
        [Authorize(Roles = "Admin,Lecturer,Student")]
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
            await _courseService.DeleteCourseAsync(Id);
            return RedirectToAction(nameof(CourseList));
            
        }
    }
}