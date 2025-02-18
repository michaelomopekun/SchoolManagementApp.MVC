// public async Task<IActionResult> Register(RegisterViewModel model)
// {
//     if (!ModelState.IsValid)
//     {
//         return View(model);
//     }

//     IUser user = await _authService.Register(model, model.Role);

//     if (user == null)
//     {
//         ModelState.AddModelError("", "User already exists");
//         return View(model);
//     }

//     var token = _jwtService.GenerateToken(user.Id, user.Username);  // No need for ToString()
//     HttpContext.Session.SetString("JWTToken", token);
//     HttpContext.Session.SetString("Role", model.Role.ToString());

//     return RedirectToAction("Index", "Home");
// }