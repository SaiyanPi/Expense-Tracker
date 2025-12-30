using System.Security.Claims;
using ExpenseTracker.Application.Common.Authorization;
using ExpenseTracker.Application.Common.Authorization.Permissions;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence;
public class ExpenseTrackerDbContextSeed
{
    public static async Task SeedAsync(
        ExpenseTrackerDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Apply migrations automatically (optional but useful for dev)
        await dbContext.Database.MigrateAsync();

        // 1. Seed Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // 1.1 seed role claims (permissions)
        var adminRole = await roleManager.FindByNameAsync("Admin");
        var userRole = await roleManager.FindByNameAsync("User");

        if (adminRole != null)
        {
            await AddPermissionAsync(roleManager, adminRole, CategoryPermission.ViewAll);
            await AddPermissionAsync(roleManager, adminRole, CategoryPermission.Create);
            await AddPermissionAsync(roleManager, adminRole, CategoryPermission.Update);
            await AddPermissionAsync(roleManager, adminRole, CategoryPermission.Delete);
            await AddPermissionAsync(roleManager, adminRole, CategoryPermission.View);

            await AddPermissionAsync(roleManager, adminRole, ExpensePermission.ViewAll);

            await AddPermissionAsync(roleManager, adminRole, BudgetPermission.ViewAll);

            await AddPermissionAsync(roleManager, adminRole, UserManagementPermission.All);

            await AddPermissionAsync(roleManager, adminRole, AuditLogPermission.View);


            // await AddPermissionAsync(roleManager, adminRole, ProfilePermission.View);
        }

        if (userRole != null)
        {
            await AddPermissionAsync(roleManager, userRole, CategoryPermission.View);
            await AddPermissionAsync(roleManager, userRole, CategoryPermission.Create);
            await AddPermissionAsync(roleManager, userRole, CategoryPermission.Update);
            await AddPermissionAsync(roleManager, userRole, CategoryPermission.Delete);

            await AddPermissionAsync(roleManager, userRole, BudgetPermission.View);
            await AddPermissionAsync(roleManager, userRole, BudgetPermission.Create);
            await AddPermissionAsync(roleManager, userRole, BudgetPermission.Update);
            await AddPermissionAsync(roleManager, userRole, BudgetPermission.Delete);

            await AddPermissionAsync(roleManager, userRole, ExpensePermission.View);
            await AddPermissionAsync(roleManager, userRole, ExpensePermission.Create);
            await AddPermissionAsync(roleManager, userRole, ExpensePermission.Update);
            await AddPermissionAsync(roleManager, userRole, ExpensePermission.Delete);

            await AddPermissionAsync(roleManager, userRole, DashboardPermission.View);

            // await AddPermissionAsync(roleManager, userRole, ProfilePermission.View);
            // await AddPermissionAsync(roleManager, userRole, ProfilePermission.Update);
            // await AddPermissionAsync(roleManager, userRole, ProfilePermission.Delete);

        }

        // 2. Seed users
        if (!userManager.Users.Any())
        {
            // i. Admin User
            var adminUser = new ApplicationUser
            {
                FullName = "John Doe",
                UserName = "admin1",
                Email = "admin1@expensetracker.com",
                EmailConfirmed = true,
                PhoneNumber = "+1234567890",
                PhoneNumberConfirmed = true

            };

            var adminUserResult = await userManager.CreateAsync(adminUser, "Admin@123");

            if (adminUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // ii. Regular User
            var regularUser = new ApplicationUser
            {
                FullName = "John Doe",
                UserName = "user1",
                Email = "user1@expensetracker.com",
                EmailConfirmed = true,
                PhoneNumber = "+0987654321",
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(regularUser, "User@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }

        }

        // 3. Seed Categories
        if (!dbContext.Categories.Any())
        {
            // Assume users exist
            var regularUser = await userManager.FindByEmailAsync("user1@expensetracker.com");
            
            if (regularUser != null)
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Food", UserId = regularUser?.Id },
                    new Category { Name = "Transport", UserId = regularUser?.Id },
                    new Category { Name = "Shopping", UserId = regularUser?.Id },
                    new Category { Name = "Bills", UserId = regularUser?.Id },
                    new Category { Name = "Other", UserId = regularUser?.Id }
                };

                await dbContext.Categories.AddRangeAsync(categories);
                await dbContext.SaveChangesAsync();
                    
            }
        
        }

        // 3. Seed Budget
        if (!dbContext.Budgets.Any())
        {
            // Assume user and categories already exist
            var regularUser = await userManager.FindByEmailAsync("user1@expensetracker.com");

            var foodCategory = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "Food");

            var transportCategory = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "Transport");

            if (regularUser != null)
            {
                var budgets = new List<Budget>
                {
                    // Category-based budget
                    new Budget(
                        name: "Family Dinner",
                        amount: 8000m,
                        startDate: DateTime.UtcNow.AddDays(-5),
                        endDate: DateTime.UtcNow.AddDays(25),
                        userId: regularUser.Id,
                        categoryId: foodCategory?.Id
                    ),

                    // Overall (no category) budget
                    new Budget(
                        name: "Treek",
                        amount: 22000m,
                        startDate: DateTime.UtcNow.AddDays(-10),
                        endDate: DateTime.UtcNow.AddDays(30),
                        userId: regularUser.Id
                    )
                };

                await dbContext.Budgets.AddRangeAsync(budgets);
                await dbContext.SaveChangesAsync();
            }
        }


        // 5. Seed Expenses
        if (!dbContext.Expenses.Any())
        {
            // Assume user, categories, and budgets already exist
            var regularUser = await userManager.FindByEmailAsync("user1@expensetracker.com");

            var foodCategory = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "Food");

            var transportCategory = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == "Transport");

            var foodBudget = await dbContext.Budgets
                .FirstOrDefaultAsync(b => b.Name == "Family Dinner");

            if (regularUser != null &&
                foodCategory != null &&
                transportCategory != null)
            {
                var expenses = new List<Expense>
                {
                    new Expense
                    {
                        Title = "Drinks",
                        Amount = 2500.50m,
                        Description = "cold drinks and hard",
                        Date = DateTime.UtcNow.AddDays(-2),
                        CategoryId = foodCategory.Id,
                        BudgetId = foodBudget?.Id,
                        UserId = regularUser.Id
                    },
                    new Expense
                    {
                        Title = "Fish",
                        Amount = 1500.00m,
                        Description = "Fish curry",
                        Date = DateTime.UtcNow.AddDays(-1),
                        CategoryId = foodCategory.Id,
                        BudgetId = foodBudget?.Id,
                        UserId = regularUser.Id
                    },
                    new Expense
                    {
                        Title = "Bus Ticket",
                        Amount = 10.00m,
                        Description = "Daily commute",
                        Date = DateTime.UtcNow.AddDays(-1),
                        CategoryId = transportCategory.Id,
                        BudgetId = null,
                        UserId = regularUser.Id
                    }
                };

                await dbContext.Expenses.AddRangeAsync(expenses);
                await dbContext.SaveChangesAsync();
            }
        }

    }

    // helper: add permission 
    private static async Task AddPermissionAsync(
        RoleManager<IdentityRole> roleManager,
        IdentityRole role,
        string permission)
    {
        var claims = await roleManager.GetClaimsAsync(role);

        if (!claims.Any(c =>
                c.Type == AppClaimTypes.Permission &&
                c.Value == permission))
        {
            await roleManager.AddClaimAsync(
                role,
                new Claim(AppClaimTypes.Permission, permission));
        }
    }


}
