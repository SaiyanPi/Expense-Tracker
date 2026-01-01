using ExpenseTracker.Application.Common.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ExpenseTracker.Persistence;
public class ExpenseTrackerDbContextFactory : IDesignTimeDbContextFactory<ExpenseTrackerDbContext>
{
    public ExpenseTrackerDbContext CreateDbContext(string[] args)
    {
        // Get current directory (Persistence project)
        var basePath = Directory.GetCurrentDirectory();

        // Navigate up to solution root
        var solutionRoot = Path.GetFullPath(Path.Combine(basePath, ".."));

        // Build path to API's appsettings.json
        var apiProjectPath = Path.Combine(solutionRoot, "ExpenseTracker.API");

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath) // this will point to API project
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true ) // relative path to API project
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<ExpenseTrackerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ExpenseTrackerDbContext(
            optionsBuilder.Options,
            new SystemRequestContext(), 
            userAccessor: null);
    }
}
