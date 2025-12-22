using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;

namespace HM.Presentation.WPF.ViewModels.Bookings.Dialogs;

public class AddBookingDialogViewModel : BaseViewModel, IDialogViewModel
{
    public AddBookingDialogViewModel(INavigationStore navigationStore) : base(navigationStore)
    {
        
    }

    public event Action<bool?>? RequestClose;
}