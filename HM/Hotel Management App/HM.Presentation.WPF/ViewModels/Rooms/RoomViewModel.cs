using System.Collections.ObjectModel;
using System.Windows.Input;
using HM.Application.Rooms.GetAllRooms;
using HM.Application.Rooms.GetRoom;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.Utilities;
using HM.Presentation.WPF.ViewModels.Rooms.Dialogs;
using MediatR;
using Microsoft.Extensions.Logging;
using IDialogService = HM.Presentation.WPF.Services.IDialogService;

namespace HM.Presentation.WPF.ViewModels.Rooms;

public class RoomViewModel : BaseViewModel
{
    public RoomViewModel(INavigationStore navigationStore, IMediator mediator,
        IDialogService dialogService, EditRoomDialogViewModel editRoomDialogViewModel,
        ILogger<RoomViewModel> logger)
        : base(navigationStore)
    {
        _mediator = mediator;
        _dialogService = dialogService;
        _editRoomDialogViewModel = editRoomDialogViewModel;
        _logger = logger;
        RefreshCommand = new AsyncRelayCommand(RefreshExecute, null, OnException);
        AddCommand = new AsyncRelayCommand(AddExecute, null, OnException);
        EditRoomCommand = new AsyncRelayCommand(EditRoomExecute, null, OnException);
    }

    #region Methods

    private void OnException(Exception ex)
    {
        _logger.LogError(ex, "Command execution failed");
    }

    #endregion

    #region Properties

    // ReSharper disable once CollectionNeverQueried.Global
    public ObservableCollection<RoomResponse> Rooms { get; set; } = new();

    public RoomResponse? Room
    {
        get => _room;
        set
        {
            if (value == null || Equals(value, _room)) return;
            _room = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ICommand RefreshCommand { get; set; }
    public ICommand AddCommand { get; set; }
    public ICommand EditRoomCommand { get; set; }

    #endregion

    #region Execute

    private async Task RefreshExecute()
    {
        _logger.LogInformation("Refresh rooms called");
        var roomsResponseResult = await Task.Run(async () => await _mediator.Send(new GetAllRoomsQuery()));
        if (roomsResponseResult.IsFailure)
        {
            _logger.LogWarning("Refresh failed, Error: {ErrorCode} : {ErrorName}",
                roomsResponseResult.Error.Code, roomsResponseResult.Error.Name);
            return;
        }

        var roomResponse = roomsResponseResult.Value;

        Rooms.Clear();
        foreach (var room in roomResponse)
        {
            Rooms.Add(room);
        }

        _logger.LogInformation("Room list updated with {count} rooms", Rooms.Count);
    }

    private async Task AddExecute()
    {
        _logger.LogInformation("Adding new room...");
        var flag = _dialogService.ShowDialog<AddRoomDialogViewModel>();

        if (flag == true)
        {
            _logger.LogInformation("A new room has been added");
            await RefreshExecute();
        }
        else
        {
            _logger.LogInformation("Adding new room operation was cancelled");
        }
    }

    private async Task EditRoomExecute()
    {
        _logger.LogInformation("Editing room with ID: {id}...", Room!.Id);
        _editRoomDialogViewModel.InitializeRoom(Room);
        var flag = _dialogService.ShowDialog(_editRoomDialogViewModel);
        if (flag == true)
        {
            _logger.LogInformation("The room with ID: {id} has been successfully edited", Room.Id);
            await RefreshExecute();
        }
        else
        {
            _logger.LogInformation("The edit operation for room with ID: {id} has been cancelled", Room.Id);
        }
    }

    #endregion

    #region Private Fields

    private readonly IMediator _mediator;
    private readonly IDialogService _dialogService;
    private readonly ILogger<RoomViewModel> _logger;
    private readonly EditRoomDialogViewModel _editRoomDialogViewModel;
    private RoomResponse? _room;

    #endregion
}