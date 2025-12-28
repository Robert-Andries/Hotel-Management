using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Services;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Value_Objects;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Rooms.GetAvailableRooms;

internal sealed class
    GetAvailableRoomsQueryHandler : IQueryHandler<GetAvailableRoomsQuery, Result<List<RoomSearchResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPricingService _pricingService;

    public GetAvailableRoomsQueryHandler(
        IApplicationDbContext context,
        IPricingService pricingService)
    {
        _context = context;
        _pricingService = pricingService;
    }

    public async Task<Result<List<RoomSearchResponse>>> Handle(GetAvailableRoomsQuery request,
        CancellationToken cancellationToken)
    {
        var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
        if (dateRangeResult.IsFailure) return Result.Failure<List<RoomSearchResponse>>(dateRangeResult.Error);

        var dateRange = dateRangeResult.Value;

        var overlappingBookingRoomIds = _context.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled &&
                        b.Duration.Start <= dateRange.End &&
                        b.Duration.End >= dateRange.Start)
            .Select(b => b.RoomId);

        var potentialRooms = await _context.Rooms
            .Where(r => r.Status != RoomStatus.Maintanance &&
                        !overlappingBookingRoomIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        var matchingRooms = potentialRooms
            .Where(r => request.RequiredFeatures.All(f => r.Features.Contains(f)))
            .ToList();

        var response = new List<RoomSearchResponse>();

        foreach (var room in matchingRooms)
        {
            var priceDetails = _pricingService.CalculatePrice(room, dateRange);

            response.Add(new RoomSearchResponse(
                room.Id,
                room.RoomType,
                room.Price,
                priceDetails.TotalPrice,
                room.Features,
                priceDetails.TotalPrice.Currency.Code,
                room.Location));
        }

        return Result.Success(response);
    }
}