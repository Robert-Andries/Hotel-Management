using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Shared;
using HM.Domain.Users.Abstractions;

namespace HM.Application.Bookings.AddBooking;

internal sealed class AddBookingCommandHandler : ICommandHandler<AddBookingCommand, Result<Guid>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ITime _time;

    public AddBookingCommandHandler(
        IRoomRepository roomRepository,
        IUserRepository userRepository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        ITime time)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _time = time;
    }


    public async Task<Result<Guid>> Handle(AddBookingCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userResult.IsFailure)
            return Result.Failure<Guid>(userResult.Error);
        var user = userResult.Value;

        var roomResult = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (roomResult.IsFailure)
            return Result.Failure<Guid>(roomResult.Error);
        var room = roomResult.Value;

        var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
        if (dateRangeResult.IsFailure)
            return Result.Failure<Guid>(dateRangeResult.Error);
        var dateRange = dateRangeResult.Value;

        var isOverlappingResult =
            await _bookingRepository.IsOverlappingAsync(roomResult.Value, dateRange, cancellationToken);
        if (isOverlappingResult.IsFailure || isOverlappingResult.Value == true)
            return Result.Failure<Guid>(BookingErrors.Overlapping);

        var booking = Booking.Reserve(
            room.Id,
            user.Id,
            dateRange,
            _time.NowUtc,
            Money.Zero());

        await _bookingRepository.AddAsync(booking, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(booking.Id);
    }
}