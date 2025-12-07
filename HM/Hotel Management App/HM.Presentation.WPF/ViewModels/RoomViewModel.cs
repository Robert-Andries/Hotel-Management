using System.Collections.ObjectModel;
using System.Windows.Input;
using HM.Application.Rooms.GetAllRooms;
using HM.Presentation.WPF.Stores;
using MediatR;

namespace HM.Presentation.WPF.ViewModels;

//TODO Add Logging
//TODO Fix freeze when refreshing
public class RoomViewModel : BaseViewModel
{
    public RoomViewModel(INavigationStore navigationStore, IMediator mediator,
        HM.Presentation.WPF.Services.IDialogService dialogService, EditRoomDialogViewModel editRoomDialogViewModel)
        : base(navigationStore)
    {
        InitialiseCommands();
        _mediator = mediator;
        _dialogService = dialogService;
        _editRoomDialogViewModel = editRoomDialogViewModel;
    }

    #region Properties

    // ReSharper disable once CollectionNeverQueried.Global
    public ObservableCollection<RoomResponse> Rooms { get; set; } = new();
    public RoomResponse Room
    {
        get => _room;
        set
        {
            if (Equals(value, _room)) return;
            _room = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Methods

    #region Initialise

    private void InitialiseCommands()
    {
        RefreshCommand = new DelegateCommand(async void () => await RefreshExecute());
        AddCommand = new DelegateCommand(async void () => await AddExecute());
        EditRoomCommand =
            new DelegateCommand(async void () => await EditRoomExecute());
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
        Rooms.Clear();
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
    private async Task EditRoomExecute()
    {
        _editRoomDialogViewModel.InitializeRoom(Room);
        bool? flag = _dialogService.ShowDialog(_editRoomDialogViewModel);
        if (flag is true) await RefreshExecute();
    }

    #endregion

    #region Private Fields

    private readonly IMediator _mediator;
    private readonly HM.Presentation.WPF.Services.IDialogService _dialogService;
    private readonly EditRoomDialogViewModel _editRoomDialogViewModel;
    private RoomResponse _room;

    #endregion
}