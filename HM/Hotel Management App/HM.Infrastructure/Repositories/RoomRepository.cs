using HM.Domain.Abstractions;
using HM.Domain.Rooms;
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

    public async Task<Result> AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Rooms.AddAsync(room, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Error.OperationCanceled);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<Room>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Room? room;
        try
        {
            room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<Room>(Error.OperationCanceled);
        }
        if (room is null)
            return Result.Failure<Room>(RoomErrors.NotFound);
        
        return Result.Success(room);
    }
    
    public async Task<Result<List<Room>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Room> rooms;
        try
        {
            rooms = await _dbContext.Rooms.ToListAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<List<Room>>(Error.OperationCanceled);
        }
        return Result.Success(rooms);
    }

    public async Task<Result> UpdateRoomAsync(Guid id, Room room, CancellationToken cancellationToken = default)
    {
        _dbContext.Rooms.Update(room);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        int rowsDeleted = 0;
        try
        {
            rowsDeleted = await _dbContext.Rooms
                .Where(room => room.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(Error.OperationCanceled);
        }
        if (rowsDeleted == 0)
            return Result.Failure(RoomErrors.NotFound);

        return Result.Success();
    }
}