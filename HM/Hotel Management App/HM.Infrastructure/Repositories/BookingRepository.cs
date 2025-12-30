using HM.Domain.Abstractions;
using HM.Domain.Bookings;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;
using Microsoft.EntityFrameworkCore;

namespace HM.Infrastructure.Repositories;

/// <summary>
///     Repository for managing booking entities.
/// </summary>
internal sealed class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BookingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        _dbContext.Bookings.Add(booking);
        return Result.Success();
    }

    public async Task<Result<Booking>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (booking is null) return Result.Failure<Booking>(BookingErrors.NotFound);

        return Result.Success(booking);
    }

    public async Task<Result<List<Booking>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await _dbContext.Bookings.ToListAsync(cancellationToken);
        return Result.Success(bookings);
    }

    public Task<Result> Update(Guid bookingId, Booking booking, CancellationToken cancellationToken = default)
    {
        // EF Core tracks changes, so explicit update might not be needed if the entity is attached.
        // However, for clarity or disconnected scenarios:
        _dbContext.Bookings.Update(booking);
        return Task.FromResult(Result.Success());
    }

    public async Task<Result<bool>> IsOverlappingAsync(Room room, DateRange range,
        CancellationToken cancellationToken = default)
    {
        bool isOverlapping;
        try
        {
            isOverlapping = await _dbContext.Bookings
                .AnyAsync(
                    b =>
                        b.RoomId == room.Id &&
                        b.Duration.Start <= range.End &&
                        b.Duration.End >= range.Start &&
                        b.Status != BookingStatus.Cancelled,
                    cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<bool>(Error.OperationCanceled);
        }

        return Result.Success(isOverlapping);
    }
}