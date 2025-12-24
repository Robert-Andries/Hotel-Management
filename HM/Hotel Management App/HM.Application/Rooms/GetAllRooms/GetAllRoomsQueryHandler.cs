using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Application.Rooms.GetRoom;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Services;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Rooms.GetAllRooms;

public class GetAllRoomsQueryHandler : IQueryHandler<GetAllRoomsQuery, Result<List<RoomResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPricingService _pricingService;
    private readonly ITime _time;

    public GetAllRoomsQueryHandler(IApplicationDbContext context, IPricingService pricingService, ITime time)
    {
        _context = context;
        _pricingService = pricingService;
        _time = time;
    }

    public async Task<Result<List<RoomResponse>>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _context.Rooms.ToListAsync(cancellationToken);
        var roomsResponse = new List<RoomResponse>();

        foreach (var room in rooms)
            roomsResponse.Add(new RoomResponse(room));

        return Result.Success(roomsResponse);
    }
}