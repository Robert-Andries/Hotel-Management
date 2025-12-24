using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Application.Bookings.GetBooking;
using HM.Application.Users.GetUsers;
using HM.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Bookings.GetBookingsByUser;

internal sealed class
    GetBookingsByUserQueryHandler : IQueryHandler<GetBookingsByUserQuery, Result<List<BookingResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetBookingsByUserQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BookingResponse>>> Handle(GetBookingsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var bookings = await (from b in _context.Bookings
                join u in _context.Users on b.UserId equals u.Id
                join r in _context.Rooms on b.RoomId equals r.Id
                where b.UserId == request.UserId
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
            .ToListAsync(cancellationToken);

        return Result.Success(bookings);
    }
}