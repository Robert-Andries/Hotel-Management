using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Rooms.GetAllRooms;

public class GetAllRoomsQueryHandler : IQueryHandler<GetAllRoomsQuery, Result<List<RoomResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllRoomsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<RoomResponse>>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _context.Rooms.ToListAsync(cancellationToken);
        var roomsResponse =  new List<RoomResponse>();
        foreach(var room in rooms)
            roomsResponse.Add(new RoomResponse(room));
        
        return Result.Success(roomsResponse);
    }
}