using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Authorization;

[Authorize(Policy = PolicyNames.RequireLecturer)]
public class LecturerController : Controller
{
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
    public IActionResult ViewCourses()
    {
        return View();
    }
}