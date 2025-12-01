using HM.Domain.Abstractions;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Domain.Rooms.Abstractions;

public interface IRoomRepository
{
    Task<Result> AddAsync(Room room, CancellationToken cancellationToken = default);
    
    Task<Result<Room>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<Room>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result> UpdateRoomAsync(Guid id, Room room, CancellationToken cancellationToken = default);
    
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}