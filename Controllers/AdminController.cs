using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Authorization;

namespace SchoolManagementApp.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Username = model.Username;
                user.Role = model.Role;

                await _userService.UpdateUserAsync(user);
                TempData["Success"] = "User updated successfully";
                return RedirectToAction(nameof(ManageUsers));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpGet]
        public IActionResult ManageCourses()
        {
            return View();
        }
    }
}