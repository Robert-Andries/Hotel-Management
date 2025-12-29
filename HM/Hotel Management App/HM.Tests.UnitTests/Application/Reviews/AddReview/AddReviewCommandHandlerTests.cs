using FluentAssertions;
using HM.Application.Abstractions.Data;
using HM.Application.Reviews.AddReview;
using HM.Domain.Abstractions;
using HM.Domain.Bookings.Entities;
using HM.Domain.Bookings.Value_Objects;
using HM.Domain.Reviews;
using HM.Domain.Reviews.Abstractions;
using HM.Domain.Reviews.Entities;
using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Rooms;
using HM.Domain.Rooms.Entities;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Domain.Users;
using HM.Domain.Users.Entities;
using HM.Domain.Users.Value_Objects;
using HM.Tests.UnitTests.Helpers;
using Moq;
using Xunit;

namespace HM.Tests.UnitTests.Application.Reviews.AddReview;

public class AddReviewCommandHandlerTests
{
    private static readonly Name TestName = new("John", "Doe");

    private static readonly ContactInfo TestContactInfo = new(
        Email.Create("test@test.com").Value,
        PhoneNumber.Create("123456789", "+1").Value);

    private static readonly DateOnly TestDateOfBirth = new(2000, 1, 1);
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly AddReviewCommandHandler _handler;
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<ITime> _timeMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AddReviewCommandHandlerTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _contextMock = new Mock<IApplicationDbContext>();
        _timeMock = new Mock<ITime>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new AddReviewCommandHandler(
            _timeMock.Object,
            _contextMock.Object,
            _unitOfWorkMock.Object,
            _reviewRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CommandIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var command = new AddReviewCommand(roomId, userId, new Comment("Title", "Content"), 5);

        _timeMock.Setup(x => x.NowUtc).Returns(DateTime.UtcNow);

        var users = new List<User>
            { User.Create(TestName, TestContactInfo, TestDateOfBirth, DateOnly.FromDateTime(DateTime.Today)).Value };
        var user = users[0];
        command = command with { UserId = user.Id };

        var rooms = new List<Room>
        {
            Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(), new Money(10, Currency.Usd))
                .Value
        };
        var room = rooms[0];
        command = command with { RoomId = room.Id };

        var booking = Booking.Reserve(room.Id, user.Id,
            DateRange.Create(DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
                DateOnly.FromDateTime(DateTime.Today.AddDays(-2))).Value, DateTime.UtcNow,
            new Money(100, Currency.Usd));

        // booking.Confirm(DateTime.UtcNow); // Method does not exist
        booking.CheckIn(DateTime.UtcNow);
        booking.CheckIn(DateTime.UtcNow);
        booking.CheckOut(DateTime.UtcNow);

        var bookings = new List<Booking> { booking };

        _contextMock.Setup(c => c.Users).Returns(MockDbSetHelper.GetQueryableMockDbSet(users));
        _contextMock.Setup(c => c.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(rooms));
        _contextMock.Setup(c => c.Bookings).Returns(MockDbSetHelper.GetQueryableMockDbSet(bookings));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _reviewRepositoryMock.Verify(x => x.AddReview(It.IsAny<RoomReview>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        var command = new AddReviewCommand(Guid.NewGuid(), Guid.NewGuid(), new Comment("T", "C"), 5);
        _contextMock.Setup(c => c.Users).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<User>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoomNotFound()
    {
        // Arrange
        var user = User.Create(TestName, TestContactInfo, TestDateOfBirth, DateOnly.FromDateTime(DateTime.Today)).Value;
        var command = new AddReviewCommand(Guid.NewGuid(), user.Id, new Comment("T", "C"), 5);

        _contextMock.Setup(c => c.Users).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<User> { user }));
        _contextMock.Setup(c => c.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<Room>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(RoomErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoBookingsExist()
    {
        // Arrange
        var user = User.Create(TestName, TestContactInfo, TestDateOfBirth, DateOnly.FromDateTime(DateTime.Today)).Value;
        var room = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(),
            new Money(10, Currency.Usd)).Value;
        var command = new AddReviewCommand(room.Id, user.Id, new Comment("T", "C"), 5);

        _contextMock.Setup(c => c.Users).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<User> { user }));
        _contextMock.Setup(c => c.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<Room> { room }));
        _contextMock.Setup(c => c.Bookings).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<Booking>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(ReviewErrors.NotBeenInRoom);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BookingNotCompleted()
    {
        // Arrange
        var user = User.Create(TestName, TestContactInfo, TestDateOfBirth, DateOnly.FromDateTime(DateTime.Today)).Value;
        var room = Room.Create(RoomType.Single, new RoomLocation(1, 101), new List<Feature>(),
            new Money(10, Currency.Usd)).Value;
        var command = new AddReviewCommand(room.Id, user.Id, new Comment("T", "C"), 5);

        var booking = Booking.Reserve(room.Id, user.Id,
            DateRange.Create(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
                .Value, DateTime.UtcNow, new Money(100, Currency.Usd));

        _contextMock.Setup(c => c.Users).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<User> { user }));
        _contextMock.Setup(c => c.Rooms).Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<Room> { room }));
        _contextMock.Setup(c => c.Bookings)
            .Returns(MockDbSetHelper.GetQueryableMockDbSet(new List<Booking> { booking }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.Should().Be(ReviewErrors.BookingStatusNeedsToBeCompleted);
    }
}