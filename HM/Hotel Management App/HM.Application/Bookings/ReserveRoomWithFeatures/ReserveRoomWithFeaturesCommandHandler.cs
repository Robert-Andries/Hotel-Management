using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Bookings.Services;
using HM.Domain.Rooms;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Domain.Users.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Bookings.ReserveRoomWithFeatures;

internal sealed class ReserveRoomWithFeaturesCommandHandler : ICommandHandler<ReserveRoomWithFeaturesCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITime _time;
    private readonly IPricingService _pricingService;

    public ReserveRoomWithFeaturesCommandHandler(
        IApplicationDbContext context,
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITime time,
        IPricingService pricingService)
    {
        _context = context;
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _time = time;
        _pricingService = pricingService;
    }

    public async Task<Result<Guid>> Handle(ReserveRoomWithFeaturesCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }
        var user = userResult.Value;

        var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
        if (dateRangeResult.IsFailure)
        {
            return Result.Failure<Guid>(dateRangeResult.Error);
        }
        var dateRange = dateRangeResult.Value;

        
        var overlappingBookingRoomIds = _context.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled &&
                        b.Duration.Start <= dateRange.End &&
                        b.Duration.End >= dateRange.Start)
            .Select(b => b.RoomId);
        
        var potentialRooms = await _context.Rooms
            .Where(r => r.Status == RoomStatus.Available &&
                        !overlappingBookingRoomIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        var bestRoom = potentialRooms
            .Where(r => request.RequiredFeatures.All(f => r.Features.Contains(f)))
            .OrderBy(r => r.Price.Amount)
            .FirstOrDefault();

        if (bestRoom is null)
        {
            return Result.Failure<Guid>(RoomErrors.NotFound);
        }
        
        var priceDetails = _pricingService.CalculatePrice(bestRoom, dateRange);

        var booking = Booking.Reserve(
            bestRoom.Id,
            user.Id,
            dateRange,
            _time.NowUtc,
            priceDetails.TotalPrice);

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(booking.Id);
    }
}
