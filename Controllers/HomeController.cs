using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    public IActionResult Index()
    {   
        try{
        var token = HttpContext.Session.GetString("JWTToken");
        if(token == null)
        {
            return RedirectToAction("Login", "Account");
        }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return RedirectToAction("Login", "Account");
        }
        return View();
    }
    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }
    [Authorize]
    public IActionResult About()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
