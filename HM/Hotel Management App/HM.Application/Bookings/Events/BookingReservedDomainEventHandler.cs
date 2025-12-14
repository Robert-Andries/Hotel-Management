using HM.Domain.Abstractions;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Events;
using HM.Domain.Rooms.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HM.Application.Bookings.Events;

internal sealed class BookingReservedDomainEventHandler : INotificationHandler<BookingReservedDomainEvent>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingReservedDomainEventHandler> _logger;
    private readonly IRoomRepository _roomRepository;
    private readonly ITime _time;

    public BookingReservedDomainEventHandler(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        ITime time,
        ILogger<BookingReservedDomainEventHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _time = time;
        _logger = logger;
    }

    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        var bookingResult = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        if (bookingResult.IsFailure)
        {
            _logger.LogError("Booking with ID {BookingId} was not found", notification.BookingId);
            return;
        }

        var booking = bookingResult.Value;

        var roomResult = await _roomRepository.GetByIdAsync(booking.RoomId, cancellationToken);
        if (roomResult.IsFailure)
        {
            _logger.LogError("Room with ID {RoomId} was not found", booking.RoomId);
            return;
        }

        var room = roomResult.Value;

        var result = room.Reserve(_time.NowUtc);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to reserve room {RoomId}: {Error}", room.Id, result.Error);
            return;
        }

        await _roomRepository.UpdateRoomAsync(room.Id, room, cancellationToken);
    }
}