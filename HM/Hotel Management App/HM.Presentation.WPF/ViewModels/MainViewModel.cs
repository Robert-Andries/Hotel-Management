using System.Windows.Input;
using HM.Presentation.WPF.Stores;

namespace HM.Presentation.WPF.ViewModels;

internal class MainViewModel : BaseViewModel
{
    public MainViewModel(INavigationStore navigationStore) : base(navigationStore)
    {
        if (navigationStore.CurrentViewModel == null)
            navigationStore.NavigateTo<BookingViewModel>();

        BookingsCommand = new DelegateCommand(ExecuteBookings);
        RoomsCommand = new DelegateCommand(ExecuteRooms);
    }

    #region Commands
    public ICommand BookingsCommand { get; set; }
    public ICommand RoomsCommand { get; set; }
    #endregion

    #region Execute
    private void ExecuteBookings()
    {
        NavigationStore.NavigateTo<BookingViewModel>();
    }

    private void ExecuteRooms()
    {
        NavigationStore.NavigateTo<RoomViewModel>();
    }
    #endregion
}