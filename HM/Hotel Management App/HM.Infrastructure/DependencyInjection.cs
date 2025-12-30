using HM.Application.Abstractions.Data;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Abstractions;
using HM.Domain.Bookings.Services;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Users.Abstractions;
using HM.Infrastructure.DateTimeProvider;
using HM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Infrastructure;

/// <summary>
///     Adds necessary services to the IServiceCollection for the infrastructure layer to work correctly.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ITime, Time>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        return services;
    }
}