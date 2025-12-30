using HM.Domain.Abstractions;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;

namespace HM.Domain.Users.Abstractions;

/// <summary>
///     Defines the contract for User persistence operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    ///     Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique GUID of the user.</param>
    /// <returns>A Result containing the User if found, or Failure if not.</returns>
    Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email value object to search for.</param>
    /// <returns>A Result containing the User if found.</returns>
    Task<Result<User>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if an email address is already registered in the system.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <returns>True if the email is unique (does not exist), False otherwise.</returns>
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    void Add(User user);
}