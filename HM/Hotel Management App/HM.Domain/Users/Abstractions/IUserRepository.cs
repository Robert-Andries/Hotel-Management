using HM.Domain.Abstractions;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;

namespace HM.Domain.Users.Abstractions;

public interface IUserRepository
{
    Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default);
    void Add(User user);
}