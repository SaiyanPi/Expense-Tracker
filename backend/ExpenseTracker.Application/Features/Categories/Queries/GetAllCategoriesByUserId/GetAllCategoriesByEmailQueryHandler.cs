using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetAllCategoriesByEmail;

public class GetAllCategoriesByEmailQueryHandler : IRequestHandler<GetAllCategoriesByEmailQuery, PagedResult<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllCategoriesByEmailQueryHandler(
        ICategoryRepository categoryRepository, 
        IUserRepository userRepository, 
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<CategoryDto>> Handle(
        GetAllCategoriesByEmailQuery request, 
        CancellationToken cancellationToken)
    {        
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.Email);

        var query = request.Paging;

        var (categories, totalCount) = await _categoryRepository.GetAllCategoriesByEmailAsync(
            user.Id, 
            skip: query.Skip,
            take: query.EffectivePageSize,
            sortBy: query.SortBy,
            sortDesc: query.SortDesc,
            cancellationToken: cancellationToken);
        
        var mappedCategories = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
        return new PagedResult<CategoryDto>(
            mappedCategories,
            totalCount,
            query.EffectivePage,
            query.EffectivePageSize);
    }
}