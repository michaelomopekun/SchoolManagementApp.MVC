using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementApp.MVC.Models;


namespace SchoolManagementApp.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly SchoolManagementAppDbContext _context;

        public AccountController(SchoolManagementAppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {   if(!ModelState.IsValid)
            {
                return View();
            }
            //checks the DB for username and password existence
            var user = _context.Student.FirstOrDefault(u => u.Username == username && u.Password == password);
            
            // verifying users existence
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Store UserID for easy access
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

        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // checks if user already exist
            var userExists = _context.Student.Any(u => u.Username == model.Username);

            // if already exists, it redirects to login page
            if(userExists){
                ModelState.AddModelError("", "Username already exists");
                System.Threading.Thread.Sleep(2000);
                return RedirectToAction("Login");
            }

            // if user is new, it adds user to the DB
            var user = new Student
            {
                Username = model.Username,
                Password = model.Password
            };

            try{
                // adds user then saves the change to the DB
                _context.Student.Add(user);
                _context.SaveChanges();
            }catch{
                System.Console.WriteLine($"could not save changes to DB.");
            }
            
        // after the user registers, it then redirects the new user to the login page
            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}