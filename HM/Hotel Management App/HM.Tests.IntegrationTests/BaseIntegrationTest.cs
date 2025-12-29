using HM.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Tests.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly ISender Sender;
    protected readonly IServiceProvider ServiceProvider;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        var scope = factory.Services.CreateScope();
        ServiceProvider = scope.ServiceProvider;

        Sender = scope.ServiceProvider.GetRequiredService<ISender>();

        DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}