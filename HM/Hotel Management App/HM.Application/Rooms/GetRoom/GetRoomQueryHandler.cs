using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Rooms.GetRoom;

public class GetRoomQueryHandler : IQueryHandler<GetRoomQuery, Result<RoomResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetRoomQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RoomResponse>> Handle(GetRoomQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == request.RoomId, cancellationToken);
        if (rooms is null)
            return Result.Failure<RoomResponse>(RoomErrors.NotFound);

        var roomResponse = new RoomResponse(rooms);

        return Result.Success(roomResponse);
    }
}