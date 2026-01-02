using AutoMapper;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Application.DTOs.Expense;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetAllDeletedCategoriesByEmail;

public class GetAllDeletedCategoriesByEmailQueryHandler 
    : IRequestHandler<GetAllDeletedCategoriesByEmailQuery, PagedResult<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetAllDeletedCategoriesByEmailQueryHandler(
        ICategoryRepository categoryRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

     public async Task<PagedResult<CategoryDto>> Handle(
        GetAllDeletedCategoriesByEmailQuery request,
        CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can view their own deleted categorys

        var userId = _userAccessor.UserId;
        
        var query = request.Paging;

        var(softDeletedCategories, totalCount) = await _categoryRepository.GetAllDeletedCategoriesByEmailAsync(
            userId,
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedCategories = _mapper.Map<IReadOnlyList<CategoryDto>>(softDeletedCategories);
        return new PagedResult<CategoryDto>(mappedCategories, totalCount, query.EffectivePage, query.EffectivePageSize);
    }
}
