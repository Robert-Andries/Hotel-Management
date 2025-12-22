using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Application.Bookings.GetBooking;
using HM.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Bookings.GetBookingsByUser;

internal sealed class GetBookingsByUserQueryHandler : IQueryHandler<GetBookingsByUserQuery, Result<List<BookingResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetBookingsByUserQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BookingResponse>>> Handle(GetBookingsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var bookings = await _context.Bookings
            .Where(b => b.UserId == request.UserId)
            .Select(b => new BookingResponse(
                b.Id,
                b.UserId,
                b.RoomId,
                b.Status,
                b.Price.Amount,
                b.Price.Currency.Code,
                b.Duration.Start,
                b.Duration.End,
                b.ReservedOnUtc))
            .ToListAsync(cancellationToken);

        return Result.Success(bookings);
    }
}
