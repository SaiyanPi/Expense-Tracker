using AutoMapper;
using ExpenseTracker.API.Contracts.V1.Budget;
using ExpenseTracker.API.Contracts.V1.Category;
using ExpenseTracker.Application.DTOs.Budget;
using ExpenseTracker.Application.DTOs.Category;


namespace ExpenseTracker.API.Contracts.V1.Common.Mappings;

public class BudgetMappingProfile : Profile
{
    public BudgetMappingProfile()
    {
        CreateMap<CreateBudgetRequestV1, CreateBudgetDto>();
        CreateMap<BudgetDto, BudgetResponseV1>();
        CreateMap<BudgetSummaryDto, BudgetSummaryResponseV1>();
        CreateMap<BudgetCategorySummaryDto, BudgetCategorySummaryResponseV1>();
    }
}

// here we map contract request objects to application DTOs