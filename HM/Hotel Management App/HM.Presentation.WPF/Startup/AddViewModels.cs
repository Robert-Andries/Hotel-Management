using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.ViewModels;
using HM.Presentation.WPF.ViewModels.Bookings;
using HM.Presentation.WPF.ViewModels.Bookings.Dialogs;
using HM.Presentation.WPF.ViewModels.Rooms;
using HM.Presentation.WPF.ViewModels.Rooms.Dialogs;
using HM.Presentation.WPF.Views.Bookings.Dialogs;
using HM.Presentation.WPF.Views.Rooms.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using IDialogService = HM.Presentation.WPF.Services.IDialogService;

namespace HM.Presentation.WPF.Startup;

internal static class AddViewModels
{
    /// <summary>
    ///     Adds the necessary view models, navigation store and maps
    ///     DialogViewModel with DialogView for quiz functionality to the service collection.
    /// </summary>
    internal static IServiceCollection AddWpfViewModels(this IServiceCollection service,
        Func<Type, BaseViewModel> GetViewModel)
    {
        var dialogService = new DialogService(GetViewModel);

        service.AddScoped<INavigationStore, NavigationStore>();
        service.AddScoped<MainViewModel>();
        service.AddScoped<BookingViewModel>();
        service.AddScoped<RoomViewModel>();
        
        service.AddScoped<AddRoomDialogViewModel>();
        service.AddScoped<EditRoomDialogViewModel>();
        service.AddScoped<AddBookingDialogViewModel>();
        service.AddScoped<EditBookingDialogViewModel>();

        dialogService.Register<AddRoomDialogViewModel, AddRoomDialogView>();
        dialogService.Register<EditRoomDialogViewModel, EditRoomDialogView>();
        dialogService.Register<AddBookingDialogViewModel, AddBookingDialogView>();
        dialogService.Register<EditBookingDialogViewModel, EditBookingDialogView>();

        service.AddSingleton<IDialogService>(dialogService);

        return service;
    }
}