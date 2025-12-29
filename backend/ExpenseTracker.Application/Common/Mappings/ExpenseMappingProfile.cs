using AutoMapper;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Application.Features.Expenses.Commands.UpdateExpense;
using ExpenseTracker.Domain.Models;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Application.DTOS.Expense;

namespace ExpenseTracker.Application.Common.Mappings;

public class ExpenseMappingProfile : Profile
{
    public ExpenseMappingProfile()
    {
        // Create -> Entity
        CreateMap<CreateExpenseDto, Expense>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())       // generated in code/db
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Update -> Entity
        CreateMap<UpdateExpenseDto, Expense>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())       // never update Id
            .ForMember(dest => dest.UserId, opt => opt.Ignore())   // UserId itself can't be updated
            .ForMember(dest => dest.Category, opt => opt.Ignore());


        // MediatR UpdateCommand -> Entity
        CreateMap<UpdateExpenseCommand, Expense>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore());

        // required to populate an edit form with existing Expense data
        CreateMap<Expense, UpdateExpenseDto>();

        // Entity -> Read DTO
        CreateMap<Expense, ExpenseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        // category summary
        CreateMap<CategorySummary, CategorySummaryDto>();

        // mapping for ExpenseSummaryForBudget to ExpenseDto
        CreateMap<ExpenseSummaryForBudget, ExpenseSummaryForBudgetDto>();

        // mapping for ExpenseSummaryForCategory to ExpenseDto
        CreateMap<ExpenseSummaryForCategory, ExpenseSummaryForCategoryDto>();

        // Partial mapping for FilteredExpenseDto
        // CreateMap<Expense, FilteredExpenseDto>()
        //     .ForMember(dest => dest.CategoryName, opt => opt.Ignore());

        // Entity -> Export DTO
        CreateMap<Expense, ExpenseExportDto>()
            .ForMember(dest => dest.Budget, opt => opt.MapFrom(src => src.Budget != null ? src.Budget.Name : null))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name));
           
    }
    
}