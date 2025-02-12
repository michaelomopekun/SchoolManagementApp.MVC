
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace SchoolManagementApp.MVC.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        public async Task<IActionResult> CourseList()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View(courses);
        }
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
        [HttpPost]
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
            return View(course);
        }
        public async Task<IActionResult> Edit(int Id)
        {
            var course = await _courseService.GetCourseAsync(Id);
            if(course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        public async Task<IActionResult> Delete(int Id)
        {
            var course = await _courseService.GetCourseAsync(Id);
            if(course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            await _courseService.DeleteCourseAsync(Id);
            return RedirectToAction(nameof(CourseList));
            
        }
    }
}