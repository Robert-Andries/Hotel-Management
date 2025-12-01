using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Entities;
using Microsoft.Extensions.Logging;

namespace HM.Application.Rooms.AddRoom;

public class AddRoomCommandHandler : ICommandHandler<AddRoomCommand, Result>
{
    private readonly IRoomRepository _roomRepository;
    private readonly ILogger<AddRoomCommandHandler> _logger;

    public AddRoomCommandHandler(IRoomRepository roomRepository, ILogger<AddRoomCommandHandler> logger)
    {
        _roomRepository = roomRepository;
        _logger = logger;
    }


    public async Task<Result> Handle(AddRoomCommand request, CancellationToken cancellationToken)
    {
        var roomResult = Room.Create(request.Type, request.Location, request.Feautres, request.Price);
        if (roomResult.IsFailure)
        {
            _logger.LogError("Room could not be created {error}", roomResult.Error);
            return Result.Failure(roomResult.Error);
        }
        var room = roomResult.Value;
        
        var result = await _roomRepository.AddAsync(room, cancellationToken);
        if(result.IsFailure)
        {
            _logger.LogError("Room could not be added {error}", result.Error);
            return Result.Failure(result.Error);
        }
        return Result.Success();
    }
}