using System.Collections.ObjectModel;
using System.Windows.Input;
using HM.Application.Bookings.AddBooking;
using HM.Application.Rooms.GetAvailableRooms;
using HM.Domain.Rooms.Value_Objects;
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

        ConfirmCommand = new AsyncRelayCommand(ConfirmExecute, ConfirmCanExecute, OnException);
        CancelCommand = new RelayCommand(CancelExecute);
        RefreshRoomsCommand = new RelayCommand(LoadRooms);
    }

    #region Events

    public event Action<bool?>? RequestClose;

    #endregion

    #region CanExecute

    private bool ConfirmCanExecute()
    {
        return SelectedRoom != null && EndDate > StartDate;
    }

    #endregion

    #region Methods

    public void InitializeUser(Guid userId)
    {
        _userId = userId;
        LoadRooms();
    }

    private async void LoadRooms()
    {
        try
        {
            var query = new GetAvailableRoomsQuery(
                DateOnly.FromDateTime(StartDate),
                DateOnly.FromDateTime(EndDate),
                new List<Feautre>());
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to load rooms: {Error}", result.Error);
                return;
            }

            Rooms.Clear();
            foreach (var room in result.Value) Rooms.Add(room);

            if (SelectedRoom != null && Rooms.All(r => r.RoomId != SelectedRoom.RoomId)) SelectedRoom = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in LoadRooms");
            ErrorMessage = "Failed to load rooms due to an unexpected error.";
        }
    }

    private void OnException(Exception ex)
    {
        _logger.LogError(ex, "Error in CreateBookingDialogViewModel");
        ErrorMessage = "An unexpected error occurred.";
    }

    #endregion

    #region Properties

    public ObservableCollection<RoomSearchResponse> Rooms { get; } = new();

    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (_startDate == value) return;
            _startDate = value;
            OnPropertyChanged();
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
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
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
            LoadRooms();
        }
    }

    public RoomSearchResponse? SelectedRoom
    {
        get => _selectedRoom;
        set
        {
            if (Equals(_selectedRoom, value)) return;
            _selectedRoom = value;
            OnPropertyChanged();
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
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

        var command = new AddBookingCommand(_userId, start, end, SelectedRoom.RoomId);

        _logger.LogInformation("Creating booking for user {UserId}, Room {RoomId}, Dates: {Start}-{End}", _userId,
            SelectedRoom.RoomId, start, end);
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

    private void CancelExecute()
    {
        RequestClose?.Invoke(false);
    }

    #endregion

    #region Private Fields

    private readonly IMediator _mediator;
    private readonly ILogger<CreateBookingDialogViewModel> _logger;
    private Guid _userId;
    private DateTime _startDate = DateTime.Today;
    private DateTime _endDate = DateTime.Today.AddDays(1);
    private RoomSearchResponse? _selectedRoom;
    private string _errorMessage = string.Empty;

    #endregion
}