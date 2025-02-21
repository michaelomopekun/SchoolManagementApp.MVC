// Import necessary namespaces for authentication, database access, and ASP.NET Core configuration.
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagementApp.MVC;
using SchoolManagementApp.MVC.Authorization;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Repository;
using SchoolManagementApp.MVC.Services;

// Create a new instance of the WebApplication builder.
var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"];

if (string.IsNullOrEmpty(secret))
{
    throw new InvalidOperationException("JWT Secret is not configured properly.");
}

// Configure DbContext
builder.Services.AddDbContext<SchoolManagementAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("SchoolManagementApp.MVC")));

// Add Identity with SignInManager and UserManager
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<SchoolManagementAppDbContext>()
.AddDefaultTokenProviders()
.AddSignInManager<SignInManager<IdentityUser>>()
.AddUserManager<UserManager<IdentityUser>>();

// Configure Cookie Policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
});

// Register required Identity services
builder.Services.AddScoped<SignInManager<IdentityUser>>();
builder.Services.AddScoped<UserManager<IdentityUser>>();

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString("JWTToken")))
            {
                context.HandleResponse();
                context.Response.Redirect("/Account/Login");
            }
            return Task.CompletedTask;
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(secret))
    };
});

// Configure Services
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Authorization
builder.Services.AddAuthorization(options => 
{
    //Role-based policies
    options.AddPolicy(PolicyNames.RequireAdmin, policy =>
        policy.RequireRole(UserRole.Admin.ToString()));
    options.AddPolicy(PolicyNames.RequireLecturer, policy =>
        policy.RequireRole(UserRole.Lecturer.ToString()));
    options.AddPolicy(PolicyNames.RequireStudent, policy =>
        policy.RequireRole(UserRole.Student.ToString()));

    options.AddPolicy(PolicyNames.ManageUsers, policy =>
        policy.Requirements.Add(new PermissionRequirement(PolicyNames.ManageUsers)));
    options.AddPolicy(PolicyNames.ManageCourses, policy =>
        policy.Requirements.Add(new PermissionRequirement(PolicyNames.ManageCourses)));
    options.AddPolicy(PolicyNames.ManageCourses, policy =>
        policy.Requirements.Add(new PermissionRequirement(PolicyNames.ManageCourses)));
    options.AddPolicy(PolicyNames.ManageCourses, policy =>
        policy.Requirements.Add(new PermissionRequirement(PolicyNames.ManageCourses)));
    options.AddPolicy(PolicyNames.ManageCourses, policy =>
        policy.Requirements.Add(new PermissionRequirement(PolicyNames.ManageCourses)));
    
});

// Register Services
builder.Services.AddScoped< PermissionHandler>();
builder.Services.AddScoped<IAuthService, AuthenticationService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Urls.Add("http://localhost:5000");
app.Urls.Add("https://localhost:7001");

app.Run();
