using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTrackler.Application.DTOs.Expense;

namespace ExpenseTrackler.Application.Mappings;

public class ExpenseMappingProfile : Profile
{
    public ExpenseMappingProfile()
    {
        // Create -> Entity
        CreateMap<CreateExpenseDto, Expense>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())       // generated in code/db
            .ForMember(dest => dest.Category, opt => opt.Ignore()); // nav property (EF will handle)

        // Update -> Entity
        CreateMap<UpdateExpenseDto, Expense>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())       // never update Id
            .ForMember(dest => dest.UserId, opt => opt.Ignore())   // UserId itself can't be updated
            .ForMember(dest => dest.Category, opt => opt.Ignore());

        // required to populate an edit form with existing Expense data
        CreateMap<Expense, UpdateExpenseDto>();

        // Entity -> Read DTO
        CreateMap<Expense, ExpenseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    }
    
}