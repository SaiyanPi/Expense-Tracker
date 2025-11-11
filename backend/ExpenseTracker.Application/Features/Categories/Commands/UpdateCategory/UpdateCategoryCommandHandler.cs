using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);
        
        // business rule: category name must be unique per user
        if (request.UserId != null)
        {
            var exists = await _categoryRepository.ExistsByNameAndUserIdAsync(request.Name, request.UserId, cancellationToken);
            if (exists && !string.Equals(category.Name, request.Name, StringComparison.OrdinalIgnoreCase))
                throw new ValidationException($"Category with name '{request.Name}' already exists for user '{request.UserId}'.");
        }

        _mapper.Map(request, category);

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        return Unit.Value;
    }
}