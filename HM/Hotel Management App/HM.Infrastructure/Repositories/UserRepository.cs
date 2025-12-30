using HM.Domain.Abstractions;
using HM.Domain.Users;
using HM.Domain.Users.Abstractions;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;
using Microsoft.EntityFrameworkCore;

namespace HM.Infrastructure.Repositories;

/// <summary>
///     Repository for managing user entities.
/// </summary>
internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
            return Result.Failure<User>(UserErrors.NotFound);

        return Result.Success(user);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default)
    {
        return !await _dbContext.Users.AnyAsync(u => u.Contact.Email.Value == email.Value, cancellationToken);
    }

    public async Task<Result<User>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Contact.Email.Value == email.Value && u.Contact.Email.Domain == email.Domain, cancellationToken);

        if (user is null)
            return Result.Failure<User>(UserErrors.NotFound);

        return Result.Success(user);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }
}