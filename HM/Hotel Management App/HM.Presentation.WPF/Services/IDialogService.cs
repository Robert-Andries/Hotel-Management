using HM.Presentation.WPF.ViewModels;

namespace HM.Presentation.WPF.Services;

public interface IDialogService
{
    bool? ShowDialog<TViewModel>(TViewModel? viewModel = null)
        where TViewModel : BaseViewModel;
}