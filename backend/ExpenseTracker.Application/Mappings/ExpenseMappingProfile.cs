using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTrackler.Application.DTOs.Expense;

namespace ExpenseTrackler.Application.Mappings;

public class ExpenseMappingProfile : Profile
{
    public ExpenseMappingProfile()
    {
        // Create -> Entity
        CreateMap<CreateExpenseDto, Expense>();

        // Update -> Entity
        CreateMap<UpdateExpenseDto, Expense>();

        // required to populate an edit form with existing Expense data
        CreateMap<Expense, UpdateExpenseDto>();

        // Entity -> Read DTO
        CreateMap<Expense, ExpenseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    }
    
}