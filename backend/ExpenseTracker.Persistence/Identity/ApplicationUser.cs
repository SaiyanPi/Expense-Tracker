using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Persistence.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = default!;
    
    // conversion/Mapping method to convert the ApplicationUser(IdentityUser) object into a Domain.Entities.User object
    public Domain.Entities.User ToDomainUser() => new Domain.Entities.User
    {
        Id = this.Id, // though Id is not defined here in ApplicationUser it is inherited from IdentityUser
        FullName = this.FullName
    };
}

