using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Entities;
using HM.Domain.Rooms.Abstractions;

namespace HM.Application.Reviews.AddReview;

internal sealed class AddReviewCommandHandler : ICommandHandler<AddReviewCommand, Result>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly ITime _time;
    private readonly IUnitOfWork _unitOfWork;

    public AddReviewCommandHandler(ITime time, IReviewRepository reviewRepository, IRoomRepository roomRepository,
        IUnitOfWork unitOfWork)
    {
        _time = time;
        _reviewRepository = reviewRepository;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        var review = RoomReview.Create(
            request.RoomId,
            request.UserId,
            request.Comment,
            request.Rating,
            _time.NowUtc);

        var roomResult = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (roomResult.IsFailure)
            return Result.Failure<Guid>(roomResult.Error);

        _reviewRepository.AddReview(review, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}