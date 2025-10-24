using HM.Application.Abstractions.Messaging;
using HM.Application.Rooms.UpdateRating;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Entities;
using HM.Domain.Rooms;
using HM.Domain.Rooms.Abstractions;
using MediatR;

namespace HM.Application.Reviews.AddReview;

internal sealed class AddReviewCommandHandler : ICommandHandler<AddReviewCommand, Result>
{
    private readonly ITime _time;
    private readonly IReviewRepository _reviewRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMediator _mediator;

    public AddReviewCommandHandler(ITime time, IReviewRepository reviewRepository, IRoomRepository roomRepository, IMediator mediator)
    {
        _time = time;
        _reviewRepository = reviewRepository;
        _roomRepository = roomRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        RoomReview newReview = new(
            id: Guid.Empty,
            roomId: request.RoomId,
            userId: request.UserId,
            comment: request.Comment,
            rating: request.Rating,
            createdAtUtc: _time.NowUtc);
        
        var result = await _reviewRepository.AddReview(newReview, cancellationToken);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);
        
        var roomResult = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (roomResult.IsFailure)
            return Result.Failure<Guid>(roomResult.Error);
        
        var ratingUpdateResult = await roomResult.Value.Rating.Update(_reviewRepository, cancellationToken);
        if(ratingUpdateResult.IsFailure)
            return Result.Failure<Guid>(ratingUpdateResult.Error);
        var ratingSummary = ratingUpdateResult.Value;

        var updateResult = await _mediator.Send(new UpdateRatingCommand(roomResult.Value.Id, ratingSummary), cancellationToken);

        return updateResult;
    }
}