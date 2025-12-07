using System.Collections.ObjectModel;
using System.Security.AccessControl;
using System.Windows.Input;
using HM.Application.Rooms.AddRoom;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Presentation.WPF.Models;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using MediatR;

namespace HM.Presentation.WPF.ViewModels;

public class AddRoomDialogViewModel : BaseViewModel, IDialogViewModel
{
    public AddRoomDialogViewModel(INavigationStore navigationStore, IMediator mediator) : base(navigationStore)
    {
        InitializeFeatures();
        _mediator = mediator;
        SaveCommand = new DelegateCommand(async void () => await SaveExecute(), CanSaveExecute);
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

    #region Execute

    private bool CanSaveExecute()
    {
        bool isFloorValid = Floor >= 0;
        bool isRoomValid = RoomNumber > 0;
        bool isPriceValid = PriceAmount > 0;
        bool isCurrencyValid = SelectedCurrency != null;

        return isFloorValid && isRoomValid && isPriceValid && isCurrencyValid;
    }

    private async Task SaveExecute()
    {
        RoomLocation selectedLocation = new(Floor, RoomNumber);
        var selectedFeautres = GetSelectedFeatures();
        var selectedPrice = new Money(PriceAmount, SelectedCurrency!);

        var addRoomCommand = new AddRoomCommand(SelectedRoomType, selectedLocation, selectedFeautres, selectedPrice);
        var result = await _mediator.Send(addRoomCommand);

        if (result.IsFailure)
        {
            ErrorMessage = result.Error.Name;
            return;
        }

        RequestClose?.Invoke(true);
    }

    private void CancelExecute()
    {
        RequestClose?.Invoke(false);
    }

    #endregion

    #region PrivateFields

    private readonly IMediator _mediator;
    private Currency? _selectedCurrency;
    private RoomType _selectedRoomType;
    private string _errorMessage = string.Empty;
    private decimal _priceAmount;
    private int _floor;
    private int _roomNumber;

    #endregion
}