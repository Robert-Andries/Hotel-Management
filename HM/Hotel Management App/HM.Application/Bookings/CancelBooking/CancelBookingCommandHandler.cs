using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Abstractions;

namespace HM.Application.Bookings.CancelBooking;

public class CancelBookingCommandHandler : ICommandHandler<CancelBookingCommand, Result>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITime _time;

    public CancelBookingCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork, ITime time)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _time = time;
    }

    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var bookingResult = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if(bookingResult.IsFailure)
            return Result.Failure(bookingResult.Error);
        var booking = bookingResult.Value;
        
        var result = booking.Cancel(_time.NowUtc);

        if (result.IsFailure)
            return Result.Failure(result.Error);
        
        await _bookingRepository.Update(booking.Id, booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}