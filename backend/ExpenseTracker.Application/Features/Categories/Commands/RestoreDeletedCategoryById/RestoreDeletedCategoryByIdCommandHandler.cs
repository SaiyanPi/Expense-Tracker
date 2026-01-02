using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Application.Features.Categories.Commands.RestoreDeletedCategoryById;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Commands.RestoreDeletedExpenseById;

public class RestoreDeletedCategoryByIdCommandHandler 
    : IRequestHandler<RestoreDeletedCategoryByIdCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserAccessor _userAccessor;

    public RestoreDeletedCategoryByIdCommandHandler(
        ICategoryRepository categoryRepository,
        IUserAccessor userAccessor)
    {
        _categoryRepository = categoryRepository;
        _userAccessor = userAccessor;
    }       

    public async Task<Unit> Handle(RestoreDeletedCategoryByIdCommand request, CancellationToken cancellationToken)
    {
        // BUISNESS RULE:
        // Only user can restore their deleted categories
        
        var userId = _userAccessor.UserId;

        // fetch deleted Category by id
        var deletedCategory = await _categoryRepository.GetDeletedCategoryAsync(request.Id, userId, cancellationToken);
        if (deletedCategory == null)
            throw new NotFoundException(nameof(Category), request.Id);

        // restore and save
        deletedCategory.IsDeleted = false;
        deletedCategory.DeletedAt = null;
        deletedCategory.DeletedBy = null;
        var restored = await _categoryRepository.RestoreDeletedCategoryAsync();
        if (!restored)
            throw new BadRequestException("restore failed!");
        return Unit.Value;

    }
}
