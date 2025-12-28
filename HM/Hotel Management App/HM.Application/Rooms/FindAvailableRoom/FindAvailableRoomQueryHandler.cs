using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Application.Rooms.GetAvailableRooms;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Services;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms;
using HM.Domain.Rooms.Value_Objects;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Rooms.FindAvailableRoom;

internal sealed class FindAvailableRoomQueryHandler : IQueryHandler<FindAvailableRoomQuery, Result<RoomSearchResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPricingService _pricingService;

    public FindAvailableRoomQueryHandler(
        IApplicationDbContext context,
        IPricingService pricingService)
    {
        _context = context;
        _pricingService = pricingService;
    }

    public async Task<Result<RoomSearchResponse>> Handle(FindAvailableRoomQuery request,
        CancellationToken cancellationToken)
    {
        var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
        if (dateRangeResult.IsFailure) return Result.Failure<RoomSearchResponse>(dateRangeResult.Error);

        var dateRange = dateRangeResult.Value;

        var overlappingBookingRoomIds = _context.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled &&
                        b.Duration.Start <= dateRange.End &&
                        b.Duration.End >= dateRange.Start)
            .Select(b => b.RoomId);

        var potentialRooms = await _context.Rooms
            .Where(r => r.Status != RoomStatus.Maintanance &&
                        r.RoomType == request.RoomType &&
                        !overlappingBookingRoomIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        var bestRoom = potentialRooms
            .Where(r => request.RequiredFeatures.All(f => r.Features.Contains(f)))
            .OrderBy(r => r.Price.Amount)
            .FirstOrDefault();

        if (bestRoom is null) return Result.Failure<RoomSearchResponse>(RoomErrors.NoneAvailable);

        var priceDetails = _pricingService.CalculatePrice(bestRoom, dateRange);

        return Result.Success(new RoomSearchResponse(
            bestRoom.Id,
            bestRoom.RoomType,
            bestRoom.Price,
            priceDetails.TotalPrice,
            bestRoom.Features,
            priceDetails.TotalPrice.Currency.Code,
            bestRoom.Location));
    }
}