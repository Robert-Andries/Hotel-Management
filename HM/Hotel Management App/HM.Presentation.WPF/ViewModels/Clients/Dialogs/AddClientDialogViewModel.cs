using System.Runtime.CompilerServices;
using System.Windows.Input;
using HM.Application.Users.AddUser;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HM.Presentation.WPF.ViewModels.Clients.Dialogs;

public class AddClientDialogViewModel : BaseViewModel, IDialogViewModel
{
    private readonly ILogger<AddClientDialogViewModel> _logger;
    private readonly IMediator _mediator;
    private string _countryCode = "+40";
    private DateTime _dateOfBirth = DateTime.Today.AddYears(-18);
    private string _email = string.Empty;
    private string _errorMessage = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _phoneNumber = string.Empty;

    public AddClientDialogViewModel(INavigationStore navigationStore, IMediator mediator,
        ILogger<AddClientDialogViewModel> logger)
        : base(navigationStore)
    {
        _mediator = mediator;
        _logger = logger;

        ConfirmCommand = new AsyncRelayCommand(ConfirmExecute, CanConfirmExecute, OnException);
        CancelCommand = new RelayCommand(CancelExecute);
    }

    #region Events

    public event Action<bool?>? RequestClose;

    #endregion

    #region Properties

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (SetProperty(ref _firstName, value)) OnPropertyChanged(nameof(CanConfirmExecute));
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (SetProperty(ref _lastName, value)) OnPropertyChanged(nameof(CanConfirmExecute));
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value)) OnPropertyChanged(nameof(CanConfirmExecute));
        }
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            if (SetProperty(ref _phoneNumber, value)) OnPropertyChanged(nameof(CanConfirmExecute));
        }
    }

    public string CountryCode
    {
        get => _countryCode;
        set
        {
            if (SetProperty(ref _countryCode, value)) OnPropertyChanged(nameof(CanConfirmExecute));
        }
    }

    public DateTime DateOfBirth
    {
        get => _dateOfBirth;
        set => SetProperty(ref _dateOfBirth, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    #endregion

    #region Commands

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    #endregion

    #region Execute

    private async Task ConfirmExecute()
    {
        ErrorMessage = string.Empty;
        var dateOnly = DateOnly.FromDateTime(DateOfBirth);

        var command = new AddUserCommand(FirstName, LastName, PhoneNumber, CountryCode, Email, dateOnly);

        _logger.LogInformation("Adding new user: {Email}", Email);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to add user: {Error}", result.Error);
            ErrorMessage = result.Error.Name;
            return;
        }

        _logger.LogInformation("User added successfully");
        RequestClose?.Invoke(true);
    }

    private bool CanConfirmExecute()
    {
        return !string.IsNullOrWhiteSpace(FirstName) &&
               !string.IsNullOrWhiteSpace(LastName) &&
               !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(PhoneNumber) &&
               !string.IsNullOrWhiteSpace(CountryCode);
    }

    private void CancelExecute()
    {
        RequestClose?.Invoke(false);
    }

    private void OnException(Exception ex)
    {
        _logger.LogError(ex, "Error in AddClientDialogViewModel");
        ErrorMessage = "An unexpected error occurred.";
    }

    // Helper for property setting and notification
    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}