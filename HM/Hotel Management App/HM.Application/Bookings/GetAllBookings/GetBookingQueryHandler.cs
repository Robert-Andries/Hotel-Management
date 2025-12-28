using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Application.Bookings.Shared;
using HM.Application.Users.GetUsers;
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
        var query = from b in _context.Bookings
            join u in _context.Users on b.UserId equals u.Id
            join r in _context.Rooms on b.RoomId equals r.Id
            select new { Booking = b, User = u, Room = r };

        if (!request.SeeCompletedBookings)
        {
            query = query.Where(x => x.Booking.Status != BookingStatus.Completed
                                     && x.Booking.Status != BookingStatus.Cancelled);
        }

        var bookings = await query
            .Select(x => new BookingResponse(
                x.Booking.Id,
                new UserResponse(x.User),
                x.Booking.RoomId,
                $"floor {x.Room.Location.Floor} roomnumber {x.Room.Location.RoomNumber}",
                x.Booking.Status,
                x.Booking.Price.Amount,
                x.Booking.Price.Currency.Code,
                x.Booking.Duration.Start,
                x.Booking.Duration.End,
                x.Booking.ReservedOnUtc))
            .ToListAsync(cancellationToken);

        return Result.Success(bookings);
    }
}