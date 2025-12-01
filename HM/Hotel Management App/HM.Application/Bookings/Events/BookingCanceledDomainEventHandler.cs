using HM.Domain.Abstractions;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Events;
using HM.Domain.Rooms.Abstractions;
using MediatR;

using Microsoft.Extensions.Logging;

namespace HM.Application.Bookings.Events;

internal sealed class BookingCanceledDomainEventHandler : INotificationHandler<BookingCanceledDomainEvent>
{
    private readonly IRoomRepository  _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITime _time;
    private readonly ILogger<BookingCanceledDomainEventHandler> _logger;

    public BookingCanceledDomainEventHandler(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ITime time,
        ILogger<BookingCanceledDomainEventHandler> logger)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _time = time;
        _logger = logger;
    }

    public async Task Handle(BookingCanceledDomainEvent notification, CancellationToken cancellationToken)
    {
        var bookingResult = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);
        if(bookingResult.IsFailure)
        {
            _logger.LogError("Booking with ID {BookingId} was not found", notification.BookingId);
            return;
        }
        var booking = bookingResult.Value;
        
        var roomResult = await _roomRepository.GetByIdAsync(booking.RoomId, cancellationToken);
        if(roomResult.IsFailure)
        {
            _logger.LogError("Room with ID {RoomId} was not found", booking.RoomId);
            return;
        }
        var room = roomResult.Value;
        
        var result = room.ResetStatus();
        if(result.IsFailure)
        {
            _logger.LogError("Failed to reset status for room {RoomId}: {Error}", room.Id, result.Error);
            return;
        }
        
        await _roomRepository.UpdateRoomAsync(room.Id, room, cancellationToken);
    }
}