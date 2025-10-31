using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence;
public class ExpenseTrackerDbContextSeed
{
    public static async Task SeedAsync(
        ExpenseTrackerDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Apply migrations automatically (optional but useful for dev)
        await context.Database.MigrateAsync();

        // 1. Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // 2. Seed 
        if (!userManager.Users.Any())
        {
            // i. Admin User
            var adminUser = new ApplicationUser
            {
                FullName = "Admin User",
                UserName = "admin@expensetracker.com",
                Email = "admin@expensetracker.com",
                EmailConfirmed = true
            };

            var adminUserResult = await userManager.CreateAsync(adminUser, "Admin@123");

            if (adminUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // ii. Regular User
            var regularUser = new ApplicationUser
            {
                FullName = "Regular User",
                UserName = "user@expensetracker.com",
                Email = "user@expensetracker.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(regularUser, "User@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }

        }

        // 3. Seed Categories
        if (!context.Categories.Any())
        {
            // Assume users exist
            var adminUser = await userManager.FindByEmailAsync("admin@expensetracker.com");
            var regularUser = await userManager.FindByEmailAsync("user@expensetracker.com");
            
            if (adminUser != null && regularUser != null)
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Food", UserId = adminUser?.Id },
                    new Category { Name = "Transport", UserId = regularUser?.Id },
                    new Category { Name = "Shopping", UserId = adminUser?.Id },
                    new Category { Name = "Bills", UserId = regularUser?.Id },
                    new Category { Name = "Other", UserId = regularUser?.Id }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
                    
            }
        
        }

        // 4. Seed Expenses
        if (!context.Expenses.Any())
        {
            // Assume categories are seeded first
            var foodCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Food");
            var transportCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Transport");

            // Assume admin user exists
            var adminUser = await userManager.FindByEmailAsync("admin@expensetracker.com");

            if (foodCategory != null && transportCategory != null && adminUser != null)
            {
                context.Expenses.AddRange(
                    new Expense
                    {
                        Title = "Lunch",
                        Amount = 25.50m,
                        Description = "Lunch at local cafe",
                        Date = DateTime.UtcNow.AddDays(-2),
                        CategoryId = foodCategory.Id,
                        UserId = adminUser.Id
                    },
                    new Expense
                    {
                        Title = "Travel",
                        Amount = 10.00m,
                        Description = "Bus ticket",
                        Date = DateTime.UtcNow.AddDays(-1),
                        CategoryId = transportCategory.Id,
                        UserId = adminUser.Id
                    }
                );

                await context.SaveChangesAsync();
            }
        }

    }
}
