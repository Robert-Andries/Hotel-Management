using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Services;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Bookings.GetBookingPreview;

internal sealed class
    GetBookingPreviewQueryHandler : IQueryHandler<GetBookingPreviewQuery, Result<BookingPreviewResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPricingService _pricingService;

    public GetBookingPreviewQueryHandler(
        IApplicationDbContext context,
        IPricingService pricingService)
    {
        _context = context;
        _pricingService = pricingService;
    }

    public async Task<Result<BookingPreviewResponse>> Handle(GetBookingPreviewQuery request,
        CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);

        if (room is null) return Result.Failure<BookingPreviewResponse>(RoomErrors.NotFound);

        var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
        if (dateRangeResult.IsFailure) return Result.Failure<BookingPreviewResponse>(dateRangeResult.Error);

        var dateRange = dateRangeResult.Value;

        var pricingDetails = _pricingService.CalculatePrice(room, dateRange);

        return Result.Success(new BookingPreviewResponse(
            pricingDetails.PriceForPeriod,
            pricingDetails.AmenitiesUpCharge,
            pricingDetails.TotalPrice,
            pricingDetails.TotalPrice.Currency.Code));
    }
}