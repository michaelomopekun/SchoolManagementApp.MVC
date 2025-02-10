using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementApp.MVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;


namespace SchoolManagementApp.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly SchoolManagementAppDbContext _context;

        private readonly IAuthService _authService;

        public AccountController(SchoolManagementAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
    
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult test()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)

        // var isAuthenticated = await _authService.Login(username, password);

        {   if(!ModelState.IsValid)
            {
                return View();
            }
            if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                //checks the DB for username and password existence
                var user = await _authService.Login(username, password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View();
                }
                
                Console.WriteLine($"✅ User found: {username}");

                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, user.Username),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

                //after verifying users existence, it redirects to Home
                // HttpContext.Session.SetString("User", username);
                return RedirectToAction("Index", "Home");

            }
            return View();
        }

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var usernameExists = await _authService.Register(model);

            if((usernameExists==null)){
                ModelState.AddModelError("", "User already exists");
                return View(model);
            }

                return  RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            // HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> TestDb()
{
    var user = await _authService.Login("ok", "ok");

    if (user == null)
    {
        return Content("❌ No user found in database.");
    }

    return Content($"✅ Found user: {user.Username}");
}

    }
}