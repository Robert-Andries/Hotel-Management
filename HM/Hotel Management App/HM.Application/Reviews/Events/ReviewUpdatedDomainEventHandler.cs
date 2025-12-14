using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Events;
using HM.Domain.Rooms.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HM.Application.Reviews.Events;

internal sealed class ReviewUpdatedDomainEventHandler : INotificationHandler<ReviewUpdatedDomainEvent>
{
    private readonly ILogger<ReviewUpdatedDomainEventHandler> _logger;
    private readonly IReviewRepository _reviewRepository;
    private readonly IRoomRepository _roomRepository;

    public ReviewUpdatedDomainEventHandler(
        IRoomRepository roomRepository,
        IReviewRepository reviewRepository,
        ILogger<ReviewUpdatedDomainEventHandler> logger)
    {
        _roomRepository = roomRepository;
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task Handle(ReviewUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var roomResult = await _roomRepository.GetByIdAsync(notification.RoomId, cancellationToken);
        if (roomResult.IsFailure)
        {
            _logger.LogError("Room with ID {RoomId} was not found", notification.RoomId);
            return;
        }

        var room = roomResult.Value;

        var ratingUpdateResult = await room.Rating.Update(_reviewRepository, cancellationToken);
        if (ratingUpdateResult.IsFailure)
        {
            _logger.LogError("Failed to calculate new rating for room {RoomId}: {Error}", room.Id,
                ratingUpdateResult.Error);
            return;
        }

        var result = room.UpdateRating(ratingUpdateResult.Value);
        if (result.IsFailure)
        {
            _logger.LogError("Failed to update rating for room {RoomId}: {Error}", room.Id, result.Error);
            return;
        }

        await _roomRepository.UpdateRoomAsync(room.Id, room, cancellationToken);

        // No need to call SaveChangesAsync here, as it's handled by the main transaction
    }
}