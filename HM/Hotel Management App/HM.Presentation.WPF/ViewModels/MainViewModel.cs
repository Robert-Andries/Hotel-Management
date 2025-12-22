using System.Windows.Input;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.Utilities;
using HM.Presentation.WPF.ViewModels.Bookings;
using HM.Presentation.WPF.ViewModels.Rooms;
using Microsoft.Extensions.Logging;

namespace HM.Presentation.WPF.ViewModels;

internal class MainViewModel : BaseViewModel
{
    #region Private Fields

    private readonly ILogger<MainViewModel> _logger;

    #endregion

    public MainViewModel(INavigationStore navigationStore, ILogger<MainViewModel> logger) : base(navigationStore)
    {
        _logger = logger;
        if (navigationStore.CurrentViewModel == null)
            navigationStore.NavigateTo<BookingViewModel>();

        BookingsCommand = new RelayCommand(ExecuteBookings);
        RoomsCommand = new RelayCommand(ExecuteRooms);
    }

    #region Commands

    public ICommand BookingsCommand { get; set; }
    public ICommand RoomsCommand { get; set; }

    #endregion

    #region Execute

    private void ExecuteBookings()
    {
        _logger.LogDebug("Navigating to BookingViewModel");
        NavigationStore.NavigateTo<BookingViewModel>();
    }

    private void ExecuteRooms()
    {
        _logger.LogDebug("Navigating to RoomViewModel");
        NavigationStore.NavigateTo<RoomViewModel>();
    }

    #endregion
}