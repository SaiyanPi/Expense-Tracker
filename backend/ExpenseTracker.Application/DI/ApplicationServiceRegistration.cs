using System.Reflection;
using ExpenseTracker.Application.Common.Behaviours;
using ExpenseTracker.Application.Common.Context;
using ExpenseTracker.Application.Common.Mappings;
using FluentValidation;
using MediatR;
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
            typeof(BudgetMappingProfile).Assembly,
            typeof(DashboardMappingProfile).Assembly
        );

        // registering MediatR handlers
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
        );
        
        // Register FluentValidation validators automatically
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //  Register MediatR pipeline behavior for ExportExpensesValidator class
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        // Registering correlation + request metadata service
        services.AddHttpContextAccessor();
        services.AddScoped<IRequestContext, RequestContext>();

        return services;
    }
}
