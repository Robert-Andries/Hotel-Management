using HM.Domain.Bookings.Entities;
using HM.Domain.Rooms.Entities;
using HM.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Abstractions.Data;

/// <summary>
///     Represents the application's database context interface, exposing domain entity sets.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>Gets the Bookings set.</summary>
    DbSet<Booking> Bookings { get; }

    /// <summary>Gets the Rooms set.</summary>
    DbSet<Room> Rooms { get; }

    /// <summary>Gets the Users set.</summary>
    DbSet<User> Users { get; }
}