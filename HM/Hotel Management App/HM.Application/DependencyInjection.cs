using HM.Application.Users.Services;
using HM.Domain.Bookings.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Application;

/// <summary>
///     Adds necessary services to the IServiceCollection for the application layer to work correctly.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services)
    {
        services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly); });
        services.AddTransient<IPricingService, PricingService>();
        services.AddTransient<IUserCreationService, UserCreationService>();
        services.AddLogging();

        return services;
    }
}