using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.Utilities;

namespace HM.Presentation.WPF.ViewModels;

/// <summary>
/// Class representing the base view model with navigation capabilities.
/// Used for inheriting common functionality in other view models.
/// </summary>
public class BaseViewModel : ObservableObject
{
    public INavigationStore NavigationStore { get; }
    
    public BaseViewModel(INavigationStore navigationStore)
    {
        NavigationStore = navigationStore;
    }
}