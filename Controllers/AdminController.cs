using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Authorization;

[Authorize(Policy = PolicyNames.RequireAdmin)]
public class AdminController : Controller
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult ManageUsers()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult ManageCourses()
    {
        return View();
    }
}