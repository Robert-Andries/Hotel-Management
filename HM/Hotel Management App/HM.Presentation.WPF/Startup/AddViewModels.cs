using HM.Presentation.WPF.Stores;
using HM.Presentation.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Presentation.WPF.Startup;

internal static class AddViewModels
{
    /// <summary>
    /// Adds the necessary view models and navigation store for quiz functionality to the service collection.
    /// </summary>
    internal static IServiceCollection AddWpfViewModels(this IServiceCollection service)
    {
        service.AddScoped<INavigationStore, NavigationStore>();
        service.AddScoped<MainViewModel>();
        service.AddScoped<BookingViewModel>();
        service.AddScoped<RoomViewModel>();
        
        return service;
    }
}