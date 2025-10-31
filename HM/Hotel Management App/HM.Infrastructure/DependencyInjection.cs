using HM.Domain.Abstractions;
using HM.Infrastructure.DateTimeProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ITime, Time>();
        
        return services;
    }
}