using System.Collections.ObjectModel;
using System.Windows.Input;
using HM.Application.Users.GetUsers;
using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.Utilities;
using HM.Presentation.WPF.ViewModels.Clients.Dialogs;
using MediatR;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HM.Presentation.WPF.ViewModels.Clients;

public class ClientViewModel : BaseViewModel
{
    public ClientViewModel(INavigationStore navigationStore, ILogger<ClientViewModel> logger,
        IDialogService dialogService, IMediator mediator, ClientDetailsDialogViewModel clientDetailsDialogViewModel)
        : base(navigationStore)
    {
        _logger = logger;
        _dialogService = dialogService;
        _mediator = mediator;
        _clientDetailsDialogViewModel = clientDetailsDialogViewModel;

        ShowDetailsCommand = new RelayCommand(ShowDetailsExecute, ShowDetailsCanExecute);
        RefreshCommand = new AsyncRelayCommand(RefreshExecute, null, OnException);
        NextPageCommand = new AsyncRelayCommand(NextPageExecute, NextPageCanExecute, OnException);
        PreviousPageCommand = new AsyncRelayCommand(PreviousPageExecute, PreviousPageCanExecute, OnException);
        AddClientCommand = new AsyncRelayCommand(AddClientExecute, null, OnException);
    }

    #region Properties

    public ObservableCollection<UserResponse> Users { get; set; } = new();

    public UserResponse? SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (Equals(value, _selectedUser)) return;
            _selectedUser = value;
            OnPropertyChanged();
        }
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (value == _searchTerm) return;
            _searchTerm = value;
            OnPropertyChanged();
        }
    }

    public int Page
    {
        get => _page;
        private set
        {
            if (value == _page) return;
            _page = value;
            OnPropertyChanged();
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value == _pageSize) return;
            _pageSize = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ICommand RefreshCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand ShowDetailsCommand { get; }
    public ICommand AddClientCommand { get; }

    #endregion

    #region Methods

    private void OnException(Exception ex)
    {
        _logger.LogError(ex, "Command execution failed");
    }

    private async Task LoadUsers()
    {
        _logger.LogInformation("Loading users... Page: {Page}, PageSize: {PageSize}, Search: {SearchTerm}", Page,
            PageSize, SearchTerm);

        var filter = new UserFilter(SearchTerm, Page, PageSize);
        var query = new GetUsersQuery(filter);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to load users: {Error}", result.Error);
            return;
        }

        Users.Clear();
        foreach (var user in result.Value) Users.Add(user);

        _logger.LogInformation("Loaded {Count} users", Users.Count);
    }

    #region CanExecute

    private bool NextPageCanExecute()
    {
        return Users.Count == PageSize;
    }

    private bool PreviousPageCanExecute()
    {
        return Page > 1;
    }

    private bool ShowDetailsCanExecute()
    {
        return SelectedUser != null;
    }

    #endregion

    #region Execute

    private async Task RefreshExecute()
    {
        Page = 1;
        await LoadUsers();
    }

    private async Task NextPageExecute()
    {
        Page++;
        await LoadUsers();
    }

    private async Task PreviousPageExecute()
    {
        Page--;
        await LoadUsers();
    }

    private void ShowDetailsExecute()
    {
        if (SelectedUser == null) return;

        _clientDetailsDialogViewModel.InitializeUser(SelectedUser);
        _dialogService.ShowDialog(_clientDetailsDialogViewModel);
    }

    private async Task AddClientExecute()
    {
        _logger.LogInformation("Opening Add Client Dialog...");
        var result = _dialogService.ShowDialog<AddClientDialogViewModel>();
        if (result == true)
        {
            _logger.LogInformation("Client added, refreshing list...");
            await RefreshExecute();
        }
    }

    #endregion

    #endregion

    #region Private Fields

    private readonly ILogger<ClientViewModel> _logger;
    private readonly IDialogService _dialogService;
    private readonly IMediator _mediator;
    private readonly ClientDetailsDialogViewModel _clientDetailsDialogViewModel;
    private UserResponse? _selectedUser;
    private string _searchTerm = string.Empty;
    private int _page = 1;
    private int _pageSize = 10;

    #endregion
}