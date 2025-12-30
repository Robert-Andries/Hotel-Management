namespace HM.Domain.Abstractions;

/// <summary>
///     Defines the Unit of Work pattern for transaction management.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    ///     Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}