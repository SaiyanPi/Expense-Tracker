using ExpenseTracker.Persistence.Identity;
using ExpenseTracker.Persistence.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Persistence.DI;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ExpenseTrackerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            // Configure Identity options here
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = false;
        })
            .AddRoles<IdentityRole>() // enable roles
            .AddEntityFrameworkStores<ExpenseTrackerDbContext>()
            .AddSignInManager() // needed for login with password
            .AddDefaultTokenProviders();


        services.AddAutoMapper(
            typeof(UserPersistenceMappingProfile).Assembly
        );

        return services;
    }
}
