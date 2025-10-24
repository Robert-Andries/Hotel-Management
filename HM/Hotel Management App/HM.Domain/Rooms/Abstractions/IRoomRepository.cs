using HM.Domain.Abstractions;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Domain.Rooms.Abstractions;

public interface IRoomRepository
{
    Task<Result<Room>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Room>> GetByLocationAsync(RoomLocation location, CancellationToken cancellationToken = default);
    Task<Result<List<Room>>> GetAllAsync(RoomLocation location, CancellationToken cancellationToken = default);
    Task<Result<List<Room>>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<Result> UpdateRoom(Guid Id ,Room room, CancellationToken cancellationToken = default);
}