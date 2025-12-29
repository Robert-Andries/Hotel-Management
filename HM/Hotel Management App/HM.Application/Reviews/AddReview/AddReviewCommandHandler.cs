using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Reviews;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Entities;
using HM.Domain.Rooms;
using HM.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Reviews.AddReview;

internal sealed class AddReviewCommandHandler : ICommandHandler<AddReviewCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IReviewRepository _reviewRepository;
    private readonly ITime _time;
    private readonly IUnitOfWork _unitOfWork;

    public AddReviewCommandHandler(ITime time, IApplicationDbContext context, IUnitOfWork unitOfWork,
        IReviewRepository reviewRepository)
    {
        _time = time;
        _context = context;
        _unitOfWork = unitOfWork;
        _reviewRepository = reviewRepository;
    }

    public async Task<Result> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure(UserErrors.NotFound);

        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);
        if (room == null)
            return Result.Failure(RoomErrors.NotFound);

        var lastBookings = await _context.Bookings.OrderByDescending(b => b.Duration.End)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastBookings == null)
            return Result.Failure(ReviewErrors.NotBeenInRoom);

        if (lastBookings.Status != BookingStatus.Completed)
            return Result.Failure(ReviewErrors.BookingStatusNeedsToBeCompleted);

        var review = RoomReview.Create(
            request.RoomId,
            request.UserId,
            request.Comment,
            request.Rating,
            _time.NowUtc);

        _reviewRepository.AddReview(review, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}