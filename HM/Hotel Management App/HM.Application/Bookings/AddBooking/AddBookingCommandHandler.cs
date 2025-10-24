using HM.Application.Abstractions.Messaging;
using HM.Application.Rooms.ChangeStatus;
using HM.Domain.Abstractions;
using HM.Domain.Bookings;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Users.Abstractions;
using MediatR;

namespace HM.Application.Bookings.AddBooking;

internal sealed class AddBookingCommandHandler : ICommandHandler<AddBookingCommand, Result<Guid>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMediator _mediator;

    public AddBookingCommandHandler(IRoomRepository roomRepository, IUserRepository userRepository, IBookingRepository bookingRepository, IMediator mediator)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
        _mediator = mediator;
    }


    public async Task<Result<Guid>> Handle(AddBookingCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if(userResult.IsFailure)
            return Result.Failure<Guid>(userResult.Error);
        
        var roomResult = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if(roomResult.IsFailure)
            return Result.Failure<Guid>(roomResult.Error);
        
        if(await _bookingRepository.IsOverlappingAsync(roomResult.Value, request.Range, cancellationToken))
            return Result.Failure<Guid>(BookingErrors.Overlaping);
        
        var bookingResult = await _bookingRepository.Add(
            userResult.Value.Id,
            request.Range,
            roomResult.Value.Id,
            cancellationToken);
        
        if(bookingResult.IsFailure)
            return Result.Failure<Guid>(bookingResult.Error);

        var result = await _mediator.Send(new ChangeStatusCommand(request.RoomId, RoomStatus.Reserved), cancellationToken);
        if(result.IsFailure)
            return Result.Failure<Guid>(result.Error);
        
        return Result.Success<Guid>(bookingResult.Value.Id);
    }
}