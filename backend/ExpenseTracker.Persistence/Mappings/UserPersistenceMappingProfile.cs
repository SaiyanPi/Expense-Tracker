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
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty));


        // 💡 following codes are commented because
        // though it works, it voiolates identity rules and may cause bugs
        // so using AutoMapper for reading and using manual mapping for creating and updating users
        // is a good practice.


        // reverse map: Creating/updating user from domain model (Domain User → ApplicationUser)
        // CreateMap<User, ApplicationUser>()
        //     .ForMember(dest => dest.Id, opt => opt.Ignore())
        //     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
        //     .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        //     .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
    }

}


//📝 UserPersistenceMappingProfile maps the ApplicationUser(Identity-specific) and Domain User
// since we've split the user into persistence application user and domain user to maintain the
// clean architecture.

