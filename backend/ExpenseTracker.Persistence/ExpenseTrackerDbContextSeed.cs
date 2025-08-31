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

        // 2. Seed Default Admin User
        if (!userManager.Users.Any())
        {
            var adminUser = new ApplicationUser
            {
                FullName = "Admin User",
                UserName = "admin@expensetracker.com",
                Email = "admin@expensetracker.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. Seed Categories
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
                {
                    new Category { Name = "Food" },
                    new Category { Name = "Transport" },
                    new Category { Name = "Shopping" },
                    new Category { Name = "Bills" },
                    new Category { Name = "Other" }
                };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // 4. Seed Expenses
        if (!context.Expenses.Any())
        {
            // Assume categories are seeded first
            var foodCategory = context.Categories.FirstOrDefault(c => c.Name == "Food");
            var transportCategory = context.Categories.FirstOrDefault(c => c.Name == "Transport");

            // Assume admin user exists
            var adminUser = userManager.FindByEmailAsync("admin@expensetracker.com").Result;

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
