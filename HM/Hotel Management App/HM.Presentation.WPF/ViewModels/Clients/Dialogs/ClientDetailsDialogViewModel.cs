using System.Windows.Input;
using HM.Application.Users.Shared;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.Utilities;

namespace HM.Presentation.WPF.ViewModels.Clients.Dialogs;

public class ClientDetailsDialogViewModel : BaseViewModel, IDialogViewModel
{
    public ClientDetailsDialogViewModel(INavigationStore navigationStore, IDialogService dialogService,
        CreateBookingDialogViewModel createBookingDialogViewModel) : base(navigationStore)
    {
        _dialogService = dialogService;
        _createBookingDialogViewModel = createBookingDialogViewModel;
        CloseCommand = new RelayCommand(CloseExecute);
        CreateBookingCommand = new RelayCommand(CreateBookingExecute);
    }

    #region Properties

    public UserResponse? User
    {
        get => _user;
        private set
        {
            if (Equals(value, _user)) return;
            _user = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Events

    public event Action<bool?>? RequestClose;

    #endregion

    #region Methods

    public void InitializeUser(UserResponse user)
    {
        User = user;
    }

    #endregion

    #region Commands

    public ICommand CloseCommand { get; }
    public ICommand CreateBookingCommand { get; }

    #endregion

    #region Execute

    private void CloseExecute()
    {
        RequestClose?.Invoke(false);
    }

    private void CreateBookingExecute()
    {
        if (User == null) return;
        _createBookingDialogViewModel.InitializeUser(User.Id);
        _dialogService.ShowDialog(_createBookingDialogViewModel);
    }

    #endregion

    #region Private Fields

    private UserResponse? _user;
    private readonly IDialogService _dialogService;
    private readonly CreateBookingDialogViewModel _createBookingDialogViewModel;

    #endregion
}