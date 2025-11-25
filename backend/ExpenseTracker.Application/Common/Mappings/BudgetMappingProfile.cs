using AutoMapper;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Common.Mappings;

public class BudgetMappingProfile : Profile
{
    public BudgetMappingProfile()
    {
        // Create -> Entity
        CreateMap<CreateBudgetDto, Budget>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())       // generated in code/db
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true)); // new budgets are active by default

        // Update -> Entity
        CreateMap<UpdateBudgetDto, Budget>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())       // never update Id
            .ForMember(dest => dest.UserId, opt => opt.Ignore());   // UserId itself can't be updated
        
        // MediatR UpdateCommand -> Entity
        // CreateMap<UpdateBudgetCommand, Budget>()
        //     .ForMember(dest => dest.Id, opt => opt.Ignore())
        //     .ForMember(dest => dest.UserId, opt => opt.Ignore());

        // required to populate an edit form with existing Budget data
        CreateMap<Budget, UpdateBudgetDto>();
        
        // Entity -> Read DTO
        CreateMap<Budget, BudgetDto>();
    }
}