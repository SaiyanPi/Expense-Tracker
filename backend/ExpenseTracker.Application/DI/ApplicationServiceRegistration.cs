using System.Reflection;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Mappings;
using ExpenseTracker.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application.DI;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // register application services
        // services.AddScoped<IExpenseService, ExpenseService>();
        // services.AddScoped<ICategoryService, CategoryService>();
        // services.AddScoped<IUserService, UserService>();

        // registering AutoMapper profiles
        services.AddAutoMapper(
            typeof(ExpenseMappingProfile).Assembly,
            typeof(CategoryMappingProfile).Assembly,
            typeof(UserDomainMappingProfile).Assembly,
            typeof(BudgetMappingProfile).Assembly
        );

        // registering MediatR handlers
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
        );
        
        // Register FluentValidation validators automatically
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
