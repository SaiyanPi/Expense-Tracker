using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence.Identity;

namespace ExpenseTracker.Persistence.Mappings;

public class UserPersistenceMappingProfile : Profile
{
    public UserPersistenceMappingProfile()
    {
        // map: Reading user info (ApplicationUser → Domain User)
        CreateMap<ApplicationUser, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        // reverse map: Creating/updating user from domain model (Domain User → ApplicationUser)
        CreateMap<User, ApplicationUser>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
    }

}


//📝 UserPersistenceMappingProfile maps the ApplicationUser(Identity-specific) and Domain User
// since we've split the user into persistence application user and domain user to maintain the
// clean architecture.

