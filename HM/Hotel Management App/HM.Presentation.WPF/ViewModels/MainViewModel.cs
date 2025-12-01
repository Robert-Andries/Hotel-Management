using HM.Presentation.WPF.Stores;

namespace HM.Presentation.WPF.ViewModels;

internal class MainViewModel : BaseViewModel
{
    public MainViewModel(INavigationStore navigationStore) : base(navigationStore)
    {
    }
}