using AutoMapper;
using ExpenseTracker.API.Contracts.V1.Expense;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.DTOS.Expense;

namespace ExpenseTracker.API.Contracts.V1.Common.Mappings;

public class ExpenseMappingProfile : Profile
{
    public ExpenseMappingProfile()
    {
        CreateMap<CreateExpenseRequestV1, CreateExpenseDto>();
        CreateMap<ExpenseDto, ExpenseResponseV1>();
        CreateMap<ExpenseSummaryForCategoryDto, ExpenseSummaryForCategoryResponseV1>();
        CreateMap<ExpenseSummaryForBudgetDto, ExpenseSummaryForBudgetResponseV1>();
    }
}

// here we map contract request objects to application DTOs