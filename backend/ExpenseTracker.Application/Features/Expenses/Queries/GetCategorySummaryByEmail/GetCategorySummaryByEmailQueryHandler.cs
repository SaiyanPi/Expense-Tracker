using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummaryByEmail;

public class GetCategorySummaryByEmailQueryHandler : IRequestHandler<GetCategorySummaryByEmailQuery, PagedResult<CategorySummaryDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetCategorySummaryByEmailQueryHandler(
        IExpenseRepository expenseRepository,
        IUserRepository userRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;
        _userAccessor = userAccessor;   
        _mapper = mapper;
    }

    public async Task<PagedResult<CategorySummaryDto>> Handle(
        GetCategorySummaryByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var query = request.Paging;

        var (categorySummaryByEmail, totalCount) = await _expenseRepository.GetCategorySummaryByEmailAsync(
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        // var categorySummaryDto = categorySummaryByEmail
        //     .Select(cs => new CategorySummaryDto
        //     {
        //         CategoryName = cs.CategoryName,
        //         TotalAmount = cs.TotalAmount
        //     })
        //     .ToList();

        var mappedCategorySummaryByEmail = _mapper.Map<IReadOnlyList<CategorySummaryDto>>(categorySummaryByEmail);
        return new PagedResult<CategorySummaryDto>(mappedCategorySummaryByEmail, totalCount, query.EffectivePage, query.EffectivePageSize);
    }
}