using HM.Application.Abstractions.Messaging;
using HM.Application.Users.Services;
using HM.Domain.Abstractions;
using HM.Domain.Bookings;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Services;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Users.Abstractions;

namespace HM.Application.Bookings.CreateBookingForGuest;

internal sealed class CreateBookingForGuestCommandHandler : ICommandHandler<CreateBookingForGuestCommand, Result<Guid>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPricingService _pricingService;
    private readonly IRoomRepository _roomRepository;
    private readonly ITime _time;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserCreationService _userCreationService;
    private readonly IUserRepository _userRepository;

    public CreateBookingForGuestCommandHandler(
        IUserRepository userRepository,
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        IPricingService pricingService,
        IUnitOfWork unitOfWork,
        ITime time,
        UserCreationService userCreationService)
    {
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _pricingService = pricingService;
        _unitOfWork = unitOfWork;
        _time = time;
        _userCreationService = userCreationService;
    }

    public async Task<Result<Guid>> Handle(CreateBookingForGuestCommand request, CancellationToken cancellationToken)
    {
        // Validate Date Range
        var dateRangeResult = DateRange.Create(request.StartDate, request.EndDate);
        if (dateRangeResult.IsFailure) return Result.Failure<Guid>(dateRangeResult.Error);

        var dateRange = dateRangeResult.Value;

        // Gets user id 
        Guid userId;
        var existingUserResult = await _userCreationService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUserResult.IsSuccess)
        {
            userId = existingUserResult.Value.Id;
        }
        else
        {
            var userResult = _userCreationService.CreateUser(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.CountryCode,
                request.DateOfBirth);

            if (userResult.IsFailure) return Result.Failure<Guid>(userResult.Error);

            var user = userResult.Value;
            _userRepository.Add(user);
            userId = user.Id;
        }

        // Load Room
        var roomResult = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (roomResult.IsFailure) return Result.Failure<Guid>(roomResult.Error);
        var room = roomResult.Value;

        // Check Overlap
        var isOverlappingResult = await _bookingRepository.IsOverlappingAsync(room, dateRange, cancellationToken);
        if (isOverlappingResult.IsFailure || isOverlappingResult.Value)
            return Result.Failure<Guid>(BookingErrors.Overlapping);

        // Calculate Price
        var priceDetails = _pricingService.CalculatePrice(room, dateRange);

        // Reserve Booking
        var booking = Booking.Reserve(
            room.Id,
            userId,
            dateRange,
            _time.NowUtc,
            priceDetails.TotalPrice);

        // Final Overlap Check (Race Condition Mitigation)
        var isOverlappingReCheck = await _bookingRepository.IsOverlappingAsync(room, dateRange, cancellationToken);
        if (isOverlappingReCheck.IsFailure || isOverlappingReCheck.Value)
            return Result.Failure<Guid>(BookingErrors.Overlapping);

        // Save
        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(booking.Id);
    }
}