using System.Windows.Input;
using HM.Application.Bookings.CheckInGuest;
using HM.Application.Bookings.CheckOutGuest;
using HM.Application.Bookings.Shared;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Value_Objects;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HM.Presentation.WPF.ViewModels.Bookings.Dialogs;

public class EditBookingDialogViewModel : BaseViewModel, IDialogViewModel
{
    public EditBookingDialogViewModel(INavigationStore navigationStore, ILogger<EditBookingDialogViewModel> logger,
        IMediator mediator)
        : base(navigationStore)
    {
        _logger = logger;
        _mediator = mediator;
        CancelCommand = new RelayCommand(CancelExecute);
        ExecuteActionCommand = new AsyncRelayCommand(ExecuteActionExecute, ExecuteActionCanExecute, OnException);
    }

    #region Events

    public event Action<bool?>? RequestClose;

    #endregion

    #region CanExecute

    private bool ExecuteActionCanExecute()
    {
        return Booking != null && IsActionButtonVisible;
    }

    #endregion

    #region Properties

    public BookingResponse? Booking
    {
        get => _booking;
        set
        {
            if (Equals(value, _booking)) return;
            _booking = value;
            OnPropertyChanged();
        }
    }

    public string ActionButtonText
    {
        get => _actionButtonText;
        set
        {
            if (value == _actionButtonText) return;
            _actionButtonText = value;
            OnPropertyChanged();
        }
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsActionButtonVisible
    {
        get => _isActionButtonVisible;
        set
        {
            if (value == _isActionButtonVisible) return;
            _isActionButtonVisible = value;
            OnPropertyChanged();
        }
    }

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

    public void InitializeBooking(BookingResponse booking)
    {
        Booking = booking;
        UpdateActionButtonState();
    }

    private void UpdateActionButtonState()
    {
        if (Booking == null)
            return;
        IsActionButtonVisible = true;
        switch (Booking.Status)
        {
            case BookingStatus.Reserved:
                ActionButtonText = "Check In";
                break;
            case BookingStatus.CheckedIn:
                ActionButtonText = "Check Out";
                break;
            default:
                ActionButtonText = string.Empty;
                IsActionButtonVisible = false;
                break;
        }
    }

    private void OnException(Exception ex)
    {
        _logger.LogError(ex, "Error executing action for booking {BookingId}", Booking?.Id);
    }

    #endregion

    #region Commands

    public ICommand CancelCommand { get; }
    public ICommand ExecuteActionCommand { get; }

    #endregion

    #region Execute

    private void CancelExecute()
    {
        RequestClose?.Invoke(false);
    }

    private async Task ExecuteActionExecute()
    {
        if (Booking == null) return;

        Result result;
        switch (Booking.Status)
        {
            case BookingStatus.Reserved:
            {
                var command = new CheckInGuestCommand(Booking.Id);
                result = await _mediator.Send(command);
                break;
            }
            case BookingStatus.CheckedIn:
            {
                var command = new CheckOutGuestCommand(Booking.Id);
                result = await _mediator.Send(command);
                break;
            }
            default:
                return;
        }

        if (result.IsFailure)
        {
            _logger.LogWarning("Action failed: {Error}", result.Error);
            ErrorMessage = result.Error.Name;
            return;
        }

        _logger.LogInformation("Action executed successfully for booking {BookingId}", Booking.Id);
        RequestClose?.Invoke(true);
    }

    #endregion

    #region Private Fields

    private readonly ILogger<EditBookingDialogViewModel> _logger;
    private readonly IMediator _mediator;
    private BookingResponse? _booking;
    private string _actionButtonText = string.Empty;
    private bool _isActionButtonVisible;
    private string _errorMessage = string.Empty;

    #endregion
}