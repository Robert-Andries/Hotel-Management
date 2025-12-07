using System.Collections.ObjectModel;
using System.Windows.Input;
using HM.Application.Rooms.GetAllRooms;
using HM.Presentation.WPF.Stores;
using MediatR;

namespace HM.Presentation.WPF.ViewModels;

//TODO Add Logging
public class RoomViewModel : BaseViewModel
{
    public RoomViewModel(INavigationStore navigationStore, IMediator mediator,
        HM.Presentation.WPF.Services.IDialogService dialogService)
        : base(navigationStore)
    {
        InitialiseCommands();
        _mediator = mediator;
        _dialogService = dialogService;
    }

    #region Properties

    // ReSharper disable once CollectionNeverQueried.Global
    public ObservableCollection<RoomResponse> Rooms { get; set; } = new();

    #endregion

    #region Methods

    #region Initialise

    private void InitialiseCommands()
    {
        RefreshCommand = new DelegateCommand(async void () => await RefreshExecute());
        AddCommand = new DelegateCommand(async void () => await AddExecute());
        EditRoomCommand =
            new DelegateCommand<RoomResponse>(async void (roomResponse) => await EditRoomExecute(roomResponse));
    }

    #endregion

    #endregion

    #region Commands

    public ICommand RefreshCommand { get; set; }
    public ICommand AddCommand { get; set; }
    public ICommand EditRoomCommand { get; set; }

    #endregion

    #region Execute

    private async Task RefreshExecute()
    {
        var roomsResponseResult = await _mediator.Send(new GetAllRoomsQuery());
        if (roomsResponseResult.IsFailure)
            return;
        var roomResponse = roomsResponseResult.Value;
        foreach (var room in roomResponse)
            Rooms.Add(room);
    }

    private async Task AddExecute()
    {
        bool? result = _dialogService.ShowDialog<AddRoomDialogViewModel>();

        if (result == true) await RefreshExecute();
    }

    private async Task EditRoomExecute(RoomResponse? room)
    {
        if (room == null) return;
    }

    #endregion

    #region Private Fields

    private readonly IMediator _mediator;
    private readonly HM.Presentation.WPF.Services.IDialogService _dialogService;

    #endregion
}