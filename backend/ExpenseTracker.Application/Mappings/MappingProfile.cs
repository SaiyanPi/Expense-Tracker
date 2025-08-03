using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTrackler.Application.DTOs.Expense;

namespace ExpenseTrackler.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Create -> Entity
        CreateMap<CreateExpenseDto, Expense>();

        // Update -> Entity
        CreateMap<UpdateExpenseDto, Expense>();

        // Entity -> Read DTO
        CreateMap<Expense, ExpenseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    }
    
}