using System.Collections.ObjectModel;
using System.Windows.Input;
using HM.Application.Rooms.AddRoom;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Presentation.WPF.Models;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HM.Presentation.WPF.ViewModels.Rooms.Dialogs;

public class AddRoomDialogViewModel : BaseViewModel, IDialogViewModel
{
    public AddRoomDialogViewModel(INavigationStore navigationStore, IMediator mediator, 
        ILogger<AddRoomDialogViewModel> logger)
        : base(navigationStore)
    {
        InitializeFeatures();
        _mediator = mediator;
        _logger = logger;
        SaveCommand = new DelegateCommand(async void () => await SaveExecute(), SaveCanExecute);
        CancelCommand = new DelegateCommand(CancelExecute);
    }

    #region Proprieties
    public IEnumerable<RoomType> RoomTypes { get; }
        = Enum.GetValues(typeof(RoomType)).Cast<RoomType>();
    public ObservableCollection<FeatureSelectionItemModel> FeatureList { get; } = new();
    public IEnumerable<Currency> CurrencyOptions => Currency.All;
    public int Floor
    {
        get => _floor;
        set
        {
            _floor = value;
            OnPropertyChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
    public int RoomNumber
    {
        get => _roomNumber;
        set
        {
            _roomNumber = value;
            OnPropertyChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
    public decimal PriceAmount
    {
        get => _priceAmount;
        set
        {
            _priceAmount = value;
            OnPropertyChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
    public Currency? SelectedCurrency
    {
        get => _selectedCurrency;
        set
        {
            _selectedCurrency = value;
            OnPropertyChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
    public RoomType SelectedRoomType
    {
        get => _selectedRoomType;
        set
        {
            _selectedRoomType = value;
            OnPropertyChanged();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region Methods
    private void InitializeFeatures()
    {
        var allFeatures = Enum.GetValues(typeof(Feautre)).Cast<Feautre>();
        foreach (var feature in allFeatures)
            FeatureList.Add(new FeatureSelectionItemModel(feature, false));
    }
    private List<Feautre> GetSelectedFeatures()
    {
        return FeatureList
            .Where(item => item.IsSelected)
            .Select(item => item.Value)
            .ToList();
    }
    #endregion

    #region Events
    public event Action<bool?>? RequestClose;
    #endregion

    #region Commands
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    #endregion
    
    #region CanExecute
    private bool SaveCanExecute()
    {
        bool isFloorValid = Floor >= 0;
        bool isRoomValid = RoomNumber > 0;
        bool isPriceValid = PriceAmount > 0;
        bool isCurrencyValid = SelectedCurrency != null;
        bool canSave = isFloorValid && isRoomValid && isPriceValid && isCurrencyValid;
        
        return canSave;
    }
    #endregion

    #region Execute
    private async Task SaveExecute()
    {
        _logger.LogInformation("Saving Room Details");
        RoomLocation selectedLocation = new(Floor, RoomNumber);
        var selectedFeautres = GetSelectedFeatures();
        var selectedPrice = new Money(PriceAmount, SelectedCurrency!);

        var addRoomCommand = new AddRoomCommand(SelectedRoomType, selectedLocation, selectedFeautres, selectedPrice);
        var result = await _mediator.Send(addRoomCommand);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to save the Room, Error: {ErrorCode} : {ErrorName}", result.Error.Code, result.Error.Name);
            ErrorMessage = result.Error.Name;
            return;
        }
        _logger.LogInformation("Room Details Saved, requesting to close the window...");
        RequestClose?.Invoke(true);
    }
    private void CancelExecute()
    {
        _logger.LogInformation("Add room dialog cancelled, requesting to close the window...");
        RequestClose?.Invoke(false);
    }
    #endregion

    #region PrivateFields
    private readonly IMediator _mediator;
    private readonly ILogger<AddRoomDialogViewModel> _logger;
    private Currency? _selectedCurrency;
    private RoomType _selectedRoomType;
    private string _errorMessage = string.Empty;
    private decimal _priceAmount;
    private int _floor;
    private int _roomNumber;
    #endregion
}