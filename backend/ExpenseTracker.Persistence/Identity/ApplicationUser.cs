using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Persistence.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = default!;
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();


    // for refresh token
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }


    // conversion/Mapping method to convert the ApplicationUser(IdentityUser) object into a Domain.Entities.User object
    public User ToDomainUser() => new()
    {
        Id = this.Id, // though Id is not defined here in ApplicationUser it is inherited from IdentityUser
        FullName = this.FullName,
        Email = this.Email ?? string.Empty
    };
}

