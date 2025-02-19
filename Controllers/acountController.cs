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
        private readonly IStudentRepository _studentRepository;
        // private readonly ILecturerRepository _lecturerRepository;
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;
        // private readonly IStudentRepository _studentRepository;

        public AccountController(SchoolManagementAppDbContext context, IAuthService authService, JwtService jwtService, IStudentRepository studentRepository)
        {
            _context = context;
            _authService = authService;
            _jwtService = jwtService;
            _studentRepository = studentRepository;
            // _lecturerRepository = lecturerRepository;
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

        var token = await _authService.Login(model.Username, model.Password, model.Role);
        var user = await _studentRepository.GetUserByUsernameAsync(model.Username);

        if(token==null)
        {
            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }
        
        var permissions = await _authService.GetPermissionsForRole(model.Role.ToString());
        var jwtToken = _jwtService.GenerateToken(user.Id, user.Username, model.Role.ToString(), permissions);

        //store token in session
        HttpContext.Session.SetString("JWTToken", token);
        HttpContext.Session.SetString("Role", model.Role.ToString());

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

            Console.WriteLine($"Registering user with role: {model.Role}");
            
            IUser user = await _authService.Register(model, model.Role);

            if (user == null)
            {
                ModelState.AddModelError("", "User already exists");
                return View(model);
            }

            var permissions = await _authService.GetPermissionsForRole(model.Role.ToString());
            var token = _jwtService.GenerateToken(user.Id, user.Username,model.Role.ToString(),permissions);

            HttpContext.Session.SetString("JWTToken", token);
            HttpContext.Session.SetString("Role", model.Role.ToString());

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
            // Response.Headers.Add("Pragma", "no-cache");

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

            var token = await _authService.Login(model.Username, model.Password, model.Role);

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var user = await _studentRepository.GetUserByUsernameAsync(model.Username);
            var permissions = await _authService.GetPermissionsForRole(model.Role.ToString());
            var jwtToken = _jwtService.GenerateToken(user.Id, model.Username, model.Role.ToString(),permissions);

            return Ok(new { Token = jwtToken, Message = "Login successful" });
        }

        [HttpPost("api/account/register")]
        public async Task<IActionResult> ApiRegister([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IUser user = await _authService.Register(model, model.Role);
            if (user == null)
            {
                return Conflict(new { Message = "User already exists" });
            }
            // var usernameExistsToken = await _authService.Register(model, model.Role);

            var permissions = await _authService.GetPermissionsForRole(model.Role.ToString());
            var token = _jwtService.GenerateToken(user.Id, user.Username, model.Role.ToString(),permissions);


            return Ok(new { Token = token, Message = "Registration successful" });
        }

    }
}