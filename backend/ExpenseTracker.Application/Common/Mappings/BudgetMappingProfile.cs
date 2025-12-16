using AutoMapper;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.Features.Budgets.Commands.UpdateBudget;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;
using ExpenseTrackerDomain.Models;

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
        CreateMap<UpdateBudgetCommand, Budget>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        // required to populate an edit form with existing Budget data
        CreateMap<Budget, UpdateBudgetDto>();
        
        // Entity -> Read DTO
        CreateMap<Budget, BudgetDto>();

        // Budget summary
        CreateMap<BudgetsSummary, BudgetSummaryDto>()
            .ForMember(dest => dest.Remaining, opt => opt.MapFrom(src => src.Remaining))
            .ForMember(dest => dest.UsedPercentage, opt => opt.MapFrom(src => src. UsedPercentage))
            .ForMember(dest => dest.IsOverBudget, opt => opt.MapFrom(src => src. IsOverBudget));
        
        CreateMap<BudgetCategorySummary, BudgetCategorySummaryDto>()
            .ForMember(dest => dest.Remaining, opt => opt.MapFrom(src => src.Remaining))
            .ForMember(dest => dest.UsedPercentage, opt => opt.MapFrom(src => src. UsedPercentage))
            .ForMember(dest => dest.IsOverBudget, opt => opt.MapFrom(src => src. IsOverBudget));
        // ---------------

        // for detailed budget view with expenses
        CreateMap<BudgetDetailWithExpensesSummary, BudgetDetailWithExpensesDto>()
            .ForMember(dest => dest.Remaining, opt => opt.MapFrom(src => src.Remaining))
            .ForMember(dest => dest.PercentageUsed, opt => opt.MapFrom(src => src. PercentageUsed))
            .ForMember(dest => dest.IsOverBudget, opt => opt.MapFrom(src => src. IsOverBudget));
        CreateMap<ExpenseSummary, ExpenseDto>();

    }
}