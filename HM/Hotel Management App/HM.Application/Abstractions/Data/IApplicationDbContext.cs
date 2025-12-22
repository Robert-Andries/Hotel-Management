using HM.Domain.Bookings.Entities;
using HM.Domain.Rooms.Entities;
using HM.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Booking> Bookings { get; }
    DbSet<Room> Rooms { get; }
    DbSet<User> Users { get; }
}