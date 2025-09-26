using HM.Domain.Booking.Value_Objects;

namespace HM.Domain.Booking.Abstractions;

public interface IBookingRepository
{
    Task<Entities.Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> IsOverlappingAsync(
        Room.Entities.Room room,
        DateRange duration,
        CancellationToken cancellationToken = default);

    void Add(Entities.Booking booking);
}