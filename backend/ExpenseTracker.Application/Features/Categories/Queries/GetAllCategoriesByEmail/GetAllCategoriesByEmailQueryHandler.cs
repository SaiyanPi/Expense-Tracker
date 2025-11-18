using AutoMapper;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Application.DTOs.Category;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using MediatR;

namespace ExpenseTracker.Application.Features.Categories.Queries.GetAllCategoriesByEmail;

public class GetAllCategoriesByEmailQueryHandler : IRequestHandler<GetAllCategoriesByEmailQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllCategoriesByEmailQueryHandler(ICategoryRepository categoryRepository, 
        IUserRepository userRepository, 
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.Email);

        var categories = await _categoryRepository.GetAllCategoriesByEmailAsync(user.Id, cancellationToken);
        return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
    }
}