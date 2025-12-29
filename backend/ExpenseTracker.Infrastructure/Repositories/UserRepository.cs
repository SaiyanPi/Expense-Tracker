using AutoMapper;
using ExpenseTracker.Application.Common.Pagination;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces.Repositories;
using ExpenseTracker.Persistence;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Twilio.Types;

namespace ExpenseTracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ExpenseTrackerDbContext _dbContext;
    private readonly IMapper _mapper;
    public UserRepository(UserManager<ApplicationUser> userManager,
        ExpenseTrackerDbContext dbContext,
        IMapper mapper)
    {
        _userManager = userManager;
        _dbContext = dbContext;
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

    // Get User by refreshToken
    // ------------------------
    public async Task<string?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await _dbContext.UserTokens
            .Where(t =>
                t.LoginProvider == "ExpenseTracker" &&
                t.Name == "RefreshToken" &&
                t.Value == refreshToken)
            .Select(t => t.UserId)
            .FirstOrDefaultAsync(cancellationToken);
        
        return token;
    }

}
