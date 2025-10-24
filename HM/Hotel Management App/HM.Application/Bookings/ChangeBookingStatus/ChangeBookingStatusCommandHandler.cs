using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Value_Objects;

namespace HM.Application.Bookings.ChangeBookingStatus;

internal sealed class ChangeBookingStatusCommandHandler :  ICommandHandler<ChangeBookingStatusCommand, Result>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITime _time;

    public ChangeBookingStatusCommandHandler(IBookingRepository bookingRepository, ITime time)
    {
        _bookingRepository = bookingRepository;
        _time = time;
    }

    public async Task<Result> Handle(ChangeBookingStatusCommand request, CancellationToken cancellationToken)
    {
        if(request.Status == BookingStatus.Reserved)
            return Result.Failure(new Error("Booking.Status", "You cannot change status to Reserved. If you wish to change status to Reserved"));
        
        var bookingResponse = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (bookingResponse.IsFailure)
            return Result.Failure(bookingResponse.Error);
        
        var booking = bookingResponse.Value;
        if(booking.Status == request.Status)
            return Result.Failure(new Error("Booking.Status","This booking already have this status"));
        
        booking.Status = request.Status;
        switch (booking.Status)
        {
            case BookingStatus.Cancelled:
                booking.CancelledOnUtc = _time.NowUtc;
                break;
            case BookingStatus.Completed:
                booking.CompletedOnUtc = _time.NowUtc;
                break;
            case BookingStatus.Confirmed:
                booking.ConfirmedOnUtc = _time.NowUtc;
                break;
            case BookingStatus.Rejected:
                booking.RejectedOnUtc = _time.NowUtc;
                break;
            default:
                return  Result.Failure(new Error("Booking.Status","This booking is in an invalid state"));
        }
        
        var updateResponse = await _bookingRepository.Update(booking.Id, booking, cancellationToken);
        if(updateResponse.IsFailure)
            return Result.Failure(updateResponse.Error);
        
        return Result.Success();
    }
}