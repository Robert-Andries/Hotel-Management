using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Services;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Value_Objects;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Rooms.GetAvailableRooms;

internal sealed class
    GetAvailableRoomsQueryHandler : IQueryHandler<GetAvailableRoomsQuery, Result<List<AvailableRoomResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPricingService _pricingService;

    public GetAvailableRoomsQueryHandler(IApplicationDbContext context, IPricingService pricingService)
    {
        _context = context;
        _pricingService = pricingService;
    }

    public async Task<Result<List<AvailableRoomResponse>>> Handle(GetAvailableRoomsQuery request,
        CancellationToken cancellationToken)
    {
        var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
        if (dateRangeResult.IsFailure) return Result.Failure<List<AvailableRoomResponse>>(dateRangeResult.Error);

        var dateRange = dateRangeResult.Value;

        var overlappingBookingRoomIds = _context.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled &&
                        b.Duration.Start <= dateRange.End &&
                        b.Duration.End >= dateRange.Start)
            .Select(b => b.RoomId);

        var availableRooms = await _context.Rooms
            .Where(r => r.Status == RoomStatus.Available &&
                        !overlappingBookingRoomIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        var response = availableRooms.Select(room =>
        {
            var pricingDetails = _pricingService.CalculatePrice(room, dateRange);
            return new AvailableRoomResponse(room, pricingDetails.TotalPrice);
        }).ToList();

        return Result.Success(response);
    }
}