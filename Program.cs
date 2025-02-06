
// Import necessary namespaces for authentication, database access, and ASP.NET Core configuration.
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC;
using System.Data.Entity;


// Create a new instance of the WebApplication builder.
var builder = WebApplication.CreateBuilder(args);

//   Authentication Setup
// Configures cookie-based authentication so that users must log in before accessing protected pages.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Account/Login"; // Redirects unauthorized users to the login page.

    });

//   Database Configuration
// Registers the application's database context and configures it using settings from `appsettings.json`.
builder.Services.AddDbContext<SchoolManagementAppDbContext>(options =>
{
    DbContextConfigurer.Configure(options, builder.Configuration);
});
 
// Enables MVC controllers and views.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();

// Enables session management to store user information across requests.
builder.Services.AddSession();

// Build the Web Application
var app = builder.Build();

// If the application is running in production mode, enable exception handling and HTTP Strict Transport Security (HSTS).
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Redirects errors to a custom error page.
    app.UseHsts(); // Enforces HTTPS for better security.
}

//Middleware Configuration
// Enables session support.
app.UseSession();
// Enables request routing (i.e., directing HTTP requests to the correct controller and action).
app.UseRouting();
// Enables authentication (checks user credentials).
app.UseAuthentication();
// Enables authorization (checks user permissions).
app.UseAuthorization();
// `MapStaticAssets()` is not a standard method. Ensure it's correctly implemented if used for serving static files.
app.MapStaticAssets(); 
 
//Define Default Routing
 
// Maps requests to the "Home" controller, "Index" action as the default route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets(); // ⚠️ Ensure `.WithStaticAssets()` is defined somewhere, or replace with `UseStaticFiles()`


 
//Start the Application
app.Run();
