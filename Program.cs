
// Import necessary namespaces for authentication, database access, and ASP.NET Core configuration.
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC;


// Create a new instance of the WebApplication builder.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthService, AuthenticationService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IStudentRepository, StudentRepository>(); 

builder.Services.AddDbContext<SchoolManagementAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));


//   Authentication Setup
// Configures cookie-based authentication so that users must log in before accessing protected pages.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Account/Login";
        option.AccessDeniedPath = "/Account/AccessDenied"; 
        option.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        option.SlidingExpiration = true; 

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
builder.Services.AddSession();

// Enables session management to store user information across requests.
builder.Services.AddSession();

var app = builder.Build();

// If the application is running in production mode, enable exception handling and HTTP Strict Transport Security (HSTS).
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); 
}

//Middleware Configuration
// Enables session support.
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets(); 
 
//Define Default Routing
 
// Maps requests to the "Home" controller, "Index" action as the default route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets(); // Ensure `.WithStaticAssets()` is defined somewhere, or replace with `UseStaticFiles()`


 
//Start the Application
app.Run();
