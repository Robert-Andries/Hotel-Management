using HM.Domain.Abstractions;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using Microsoft.EntityFrameworkCore;

namespace HM.Infrastructure.Repositories;

internal sealed class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoomRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Room>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (room is null)
            return Result.Failure<Room>(new Error("Room.NotFound",
                "The room with the specified identifier was not found."));

        return Result.Success(room);
    }

    public async Task<Result<Room>> GetByLocationAsync(RoomLocation location,
        CancellationToken cancellationToken = default)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Location == location, cancellationToken);

        if (room is null)
            return Result.Failure<Room>(new Error("Room.NotFound",
                "The room with the specified location was not found."));

        return Result.Success(room);
    }

    public async Task<Result<List<Room>>> GetAllAsync(RoomLocation location,
        CancellationToken cancellationToken = default)
    {
        var rooms = await _dbContext.Rooms
            .Where(r => r.Location == location)
            .ToListAsync(cancellationToken);

        return Result.Success(rooms);
    }

    public async Task<Result<List<Room>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await _dbContext.Rooms.ToListAsync(cancellationToken);
        return Result.Success(rooms);
    }

    public Task<Result> UpdateRoom(Guid id, Room room, CancellationToken cancellationToken = default)
    {
        _dbContext.Rooms.Update(room);
        return Task.FromResult(Result.Success());
    }
}