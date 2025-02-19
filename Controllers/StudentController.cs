using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Authorization;

[Authorize(Policy = PolicyNames.RequireStudent)]
public class StudentController : Controller
{
    [HttpGet]
    [Authorize(Roles = "Admin,Lecturer,Student")]
    public IActionResult Dashboard()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Lecturer,Student")]
    public IActionResult ViewGrades()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Student")]

    public IActionResult EnrollCourse()
    {
        return View();
    }
}