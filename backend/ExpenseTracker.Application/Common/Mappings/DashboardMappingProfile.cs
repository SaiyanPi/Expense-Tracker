using AutoMapper;
using ExpenseTracker.Application.DTOs.Dashboard;
using ExpenseTracker.Application.DTOS.Dashboard;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Models;

namespace ExpenseTracker.Application.Common.Mappings;

public class DashboardMappingProfile : Profile
{
    public DashboardMappingProfile()
    {
        CreateMap<DashboardCategoryExpenseSummary, CategoryExpenseDto>();
        CreateMap<DashboardBudgetUtilizationSummary,BudgetUtilizationDto>();
        CreateMap<DashboardDailyExpenseSummary, DailyExpenseDto>();
        CreateMap<Expense, RecentExpenseDto>();
    }
    
}