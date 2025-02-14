
// Import necessary namespaces for authentication, database access, and ASP.NET Core configuration.
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagementApp.MVC;


// Create a new instance of the WebApplication builder.
var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"];

if (string.IsNullOrEmpty(secret))
{
    throw new InvalidOperationException("JWT Secret is not configured properly.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>{
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
                try
                {
                    var token = context.HttpContext.Session.GetString("JWTToken");
                    if (string.IsNullOrEmpty(token))
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Account/Login");
                    }
                    return Task.CompletedTask;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"❌{e.Message}");
                    // context.HandleResponse();
                    context.Response.Redirect("/Account/Login");
                    return Task.CompletedTask;
                }
                // Redirect to login when authentication fails
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["jwtSettings:Issuer"],
            ValidAudience = builder.Configuration["jwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
        };
});

// Enables MVC controllers and views.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthorization();
builder.Services.AddSession();
builder.Services.AddSession();
//register services
builder.Services.AddScoped<IAuthService, AuthenticationService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>(); 
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();

//configure DB
builder.Services.AddDbContext<SchoolManagementAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddDbContext<SchoolManagementAppDbContext>(options =>
{
    DbContextConfigurer.Configure(options, builder.Configuration);
});

var app = builder.Build();

// If the application is running in production mode, enable exception handling and HTTP Strict Transport Security (HSTS).
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}else{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//Middleware Configuration
// Enables session support.
app.UseSession();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
// app.MapStaticAssets(); 
// app.MapControllers();
  
// Configure Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Urls.Add("http://localhost:5000");
app.Urls.Add("https://localhost:7001");

 
//Start the Application
app.Run();
