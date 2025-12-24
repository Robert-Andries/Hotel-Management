using System.Collections.ObjectModel;
using System.Windows.Input;
using HM.Application.Bookings.AddBooking;
using HM.Application.Rooms.GetAvailableRooms;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HM.Presentation.WPF.ViewModels.Clients.Dialogs;

public class CreateBookingDialogViewModel : BaseViewModel, IDialogViewModel
{
    public CreateBookingDialogViewModel(INavigationStore navigationStore, IMediator mediator,
        ILogger<CreateBookingDialogViewModel> logger)
        : base(navigationStore)
    {
        _mediator = mediator;
        _logger = logger;

        ConfirmCommand = new AsyncRelayCommand(ConfirmExecute, CanConfirmExecute, OnException);
        CancelCommand = new RelayCommand(CancelExecute);
        RefreshRoomsCommand = new RelayCommand(LoadRooms);
    }

    #region Events

    public event Action<bool?>? RequestClose;

    #endregion

    #region Methods

    public void InitializeUser(Guid userId)
    {
        _userId = userId;
        LoadRooms();
    }

    private async void LoadRooms()
    {
        // Use the current dates for the query
        var query = new GetAvailableRoomsQuery(DateOnly.FromDateTime(StartDate), DateOnly.FromDateTime(EndDate));
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to load rooms: {Error}", result.Error);
            Rooms.Clear(); // Clear if failed or invalid dates
            return;
        }

        Rooms.Clear();
        foreach (var room in result.Value) Rooms.Add(room);

        // Clear selection if the previously selected room is no longer available
        if (SelectedRoom != null && !Rooms.Any(r => r.Id == SelectedRoom.Id)) SelectedRoom = null;
    }

    #endregion

    #region Properties

    public ObservableCollection<AvailableRoomResponse> Rooms { get; } = new();

    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (_startDate == value) return;
            _startDate = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanConfirmExecute));
            LoadRooms();
        }
    }

    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            if (_endDate == value) return;
            _endDate = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanConfirmExecute));
            LoadRooms();
        }
    }

    public AvailableRoomResponse? SelectedRoom
    {
        get => _selectedRoom;
        set
        {
            if (Equals(_selectedRoom, value)) return;
            _selectedRoom = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanConfirmExecute));
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage == value) return;
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand RefreshRoomsCommand { get; }

    #endregion

    #region Execute

    private async Task ConfirmExecute()
    {
        ErrorMessage = string.Empty;
        var start = DateOnly.FromDateTime(StartDate);
        var end = DateOnly.FromDateTime(EndDate);

        if (SelectedRoom == null) return;

        var command = new AddBookingCommand(_userId, start, end, SelectedRoom.Id);

        _logger.LogInformation("Creating booking for user {UserId}, Room {RoomId}, Dates: {Start}-{End}", _userId,
            SelectedRoom.Id, start, end);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to create booking: {Error}", result.Error);
            ErrorMessage = result.Error.Name;
            return;
        }

        _logger.LogInformation("Booking created successfully");
        RequestClose?.Invoke(true);
    }

    private bool CanConfirmExecute()
    {
        return SelectedRoom != null && EndDate > StartDate;
    }

    private void CancelExecute()
    {
        RequestClose?.Invoke(false);
    }

    private void OnException(Exception ex)
    {
        _logger.LogError(ex, "Error in CreateBookingDialogViewModel");
        ErrorMessage = "An unexpected error occurred.";
    }

    #endregion

    #region Private Fields

    private readonly IMediator _mediator;
    private readonly ILogger<CreateBookingDialogViewModel> _logger;
    private Guid _userId;
    private DateTime _startDate = DateTime.Today;
    private DateTime _endDate = DateTime.Today.AddDays(1);
    private AvailableRoomResponse? _selectedRoom;
    private string _errorMessage = string.Empty;

    #endregion
}