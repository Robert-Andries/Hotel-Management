using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Value_Objects;

namespace HM.Application.Rooms.ChangeStatus;

internal sealed class ChangeStatusCommandHandler : ICommandHandler<ChangeStatusCommand, Result>
{
    private readonly IRoomRepository _roomRepository;
    private readonly ITime _time;

    public ChangeStatusCommandHandler(IRoomRepository roomRepository, ITime time)
    {
        _roomRepository = roomRepository;
        _time = time;
    }

    public async Task<Result> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
    {
        var roomResponse = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (roomResponse.IsFailure)
            return Result.Failure(roomResponse.Error);
        
        var room = roomResponse.Value;
        room.Status = request.Status;
        if (room.Status == RoomStatus.Reserved)
            room.LastBookedOnUtc = _time.NowUtc;
        
        var updateResponse = await _roomRepository.UpdateRoom(room.Id, room, cancellationToken);

        return updateResponse;
    }
}