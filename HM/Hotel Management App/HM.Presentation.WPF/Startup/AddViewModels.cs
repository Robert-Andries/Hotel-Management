using HM.Presentation.WPF.Services;
using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.ViewModels;
using HM.Presentation.WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Presentation.WPF.Startup;

internal static class AddViewModels
{
    /// <summary>
    /// Adds the necessary view models, navigation store and maps
    /// DialogViewModel with DialogView for quiz functionality to the service collection.
    /// </summary>
    internal static IServiceCollection AddWpfViewModels(this IServiceCollection service, Func<Type, BaseViewModel> GetViewModel)
    {
        var dialogService = new DialogService(GetViewModel);
        
        service.AddScoped<INavigationStore, NavigationStore>();
        service.AddScoped<MainViewModel>();
        service.AddScoped<BookingViewModel>();
        service.AddScoped<RoomViewModel>();
        service.AddScoped<AddRoomDialogViewModel>();
        service.AddScoped<EditRoomDialogViewModel>();
        
        dialogService.Register<AddRoomDialogViewModel, AddRoomDialogView>();
        dialogService.Register<EditRoomDialogViewModel, EditRoomDialogView>();
        
        service.AddSingleton<Services.IDialogService>(dialogService);
        
        return service;
    }
}