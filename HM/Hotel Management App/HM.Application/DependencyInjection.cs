using HM.Domain.Bookings.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencyInjection(this IServiceCollection services)
    {
        services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly); });
        services.AddTransient<PricingService>();
        services.AddLogging();

        return services;
    }
}