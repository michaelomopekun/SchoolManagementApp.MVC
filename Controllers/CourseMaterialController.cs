using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Services;

[Authorize]
public class CourseMaterialController : Controller
{
    private readonly ICourseMaterialService _materialService;
    private readonly IUserService _userService;
    private readonly ICourseService _courseService;

    public CourseMaterialController(ICourseMaterialService materialService, IUserService userService, ICourseService courseService)
    {
        _materialService = materialService;
        _userService = userService;
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // var materials = await _materialService.GetCourseMaterialsAsync(courseId);
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        Console.WriteLine("⌚⌚⌚⌚⌚⌚the user id is {currentUserId}");

        var LecturerCourses = await _courseService.GetLecturerCoursesAsync(currentUserId);

        foreach (var course in LecturerCourses)
        {
            Console.WriteLine($"⌚⌚⌚⌚⌚⌚the course id is {course.Id}");
        }
        // ViewBag.CourseId = courseId;
        return View("~/Views/CourseMaterial/Index.cshtml",LecturerCourses);
    }

    [HttpGet]
    [Authorize(Roles = "Lecturer")]
    public IActionResult Upload(int courseId)
    {
        ViewBag.CourseId = courseId;
        return View("~/Views/CourseMaterial/Upload.cshtml");
    }

    [HttpPost]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> Upload(IFormFile file, int courseId, string title, string description)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a file to upload.";
            return RedirectToAction(nameof(Index), new { courseId });
        }

        // var currentUser = await _userService.GetCurrentUserAsync();
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUser = await _userService.GetUserByIdAsync(currentUserId);
        
        try
        {
            Console.WriteLine("⌚⌚⌚⌚Uploading material for course: " + courseId);
            Console.WriteLine("⌚⌚⌚⌚Uploading material for user: " + currentUser.Id);
            Console.WriteLine("⌚⌚⌚⌚Uploading material with title: " + title);
            Console.WriteLine("⌚⌚⌚⌚Uploading material with description: " + description);
            
            await _materialService.UploadMaterialAsync(file, courseId, title, description, currentUser.Id);
            TempData["Success"] = "Course material uploaded successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error uploading material: {ex.Message}";
        }

        return RedirectToAction(nameof(Index), new { courseId });
    }

    [HttpGet]
    public async Task<IActionResult> Download(int id)
    {
        CourseMaterial? material = null;
        try
        {
            material = await _materialService.GetMaterialAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUser = await _userService.GetUserByIdAsync(currentUserId);
            
            await _materialService.RecordDownloadAsync(id, currentUser.Id);

            return File(material.FileContent, material.ContentType, material.Title);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error downloading material: {ex.Message}";
            return RedirectToAction(nameof(Index), new { courseId = material?.CourseId });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> Edit(int id)
    {
        var material = await _materialService.GetMaterialAsync(id);
        if (material == null)
        {
            return NotFound();
        }
        return View(material);
    }

    [HttpPost]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> Edit(int id, string title, string description)
    {
        try
        {
            await _materialService.UpdateMaterialAsync(id, title, description);
            TempData["Success"] = "Course material updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error updating material: {ex.Message}";
        }

        return RedirectToAction(nameof(Index), new { courseId = id });
    }

    [HttpPost]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _materialService.DeleteMaterialAsync(id);
            TempData["Success"] = "Course material deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting material: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> DownloadHistory(int id)
    {
        var history = await _materialService.GetDownloadHistoryAsync(id);
        return View(history);
    }
}