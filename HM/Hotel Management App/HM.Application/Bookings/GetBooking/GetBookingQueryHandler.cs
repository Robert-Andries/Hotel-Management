using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Bookings.GetBooking;

internal sealed class GetBookingQueryHandler : IQueryHandler<GetBookingQuery, Result<BookingResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetBookingQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BookingResponse>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .Where(b => b.Id == request.BookingId)
            .Select(b => new BookingResponse(
                b.Id,
                b.UserId,
                b.RoomId,
                (int)b.Status,
                b.Price.Amount,
                b.Price.Currency.Code,
                b.Duration.Start,
                b.Duration.End,
                b.ReservedOnUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (booking is null)
        {
            return Result.Failure<BookingResponse>(BookingErrors.NotFound);
        }

        return Result.Success(booking);
    }
}
