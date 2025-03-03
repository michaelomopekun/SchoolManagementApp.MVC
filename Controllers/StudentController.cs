using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Authorization;
using SchoolManagementApp.MVC.Models;

[Authorize(Policy = PolicyNames.RequireStudent)]
public class StudentController : Controller
{

    private readonly IGradeService _gradeService;

    public StudentController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Lecturer,Student")]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyGrades()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            TempData["Error"] = "User ID claim not found.";
            return RedirectToAction("Index", "Home");
        }

        var userId = int.Parse(userIdClaim.Value);
        var grades = await _gradeService.GetUserGradesAsync(userId);
        if(grades == null)
        {
            TempData["Error"] = "No Grades available.";
            return View(new List<Grade>());
        }
        return View(grades);
    }

    [HttpGet]
    [Authorize(Roles = "Student")]

    public IActionResult EnrollCourse()
    {
        return View();
    }
}