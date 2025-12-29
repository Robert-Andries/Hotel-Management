using FluentAssertions;
using HM.Application.Bookings.AddBooking;
using HM.Application.Bookings.CheckInGuest;
using HM.Application.Bookings.CheckOutGuest;
using HM.Application.Reviews.AddReview;
using HM.Application.Rooms.AddRoom;
using HM.Application.Users.AddUser;
using HM.Domain.Reviews;
using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.IntegrationTests.Extensions;
using HM.Tests.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HM.Tests.IntegrationTests.Application.Reviews;

public class AddReviewTests : BaseIntegrationTest
{
    public AddReviewTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_AddReview_WhenBookingIsCompleted()
    {
        // Arrange
        var addRoomCommand = new AddRoomCommand(
            RoomType.Single,
            new RoomLocation(6, 601),
            new List<Feature>(),
            new Money(80, Currency.Usd));
        await Sender.Send(addRoomCommand);
        var room = await DbContext.Rooms.FirstAsync(r => r.Location.RoomNumber == 601);

        var addUserCommand = new AddUserCommand(
            "Dave",
            "Reviewer",
            "111222333",
            "+1",
            "dave@test.com",
            new DateOnly(1985, 5, 5));
        var userResult = await Sender.Send(addUserCommand);

        var addBookingCommand = new AddBookingCommand(
            userResult.Value,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            room.Id);
        var bookingResult = await Sender.Send(addBookingCommand);

        await Sender.Send(new CheckInGuestCommand(bookingResult.Value));
        await Sender.Send(new CheckOutGuestCommand(bookingResult.Value));

        // Act

        var reviewCommand = new AddReviewCommand(
            room.Id,
            userResult.Value,
            new Comment("Awesome", "Great stay!"),
            5);

        var result = await Sender.Send(reviewCommand);

        // 3. Assert
        result.ShouldBeSuccess();

        var review =
            await DbContext.Reviews.FirstOrDefaultAsync(r => r.RoomId == room.Id && r.UserId == userResult.Value);
        review.Should().NotBeNull();
        review.RoomId.Should().Be(room.Id);
        review.UserId.Should().Be(userResult.Value);
        review.Rating.Should().Be(5);
        review.Comment.Title.Should().Be("Awesome");
        review.Comment.Content.Should().Be("Great stay!");
    }

    [Fact]
    public async Task Should_ReturnFailure_When_BookingNotCompleted()
    {
        // Arrange
        var addRoomCommand = new AddRoomCommand(
            RoomType.Single,
            new RoomLocation(6, 602),
            new List<Feature>(),
            new Money(80, Currency.Usd));
        await Sender.Send(addRoomCommand);
        var room = await DbContext.Rooms.FirstAsync(r => r.Location.RoomNumber == 602);

        var addUserCommand = new AddUserCommand(
            "Kevin",
            "Reviewer",
            "111222999",
            "+1",
            "kevin@test.com",
            new DateOnly(1990, 1, 1));
        var userResult = await Sender.Send(addUserCommand);

        var addBookingCommand = new AddBookingCommand(
            userResult.Value,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            room.Id);
        await Sender.Send(addBookingCommand);

        // Act
        var reviewCommand = new AddReviewCommand(
            room.Id,
            userResult.Value,
            new Comment("Premature", "Early review"),
            1);

        var result = await Sender.Send(reviewCommand);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReviewErrors.BookingStatusNeedsToBeCompleted);
    }
}