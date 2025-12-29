using HM.Domain.Rooms.Value_Objects;

namespace HM.Presentation.WebUI.ViewModels;

public class ConfirmBookingViewModel
{
    public Guid RoomId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public RoomType RoomType { get; set; }
    public decimal PricePerNightAmount { get; set; }
    public decimal TotalPriceAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<Feature> Features { get; set; } = new();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
}