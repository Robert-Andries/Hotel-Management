using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Entities;

namespace HM.Application.Reviews.UpdateReview;

internal sealed class UpdateReviewCommandHandler : ICommandHandler<UpdateReviewCommand, Result>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITime _time;

    public UpdateReviewCommandHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork, ITime time)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _time = time;
    }

    public async Task<Result> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var reviewResult = await _reviewRepository.GetReview(request.ReviewId, cancellationToken);

        if (reviewResult.IsFailure)
        {
            return Result.Failure(reviewResult.Error);
        }

        var review = reviewResult.Value;

        review.Update(request.Rating, request.Comment, _time.NowUtc);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
