using HM.Domain.Abstractions;
using HM.Domain.Users.Abstractions;
using HM.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace HM.Infrastructure.Repositories;

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
            return Result.Failure<User>(new Error("User.NotFound",
                "The user with the specified identifier was not found."));

        return Result.Success(user);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }
}