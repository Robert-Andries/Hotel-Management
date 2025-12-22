using System.Collections.ObjectModel;
using System.Windows.Input;
using HM.Application.Bookings.GetAllBookings;
using HM.Application.Bookings.GetBooking;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.Utilities;
using HM.Presentation.WPF.ViewModels.Bookings.Dialogs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HM.Presentation.WPF.ViewModels.Bookings;

public class BookingViewModel : BaseViewModel
{
    public BookingViewModel(INavigationStore navigationStore, ILogger<BookingViewModel> logger,
        IDialogService dialogService, IMediator mediator, EditBookingDialogViewModel editBookingDialogViewModel) : base(navigationStore)
    {
        _logger = logger;
        _dialogService = dialogService;
        _mediator = mediator;
        _editBookingDialogViewModel = editBookingDialogViewModel;
        AddCommand = new AsyncRelayCommand(AddExecute, null, OnException);
        RefreshCommand = new AsyncRelayCommand(RefreshExecute, null, OnException);
        EditBookingsCommand = new AsyncRelayCommand(EditBookingExecute, null, OnException);
    }
    
    #region Proprieties

    public ObservableCollection<BookingResponse> Bookings { get; set; } = new();
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
    public bool SeeCompletedBookings
    {
        get => _seeCompletedBookings;
        set
        {
            if (value == _seeCompletedBookings) return;
            _seeCompletedBookings = value;
            OnPropertyChanged();
        }
    }

    #endregion
    
    #region Methods
    private void OnException(Exception ex)
    {
        _logger.LogError(ex, "Command execution failed");
    }
    #endregion
    
    #region Commands

    public ICommand AddCommand { get; set; }

    public ICommand RefreshCommand { get; set; }

    public ICommand EditBookingsCommand { get; set; }

    #endregion
    
    #region Execute
    
    private async Task AddExecute()
    {
        _logger.LogInformation("Adding new booking...");
        var flag = _dialogService.ShowDialog<AddBookingDialogViewModel>();
        
        if (flag == true)
        {
            _logger.LogInformation("A new booking has been added");
            await RefreshExecute();
        }
        else
        {
            _logger.LogInformation("Adding new booking operation was cancelled");
        }
    }
    
    private async Task RefreshExecute()
    {
        _logger.LogInformation("Refresh bookings was called with completed bookings set to: {CompletedBookings}", SeeCompletedBookings);
        
        var bookingResponseResult = await Task.Run(async () => await _mediator.Send(new GetAllBookingsQuery(SeeCompletedBookings)));
        if (bookingResponseResult.IsFailure)
        {
            _logger.LogWarning("Refresh failed, Error: {ErrorCode} : {ErrorName}",
                bookingResponseResult.Error.Code, bookingResponseResult.Error.Name);
            return;
        }

        var bookingResponse = bookingResponseResult.Value;

        Bookings.Clear();
        foreach (var booking in bookingResponse)
        {
            Bookings.Add(booking);
        }

        _logger.LogInformation("Booking list updated with {count} bookings", Bookings.Count);
    }

    private async Task EditBookingExecute()
    {
        _logger.LogInformation("Editing booking with ID: {id}...", Booking!.Id);
        _editBookingDialogViewModel.InitializeBooking(Booking);
        var flag = _dialogService.ShowDialog(_editBookingDialogViewModel);
        if (flag == true)
        {
            _logger.LogInformation("The booking with ID: {id} has been successfully edited", Booking.Id);
            await RefreshExecute();
        }
        else
        {
            _logger.LogInformation("The edit operation for room with ID: {id} has been cancelled", Booking.Id);
        }
    }
    
    #endregion
    
    #region Private Fields
    
    private readonly ILogger<BookingViewModel> _logger;
    private readonly IDialogService _dialogService;
    private readonly IMediator _mediator;
    private ObservableCollection<BookingResponse> _rooms = new();
    private BookingResponse? _booking;
    private readonly EditBookingDialogViewModel _editBookingDialogViewModel;
    private bool _seeCompletedBookings;

    #endregion
}