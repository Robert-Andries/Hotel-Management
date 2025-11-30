using HM.Domain.Bookings.Entities;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Booking> Bookings { get; }
}
