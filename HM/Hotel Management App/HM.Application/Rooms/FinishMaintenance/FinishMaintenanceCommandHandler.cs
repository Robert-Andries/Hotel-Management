using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Value_Objects;
using Microsoft.Extensions.Logging;

namespace HM.Application.Rooms.FinishMaintenance;

public class FinishMaintenanceCommandHandler : ICommandHandler<FinishMaintenanceCommand, Result>
{
    private readonly ILogger<FinishMaintenanceCommandHandler> _logger;
    private readonly IRoomRepository _roomRepository;

    public FinishMaintenanceCommandHandler(ILogger<FinishMaintenanceCommandHandler> logger,
        IRoomRepository roomRepository)
    {
        _logger = logger;
        _roomRepository = roomRepository;
    }

    public async Task<Result> Handle(FinishMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var roomResult = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (roomResult.IsFailure)
        {
            _logger.LogError("There was an error trying to get the room with Id: {Id}, Error: {error.code} {error.Name}"
                , request.RoomId, roomResult.Error.Code, roomResult.Error.Name);
            return roomResult;
        }

        var room = roomResult.Value;

        if (room.Status != RoomStatus.Maintanance)
        {
            _logger.LogError(
                "The room with Id: {Id} is already in the maintainance room, Error: {error.code} {error.Name}",
                room.Id, roomResult.Error.Code, roomResult.Error.Name);
            return Result.Failure(RoomErrors.InvalidStatus);
        }

        var result = room.ResetStatus();
        if (result.IsFailure)
            return result;

        var updateResult = await _roomRepository.UpdateRoomAsync(room.Id, room, cancellationToken);
        return updateResult;
    }
}