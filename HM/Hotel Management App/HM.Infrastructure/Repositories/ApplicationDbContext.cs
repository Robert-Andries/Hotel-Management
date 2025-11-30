using HM.Application.Abstractions.Data;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Rooms.Entities;
using HM.Domain.Users.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HM.Infrastructure.Repositories;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork, IApplicationDbContext
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Publish domain events BEFORE saving changes to ensure atomicity
        // Note: This assumes handlers use the same DbContext instance and don't call SaveChangesAsync themselves
        await PublishDomainEventsAsync();

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}