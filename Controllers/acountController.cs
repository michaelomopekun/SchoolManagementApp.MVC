using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SchoolManagementApp.MVC.Models;


namespace SchoolManagementApp.MVC.Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    
    public class AccountController : Controller
    {
        private readonly SchoolManagementAppDbContext _context;

        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;
        // private readonly IStudentRepository _studentRepository;

        public AccountController(SchoolManagementAppDbContext context, IAuthService authService, JwtService jwtService)
        {
            _context = context;
            _authService = authService;
            _jwtService = jwtService;
            // _studentRepository = studentRepository;
        }
    
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        // public IActionResult Login(string returnUrl = null)
        // {
        //     ViewData["ReturnUrl"] = returnUrl;
        //     return View();
        // }

        [HttpPost]
        public async Task<IActionResult> Login(LoginView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

        var token = await _authService.Login(model.Username, model.Password);

        if(token==null)
        {
            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }
        //store token in session
        HttpContext.Session.SetString("JWTToken", token);

        return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usernameExists = await _authService.Register(model);

            if(usernameExists==null)
            {
                ModelState.AddModelError("", "User already exists");
                return RedirectToAction("Login");
            }
                var token = _jwtService.GenerateToken(usernameExists.Id.ToString(), usernameExists.Username);

                HttpContext.Session.SetString("JWTToken", token);

                return RedirectToAction("Index", "Home");
        }

            [Authorize]
            [HttpPost]
            [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            HttpContext.Session.Clear();

            // Response.Headers.Add("Cache-Control", "no-cache", "no-store", "must-revalidate");
            Response.Headers.Add("Pragma", "no-cache");
            
            

            if(Request.Headers["Accept"].Contains("application/json"))
            {
                return Ok(new {message = "Logged out successfully"});
            }

            return RedirectToAction("Login","Account",new {returnUrl = "/"});  
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var username = User.Identity?.Name;
            return Ok(new { message = $"Hello, {username}!" });
        }

        [HttpPost("api/account/login")]
        public async Task<IActionResult> ApiLogin([FromBody] LoginView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _authService.Login(model.Username, model.Password);

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(new { Token = token, Message = "Login successful" });
        }

        [HttpPost("api/account/register")]
        public async Task<IActionResult> ApiRegister([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usernameExists = await _authService.Register(model);
            if (usernameExists == null)
            {
                var alreadyExistingUserToken = _jwtService.GenerateToken(usernameExists.Id.ToString(), usernameExists.Username);
                return Ok(new { Token = alreadyExistingUserToken, Message = "User already exists" });
            }

            var token = _jwtService.GenerateToken(usernameExists.Id.ToString(), usernameExists.Username);

            return Ok(new { Token = token, Message = "Registration successful" });
        }

    }
}