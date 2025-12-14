using System.Windows.Input;
using HM.Application.Rooms.FinishMaintenance;
using HM.Application.Rooms.GetRoom;
using HM.Domain.Rooms.Value_Objects;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HM.Presentation.WPF.ViewModels.Rooms.Dialogs;

public class EditRoomDialogViewModel : BaseViewModel, IDialogViewModel
{
    public EditRoomDialogViewModel(INavigationStore navigationStore, IMediator mediator,
        ILogger<EditRoomDialogViewModel> logger) : base(navigationStore)
    {
        _mediator = mediator;
        _logger = logger;
        CancelCommand = new DelegateCommand(CancelExecute);
        ClearMaintenanceCommand = new DelegateCommand(async void () => await ClearMaintenanceExecute(),
            ClearMaintenanceCanExecute);
    }

    #region Events

    public event Action<bool?>? RequestClose;

    #endregion

    #region Methods

    public void InitializeRoom(RoomResponse room)
    {
        Room = room;
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

    #region Commands

    public ICommand CancelCommand { get; set; }
    public ICommand ClearMaintenanceCommand { get; set; }

    #endregion

    #region Executes

    public void CancelExecute()
    {
        _logger.LogInformation("Edt room dialog cancelled, requesting to close the window...");
        RequestClose?.Invoke(false);
    }

    public async Task ClearMaintenanceExecute()
    {
        _logger.LogInformation("Clearing maintenance for the room with ID: {id}", Room!.Id);
        var finishMaintenanceCommand = new FinishMaintenanceCommand(Room!.Id);
        var result = await _mediator.Send(finishMaintenanceCommand);
        if (result.IsFailure)
        {
            _logger.LogWarning("Maintenance for the room could not be cleared, Error: {ErrorCode} : {ErrorName}",
                result.Error.Code, result.Error.Name);
            ErrorMessage = result.Error.Name;
            return;
        }

        _logger.LogInformation(
            "Maintenance for the room with ID: {id} successfully cleared, requesting to close the window...",
            Room!.Id);
        RequestClose?.Invoke(true);
    }

    #endregion

    #region Private Fields

    private readonly IMediator _mediator;
    private readonly ILogger<EditRoomDialogViewModel> _logger;
    private string _errorMessage = string.Empty;

    #endregion
}