using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Application.Bookings.GetBooking;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Value_Objects;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Bookings.GetAllBookings;

internal sealed class GetAllBookingsQueryHandler : IQueryHandler<GetAllBookingsQuery, Result<List<BookingResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllBookingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BookingResponse>>> Handle(GetAllBookingsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Bookings.AsQueryable();

        if (!request.SeeCompletedBookings)
        {
            query = query.Where(b => b.Status != BookingStatus.Completed 
                                     && b.Status != BookingStatus.Cancelled);
        }

        var bookings = await query
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