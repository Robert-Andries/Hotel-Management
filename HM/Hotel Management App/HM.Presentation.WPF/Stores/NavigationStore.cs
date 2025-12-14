using HM.Presentation.WPF.Utilities;
using HM.Presentation.WPF.ViewModels;

namespace HM.Presentation.WPF.Stores;

/// <summary>
///     Implements navigation between different ViewModels.
/// </summary>
public sealed class NavigationStore : ObservableObject, INavigationStore
{
    private readonly Func<Type, BaseViewModel> _getViewModel;
    private BaseViewModel _currentViewModel = default!;

    public NavigationStore(Func<Type, BaseViewModel> getViewModel)
    {
        _getViewModel = getViewModel;
    }

    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            if (value == null || _currentViewModel == value) return;
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
    {
        CurrentViewModel = _getViewModel?.Invoke(typeof(TViewModel))!;
    }
}