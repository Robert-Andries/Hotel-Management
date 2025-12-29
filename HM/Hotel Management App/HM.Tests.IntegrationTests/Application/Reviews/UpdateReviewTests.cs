using FluentAssertions;
using HM.Application.Bookings.AddBooking;
using HM.Application.Bookings.CheckInGuest;
using HM.Application.Bookings.CheckOutGuest;
using HM.Application.Reviews.AddReview;
using HM.Application.Reviews.UpdateReview;
using HM.Application.Rooms.AddRoom;
using HM.Application.Users.AddUser;
using HM.Domain.Reviews.Value_Objects;
using HM.Domain.Rooms.Value_Objects;
using HM.Domain.Shared;
using HM.Tests.IntegrationTests.Extensions;
using HM.Tests.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HM.Tests.IntegrationTests.Application.Reviews;

public class UpdateReviewTests : BaseIntegrationTest
{
    public UpdateReviewTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_UpdateReview_WhenReviewExists()
    {
        // Arrange
        var addRoomCommand = new AddRoomCommand(
            RoomType.Double,
            new RoomLocation(7, 701),
            new List<Feature>(),
            new Money(90, Currency.Usd));
        await Sender.Send(addRoomCommand);
        var room = await DbContext.Rooms.FirstAsync();

        var addUserCommand = new AddUserCommand(
            "Eve",
            "Updater",
            "444555666",
            "+1",
            "eve@test.com",
            new DateOnly(1999, 9, 9));
        var userResult = await Sender.Send(addUserCommand);

        var addBookingCommand = new AddBookingCommand(
            userResult.Value,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            room.Id);
        var bookingResult = await Sender.Send(addBookingCommand);

        await Sender.Send(new CheckInGuestCommand(bookingResult.Value));
        await Sender.Send(new CheckOutGuestCommand(bookingResult.Value));

        var addReviewCommand = new AddReviewCommand(
            room.Id,
            userResult.Value,
            new Comment("OK", "Good."),
            3);
        await Sender.Send(addReviewCommand);
        var review = await DbContext.Reviews.FirstAsync();

        // Act
        var updateCommand = new UpdateReviewCommand(
            review.Id,
            5,
            new Comment("Amazing", "Actually it was amazing!"));

        var result = await Sender.Send(updateCommand);

        // Assert
        result.ShouldBeSuccess();

        var updatedReview = await DbContext.Reviews.FindAsync(review.Id);
        updatedReview.Should().NotBeNull();
        updatedReview!.Rating.Should().Be(5);
        updatedReview.Comment.Title.Should().Be("Amazing");
        updatedReview.Comment.Content.Should().Be("Actually it was amazing!");
    }
}