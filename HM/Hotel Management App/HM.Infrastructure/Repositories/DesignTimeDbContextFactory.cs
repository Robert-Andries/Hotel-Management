using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HM.Infrastructure.Repositories;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        // Note: We assume the command is run from the startup project directory (HM.Presentation.WPF)
        // or we explicitly point to it.
        // We need to find the path to HM.Presentation.WPF to load secrets correctly if running from Infrastructure,
        // but usually we run from the startup project.

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddUserSecrets("d4c27cd9-b98a-46cd-9fa1-29214f08ab4f") // UserSecretsId from HM.Presentation.WPF
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("Database");

        if (string.IsNullOrEmpty(connectionString))
            // Fallback or throw if connection string is missing. 
            // For now, let's assume it's there or throw a clear error.
            throw new InvalidOperationException("Connection string 'Database' not found.");

        builder.UseSqlServer(connectionString);

        return new ApplicationDbContext(builder.Options, new NullPublisher());
    }
}