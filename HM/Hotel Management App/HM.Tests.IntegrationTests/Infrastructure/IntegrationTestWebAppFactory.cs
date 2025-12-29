using HM.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HM.Tests.IntegrationTests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveAll(typeof(DbContextOptions));
            services.RemoveAll(typeof(ApplicationDbContext));

            // Remove the configuration that calls UseSqlServer
            var contextOptionsConfig = services.FirstOrDefault(d =>
                d.ServiceType.Name == "IDbContextOptionsConfiguration`1" &&
                d.ServiceType.GenericTypeArguments.Length == 1 &&
                d.ServiceType.GenericTypeArguments[0] == typeof(ApplicationDbContext));

            if (contextOptionsConfig != null) services.Remove(contextOptionsConfig);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
        });
    }
}