using HM.Domain.Abstractions;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Events;
using HM.Domain.Rooms.Abstractions;
using MediatR;

using Microsoft.Extensions.Logging;

namespace HM.Application.Bookings.Events;

internal sealed class BookingCheckedInDomainEventHandler : INotificationHandler<BookingCheckedInDomainEvent>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookingCheckedInDomainEventHandler> _logger;

    public BookingCheckedInDomainEventHandler(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IUnitOfWork unitOfWork,
        ILogger<BookingCheckedInDomainEventHandler> logger)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(BookingCheckedInDomainEvent notification, CancellationToken cancellationToken)
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
        
        var result = room.Occupy();
        if (result.IsFailure)
        {
            _logger.LogError("Failed to occupy room {RoomId}: {Error}", room.Id, result.Error);
            return;
        }
        
        await _roomRepository.UpdateRoom(room.Id, room, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}