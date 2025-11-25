using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Infrastructure.Identity.Services;
using ExpenseTracker.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure.DI;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // registering repositories
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();

        // registering identity service
        services.AddScoped<IIdentityService, IdentityService>();

        // registering JWT token service
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        
        return services;
    }
}

