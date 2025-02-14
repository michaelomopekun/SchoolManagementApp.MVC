
        // var isAuthenticated = await _authService.Login(username, password);

        // {   if(!ModelState.IsValid)
        //     {
        //         return View();
        //     }
        //     if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        //     {
        //         //checks the DB for username and password existence
        //         var user = await _authService.Login(username, password);

        //         if (user == null)
        //         {
        //             ModelState.AddModelError("", "Invalid username or password");
        //             return View();
        //         }
                
        //         Console.WriteLine($"✅ User found: {username}");

        //         var claims = new List<Claim>
        //         {
        //         new Claim(ClaimTypes.Name, user.Username),
        //         };

        //         var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //         var authProperties = new AuthenticationProperties { IsPersistent = true };

        //         await HttpContext.SignInAsync(
        //         CookieAuthenticationDefaults.AuthenticationScheme,
        //         new ClaimsPrincipal(claimsIdentity),
        //         authProperties);

        //         //after verifying users existence, it redirects to Home
        //         // HttpContext.Session.SetString("User", username);
        //         return RedirectToAction("Index", "Home");


        //authentication service
                // if (!VerifyPassword(password, user.Password))
        // {
        //     Console.WriteLine("❌ Authentication failed: Incorrect password.");
        //     return null; // ✅ Reject if password is wrong
        // }

        // Console.WriteLine($"✅ Authentication successful for user: {user.Username}");