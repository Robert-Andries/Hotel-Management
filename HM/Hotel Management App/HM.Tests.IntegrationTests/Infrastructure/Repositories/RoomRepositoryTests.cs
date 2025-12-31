using FluentAssertions;
using HM.Domain.Rooms.Abstractions;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HM.Tests.IntegrationTests.Infrastructure.Repositories;

public class RoomRepositoryTests : BaseIntegrationTest
{
    private readonly IRoomRepository _roomRepository;

    public RoomRepositoryTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _roomRepository = ServiceProvider.GetRequiredService<IRoomRepository>();
    }

    [Fact]
    public async Task AddAsync_ShouldPersistRoom_WhenRoomIsValid()
    {
        // Arrange
        var room = Room.Create(
            RoomType.Single,
            new RoomLocation(7, 701),
            new List<Feature>(),
            new Money(50, Currency.Usd)).Value;

        // Act
        var result = await _roomRepository.AddAsync(room);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var fromDb = await DbContext.Rooms.FirstOrDefaultAsync(r => r.Location.RoomNumber == 701);
        fromDb.Should().NotBeNull();
        fromDb.Id.Should().Be(room.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRoom_WhenRoomExists()
    {
        // Arrange
        var room = Room.Create(
            RoomType.Double,
            new RoomLocation(7, 702),
            new List<Feature>(),
            new Money(100, Currency.Usd)).Value;

        await _roomRepository.AddAsync(room);

        // Act
        var result = await _roomRepository.GetByIdAsync(room.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(room.Id);
    }
}