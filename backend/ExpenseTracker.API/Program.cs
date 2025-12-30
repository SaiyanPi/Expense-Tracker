using ExpenseTracker.Persistence.DI;
using ExpenseTracker.Application.DI;
using ExpenseTracker.Infrastructure.DI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ExpenseTracker.Persistence;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using ExpenseTracker.API.Middleware;
using FluentValidation.AspNetCore;
using FluentValidation;
using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Application.Common.Authorization;
using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// qualifying for community license (required for pdf export)
QuestPDF.Settings.License = LicenseType.Community;


// Register services, repositories, etc.
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add controllers / minimal APIs
builder.Services.AddControllers();

// Modern FluentValidation registration
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
// Register all validators from Application assembly
// builder.Services.AddValidatorsFromAssemblyContaining<CreateExpenseDtoValidator>();
builder.Services.AddValidatorsFromAssembly(typeof(ExpenseTracker.Application.AssemblyReference).Assembly);


builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT authentication 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtConfig");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is missing from configuration."))
        )
    };
});

// add authorization policies
builder.Services.AddAuthorization(options =>
{
    // Expense policies
    options.AddPolicy(ExpensePermission.View, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, ExpensePermission.View));
    options.AddPolicy(ExpensePermission.Create, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, ExpensePermission.Create));
    options.AddPolicy(ExpensePermission.Update, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, ExpensePermission.Update));
    options.AddPolicy(ExpensePermission.Delete, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, ExpensePermission.Delete));
    options.AddPolicy(ExpensePermission.ViewAll, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, ExpensePermission.ViewAll));
    
    // Expense filter policy(access to both admin and regular user)
    options.AddPolicy("Expense.Filter", policy =>
        policy.RequireAssertion(context => 
        context.User.HasClaim(AppClaimTypes.Permission, ExpensePermission.View) ||
        context.User.HasClaim(AppClaimTypes.Permission, ExpensePermission.ViewAll)
        ));

    // Category policies
    options.AddPolicy(CategoryPermission.View, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, CategoryPermission.View));
    options.AddPolicy(CategoryPermission.Create, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, CategoryPermission.Create));
    options.AddPolicy(CategoryPermission.Update, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, CategoryPermission.Update));
    options.AddPolicy(CategoryPermission.Delete, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, CategoryPermission.Delete));
    options.AddPolicy(CategoryPermission.ViewAll, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, CategoryPermission.ViewAll));

    // Budget policies
    options.AddPolicy(BudgetPermission.View, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, BudgetPermission.View));
    options.AddPolicy(BudgetPermission.Create, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, BudgetPermission.Create));
    options.AddPolicy(BudgetPermission.Update, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, BudgetPermission.Update));
    options.AddPolicy(BudgetPermission.Delete, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, BudgetPermission.Delete));
    options.AddPolicy(BudgetPermission.ViewAll, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, BudgetPermission.ViewAll));
    
    // Dashboard policy
    options.AddPolicy(DashboardPermission.View, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, DashboardPermission.View));
    
    // UserManagement policy
    options.AddPolicy(UserManagementPermission.All, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, UserManagementPermission.All));
    
    // AuditLog policy
    options.AddPolicy(AuditLogPermission.View, policy =>
        policy.RequireClaim(AppClaimTypes.Permission, AuditLogPermission.View));
    
   

    
});


var app = builder.Build();

// Run the seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ExpenseTrackerDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await ExpenseTrackerDbContextSeed.SeedAsync(context, userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register exception middleware FIRST in pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();   // comes BEFORE Authorization

app.UseAuthorization();

app.MapControllers();

app.Run();


