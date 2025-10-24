using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Rooms.Abstractions;

namespace HM.Application.Rooms.UpdateRating;

internal sealed class UpdateRatingCommandHandler : ICommandHandler<UpdateRatingCommand, Result>
{
    private readonly IRoomRepository _roomRepository;

    public UpdateRatingCommandHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<Result> Handle(UpdateRatingCommand request, CancellationToken cancellationToken)
    {
        var roomResult = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (roomResult.IsFailure)
            return Result.Failure<Guid>(roomResult.Error);

        roomResult.Value.Rating = request.NewRating;
        var updateResult = await _roomRepository.UpdateRoom(roomResult.Value.Id, roomResult.Value, cancellationToken);
        return updateResult;
    }
}