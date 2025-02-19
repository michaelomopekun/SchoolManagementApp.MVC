using Microsoft.EntityFrameworkCore;

namespace SchoolManagementApp.MVC
{
    public static class DbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder option, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (!option.IsConfigured)
            {
                option.UseSqlServer(connectionString);
            }

        }
    }

}