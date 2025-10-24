using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Entities;

namespace HM.Application.Bookings.GetAllBookings;

internal sealed class GetAllBookingsQuerryHandler : IQueryHandler<GetAllBookingsQuerry, Result<List<Booking>>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetAllBookingsQuerryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }
    
    public async Task<Result<List<Booking>>> Handle(GetAllBookingsQuerry request, CancellationToken cancellationToken)
    {
        return await _bookingRepository.GetAllAsync(cancellationToken);
    }
}