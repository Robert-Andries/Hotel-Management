using HM.Application.Abstractions.Messaging;
using HM.Domain.Abstractions;

namespace HM.Application.Bookings.GetBookingPreview;

public sealed record GetBookingPreviewQuery(Guid RoomId, DateOnly StartDate, DateOnly EndDate)
    : IQuery<Result<BookingPreviewResponse>>;