using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Application.Users.GetUsers;
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
        var booking = await (from b in _context.Bookings
                join u in _context.Users on b.UserId equals u.Id
                join r in _context.Rooms on b.RoomId equals r.Id
                where b.Id == request.BookingId
                select new BookingResponse(
                    b.Id,
                    new UserResponse(u),
                    b.RoomId,
                    $"floor {r.Location.Floor} roomnumber {r.Location.RoomNumber}",
                    b.Status,
                    b.Price.Amount,
                    b.Price.Currency.Code,
                    b.Duration.Start,
                    b.Duration.End,
                    b.ReservedOnUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (booking is null) return Result.Failure<BookingResponse>(BookingErrors.NotFound);

        return Result.Success(booking);
    }
}