using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTrackler.Application.DTOs.User;

namespace ExpenseTrackler.Application.Mappings;

public class UserDomainMappingProfile : Profile
{
    public UserDomainMappingProfile()
    {
        // Create -> Entity
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Update -> Entity
        CreateMap<UpdateUserDto, User>();

        // required to populate an edit form with existing user data
        CreateMap<User, UpdateUserDto>();

        // Entity -> Read DTO
        CreateMap<User, UserDto>();
    }

}

// 📝 UserDomainMappingProfile maps domain user and DTO.
// This keeps the Application layer clean and decoupled from persistence details likeApplicationUser from
// Identity