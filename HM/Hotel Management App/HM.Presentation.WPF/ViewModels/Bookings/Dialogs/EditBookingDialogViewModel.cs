using HM.Application.Bookings.GetBooking;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using Microsoft.Extensions.Logging;

namespace HM.Presentation.WPF.ViewModels.Bookings.Dialogs;

public class EditBookingDialogViewModel : BaseViewModel, IDialogViewModel
{

    public EditBookingDialogViewModel(INavigationStore navigationStore, ILogger<EditBookingDialogViewModel> logger)
        : base(navigationStore)
    {
        _logger = logger;
    }

    #region Properties
    public BookingResponse Booking
    {
        get => _booking;
        set
        {
            if (Equals(value, _booking)) return;
            _booking = value;
            OnPropertyChanged();
        }
    }
    #endregion
    
    #region Methods
    public void InitializeBooking(BookingResponse booking)
    {
        Booking = booking;
    }
    #endregion
    
    #region Events
    public event Action<bool?>? RequestClose;
    #endregion
    
    #region Private Fields
    private readonly ILogger<EditBookingDialogViewModel> _logger;
    private BookingResponse _booking;
    #endregion
}