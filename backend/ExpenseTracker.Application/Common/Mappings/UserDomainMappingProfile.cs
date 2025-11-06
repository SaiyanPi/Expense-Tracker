using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Application.DTOs.User;

namespace ExpenseTracker.Application.Common.Mappings;

public class UserDomainMappingProfile : Profile
{
    public UserDomainMappingProfile()
    {
        // Create -> Entity
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Update -> Entity
        CreateMap<UpdateUserDto, User>().ReverseMap();

        // required to populate an edit form with existing user data
        // CreateMap<User, UpdateUserDto>();

        // Entity -> Read DTO
        CreateMap<User, UserDto>().ReverseMap();
    }

}

// 📝 UserDomainMappingProfile maps domain user and DTO.
// This keeps the Application layer clean and decoupled from persistence details like ApplicationUser from
// Identity