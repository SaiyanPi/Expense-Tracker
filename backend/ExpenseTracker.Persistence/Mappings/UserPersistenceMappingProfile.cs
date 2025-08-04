using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence.Identity;

namespace ExpenseTrackler.Persistence.Mappings;

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


//📝 UserMappingProfile is located here in Persistence layer because Application layer does not have
// a reference to a persistence layer where ApplicationUser resides. It is fine to place the
// UserMappingProfile here
