using System.Windows.Input;
using HM.Application.Rooms.FinishMaintenance;
using HM.Application.Rooms.GetAllRooms;
using HM.Domain.Rooms.Value_Objects;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using MediatR;

namespace HM.Presentation.WPF.ViewModels;

public class EditRoomDialogViewModel : BaseViewModel, IDialogViewModel
{

    public EditRoomDialogViewModel(INavigationStore navigationStore, IMediator mediator) : base(navigationStore)
    {
        _mediator = mediator;
        CancelCommand = new DelegateCommand(CancelExecute);
        ClearMaintenanceCommand = new DelegateCommand(async void () => await ClearMaintenanceExecute(),
            ClearMaintenanceCanExecute);
    }
    
    #region Proprieties
    public RoomResponse? Room { get; private set; }
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (value == _errorMessage) return;
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    #endregion
    
    #region Methods
    public void InitializeRoom(RoomResponse room)
    {
        Room = room;
    }
    #endregion
    
    #region Commands

    public ICommand CancelCommand { get; set; }
    public ICommand ClearMaintenanceCommand { get; set; }
    #endregion
    
    #region Executes

    public void CancelExecute()
    {
        RequestClose?.Invoke(false);
    }

    public async Task ClearMaintenanceExecute()
    {
        var finishMaintenanceCommand = new FinishMaintenanceCommand(Room!.Id);
        var result = await _mediator.Send(finishMaintenanceCommand);
        if (result.IsFailure)
        {
            ErrorMessage = result.Error.Name;
            return;
        }
        RequestClose?.Invoke(true);
    }
    #endregion
    
    #region CanExecute

    public bool ClearMaintenanceCanExecute()
    {
        if (Room?.Status == RoomStatus.Maintanance)
            return true;
        return false;
    }
    #endregion
    
    #region Events
    public event Action<bool?>? RequestClose;
    #endregion
    
    #region Private Fields
    private string _errorMessage = string.Empty;
    private readonly IMediator _mediator;
    #endregion
}