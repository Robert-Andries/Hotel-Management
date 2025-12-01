using HM.Domain.Abstractions;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Events;
using HM.Domain.Rooms.Abstractions;
using MediatR;

using Microsoft.Extensions.Logging;

namespace HM.Application.Reviews.Events;

internal sealed class ReviewCreatedDomainEventHandler : INotificationHandler<ReviewCreatedDomainEvent>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewCreatedDomainEventHandler> _logger;

    public ReviewCreatedDomainEventHandler(
        IRoomRepository roomRepository,
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReviewCreatedDomainEventHandler> logger)
    {
        _roomRepository = roomRepository;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ReviewCreatedDomainEvent notification, CancellationToken cancellationToken)
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
            _logger.LogError("Failed to calculate new rating for room {RoomId}: {Error}", room.Id, ratingUpdateResult.Error);
            return;
        }
        var ratingUpdate = ratingUpdateResult.Value;

        var result = room.UpdateRating(ratingUpdate);
        if (result.IsFailure)
        {
            _logger.LogError("Failed to update rating for room {RoomId}: {Error}", room.Id, result.Error);
            return;
        }

        await _roomRepository.UpdateRoomAsync(room.Id, room, cancellationToken);
        
        // No need to call SaveChangesAsync here, as it's handled by the main transaction
    }
}
