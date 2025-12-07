using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Expenses.Queries.GetCategorySummaryByEmail;

public class GetCategorySummaryByEmailQueryHandler : IRequestHandler<GetCategorySummaryByEmailQuery, List<CategorySummaryDto>>
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetCategorySummaryByEmailQueryHandler(IExpenseRepository expenseRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _userRepository = userRepository;   
        _mapper = mapper;
    }

    public async Task<List<CategorySummaryDto>> Handle(GetCategorySummaryByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(CategorySummaryDto), request.Email);
   
        var categorySummaryByEmail = await _expenseRepository.GetCategorySummaryByEmailAsync(user.Id, cancellationToken);
        // var categorySummaryDto = categorySummaryByEmail
        //     .Select(cs => new CategorySummaryDto
        //     {
        //         CategoryName = cs.CategoryName,
        //         TotalAmount = cs.TotalAmount
        //     })
        //     .ToList();

        return _mapper.Map<List<CategorySummaryDto>>(categorySummaryByEmail) ;
    }
}