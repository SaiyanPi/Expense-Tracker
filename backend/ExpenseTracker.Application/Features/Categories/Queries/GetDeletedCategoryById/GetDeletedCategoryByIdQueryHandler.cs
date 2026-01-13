using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;
using static ExpenseTracker.Application.Common.Observability.Metrics.MetricsConstants;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetDeletedCategoryById;

public class GetDeletedCategoryByIdQueryHandler : IRequestHandler<GetDeletedCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public GetDeletedCategoryByIdQueryHandler(
        ICategoryRepository categoryRepository,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetDeletedCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        var category = await _categoryRepository.GetDeletedCategoryAsync(request.Id, userId, cancellationToken);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);

        return _mapper.Map<CategoryDto>(category);
    }
}