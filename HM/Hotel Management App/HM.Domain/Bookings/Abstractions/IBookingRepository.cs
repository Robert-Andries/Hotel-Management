using HM.Domain.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Entities;

namespace HM.Domain.Bookings.Abstractions;

public interface IBookingRepository
{
    void Add(Booking booking);
    Task<Result<Booking>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<Booking>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result> Update(Guid bookingId, Booking booking, CancellationToken cancellationToken = default);

    Task<bool> IsOverlappingAsync(
        Room room,
        DateRange duration,
        CancellationToken cancellationToken = default);
}