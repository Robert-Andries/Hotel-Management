using HM.Domain.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;

namespace HM.Domain.Bookings.Abstractions;

/// <summary>
///     Defines the contract for Booking persistence operations.
/// </summary>
public interface IBookingRepository
{
    /// <summary>Adds a new booking to the repository.</summary>
    Task<Result> AddAsync(Booking booking, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a booking by ID.</summary>
    Task<Result<Booking>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all bookings.</summary>
    Task<Result<List<Booking>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Updates an existing booking.</summary>
    Task<Result> Update(Guid bookingId, Booking booking, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks if a room overlaps on the specified duration.
    /// </summary>
    /// <param name="room">The room to check.</param>
    /// <param name="duration">The date range to verify.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if an overlap exists, False otherwise.</returns>
    Task<Result<bool>> IsOverlappingAsync(
        Room room,
        DateRange duration,
        CancellationToken cancellationToken = default);
}