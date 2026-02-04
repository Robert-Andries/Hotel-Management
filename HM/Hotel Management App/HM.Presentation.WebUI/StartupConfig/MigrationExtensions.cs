using HM.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HM.Presentation.WebUI.StartupConfig;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Non relational dbs are not compatible with migration
        // e.g. of non realational db: InMemory db
        if (dbContext.Database.IsRelational()) dbContext.Database.Migrate();
    }
}