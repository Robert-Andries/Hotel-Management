using HM.Domain.Abstractions;
using HM.Domain.Users.Entities;

namespace HM.Domain.Users.Abstractions;

public interface IUserRepository
{
    Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}