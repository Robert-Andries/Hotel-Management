using System.Windows;
using HM.Presentation.WPF.ViewModels;

namespace HM.Presentation.WPF.Services;

public class DialogService : IDialogService
{
    private readonly Func<Type, BaseViewModel> _viewModelFactory;
    private readonly Dictionary<Type, Type> _mappings = new();

    public DialogService(Func<Type, BaseViewModel> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void Register<TViewModel, TWindow>() 
        where TViewModel : BaseViewModel 
        where TWindow : Window
    {
        _mappings[typeof(TViewModel)] = typeof(TWindow);
    }

    public bool? ShowDialog<TViewModel>(TViewModel? viewModel = null) 
        where TViewModel : BaseViewModel
    {
        Type viewModelType = typeof(TViewModel);
        BaseViewModel effectiveViewModel = viewModel ?? _viewModelFactory(viewModelType);

        if (!_mappings.TryGetValue(viewModelType, out var windowType))
            throw new InvalidOperationException($"No window registered for {viewModelType.Name}");

        var window = (Window)Activator.CreateInstance(windowType)!;

        window.DataContext = effectiveViewModel;
        window.Owner = System.Windows.Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        
        if (effectiveViewModel is IDialogViewModel dialogVm) 
        {
            dialogVm.RequestClose += (result) =>
            {
                window.DialogResult = result;
                window.Close();
            };
        }

        return window.ShowDialog();
    }
}