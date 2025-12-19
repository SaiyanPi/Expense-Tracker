using AutoMapper;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    public UserRepository(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    // USER MANAGEMENT

    // Get All Users
    // --------------
    public async Task<(IReadOnlyList<User> Users, int TotalCount)> GetAllAsync(
        int skip,
        int take,
        string? sortBy = null,
        bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query
            .CountAsync(cancellationToken);

        query = query.ApplySorting(sortBy, sortDesc);
        
        var appUsers = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        var users = _mapper.Map<IReadOnlyList<User>>(appUsers);
       
        return (users, totalCount);
    }


    // Get User By Id
    // ---------------
    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(id);
        return appUser is null ? null : _mapper.Map<User>(appUser);
    }


    // Get User By Email
    // -------------------
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        return appUser is null ? null : _mapper.Map<User>(appUser);
    }


    // Delete User
    // -------------
    public async Task<bool> DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id);
        if (appUser is null) return false;

        var result = await _userManager.DeleteAsync(appUser);
        return result.Succeeded;
    
    }

}
