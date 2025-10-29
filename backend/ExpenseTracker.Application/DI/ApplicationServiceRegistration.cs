using System.Reflection;
using ExpenseTracker.Application.Interfaces.Services;
using ExpenseTracker.Application.Services;
using ExpenseTrackler.Application.Mappings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application.DI;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // register application services
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();

        // registering JWT token service
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // registering AutoMapper profiles
        services.AddAutoMapper(
            typeof(ExpenseMappingProfile).Assembly,
            typeof(CategoryMappingProfile).Assembly,
            typeof(UserDomainMappingProfile).Assembly
        );
        
        // Register FluentValidation validators automatically
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
