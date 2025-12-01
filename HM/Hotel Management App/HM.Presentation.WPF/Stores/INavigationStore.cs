using HM.Presentation.WPF.ViewModels;

namespace HM.Presentation.WPF.Stores;

/// <summary>
/// Interface for navigation store to manage current view model and navigation.
/// </summary>
public interface INavigationStore
{
    BaseViewModel CurrentViewModel { get; }
    void NavigateTo<TViewModel>() where TViewModel : BaseViewModel;
}