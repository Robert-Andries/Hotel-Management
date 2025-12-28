using HM.Domain.Rooms.Value_Objects;

namespace HM.Presentation.WebUI.ViewModels;

public class SearchRoomViewModel
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public RoomType RoomType { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CountryCode { get; set; } = "+40";
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));
    public List<Feautre> SelectedFeatures { get; set; } = new();
}