using HM.Domain.Abstractions;
using HM.Domain.Rooms.Entities;

namespace HM.Domain.Rooms.Abstractions;

/// <summary>
///     Defines the contract for Room persistence operations.
/// </summary>
public interface IRoomRepository
{
    /// <summary>Adds a new room to the repository.</summary>
    Task<Result> AddAsync(Room room, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a room by its ID.</summary>
    Task<Result<Room>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary> Retrieves all rooms in the system.</summary>
    Task<Result<List<Room>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Updates an existing room's details.</summary>
    Task<Result> UpdateRoomAsync(Guid id, Room room, CancellationToken cancellationToken = default);

    /// <summary>Deletes a room by its ID.</summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}