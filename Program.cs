
// Import necessary namespaces for authentication, database access, and ASP.NET Core configuration.
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagementApp.MVC;


// Create a new instance of the WebApplication builder.
var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Jwt.Settings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddScoped<IAuthService, AuthenticationService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IStudentRepository, StudentRepository>(); 
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();

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
