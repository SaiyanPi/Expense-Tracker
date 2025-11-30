using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Infrastructure.Repositories;
using ExpenseTracker.Infrastructure.Services.Email;
using ExpenseTracker.Infrastructure.Services.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Infrastructure.DI;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
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
        // services.AddScoped<IJwtTokenService, JwtTokenService>();

        // SMTP config
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.AddSingleton(resolver =>
            resolver.GetRequiredService<IOptions<SmtpSettings>>().Value);

        // registering email service
        services.AddScoped<IEmailService, SmtpEmailService>();
        
        return services;
    }
}

