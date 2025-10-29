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


var builder = WebApplication.CreateBuilder(args);

// Register services, repositories, etc.
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();

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


