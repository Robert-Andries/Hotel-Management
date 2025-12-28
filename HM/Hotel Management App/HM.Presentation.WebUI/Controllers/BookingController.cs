using HM.Application.Bookings.CreateBookingForGuest;
using HM.Application.Rooms.FindAvailableRoom;
using HM.Presentation.WebUI.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HM.Presentation.WebUI.Controllers;

public class BookingController : Controller
{
    private readonly IMediator _mediator;

    public BookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET
    public IActionResult Index()
    {
        return View(new SearchRoomViewModel
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
        });
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> Search(SearchRoomViewModel model)
    {
        if (!ModelState.IsValid) return View("Index", model);

        var query = new FindAvailableRoomQuery(
            model.StartDate,
            model.EndDate,
            model.RoomType,
            model.SelectedFeatures);

        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Name);
            return View("Index", model);
        }

        // Map successful search to Confirm view model
        var confirmModel = new ConfirmBookingViewModel
        {
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            RoomType = model.RoomType,
            RoomId = result.Value.RoomId,
            PricePerNightAmount = result.Value.PricePerNight.Amount,
            TotalPriceAmount = result.Value.TotalPrice.Amount,
            Currency = result.Value.Currency,
            Features = result.Value.Features,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            CountryCode = model.CountryCode,
            PhoneNumber = model.PhoneNumber,
            DateOfBirth = model.DateOfBirth
        };

        return View("~/Views/Booking/Confirm.cshtml", confirmModel);
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> Create(ConfirmBookingViewModel model)
    {
        if (!ModelState.IsValid) return View("~/Views/Booking/Confirm.cshtml", model);

        // Create Booking (with implicit User creation if needed)
        var command = new CreateBookingForGuestCommand(
            model.FirstName,
            model.LastName,
            model.Email,
            model.PhoneNumber,
            model.CountryCode,
            model.DateOfBirth,
            model.StartDate,
            model.EndDate,
            model.RoomId);

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            // Logic error (e.g. room overlap)
            return View("~/Views/Booking/Error.cshtml", result.Error.Name);

        // Success
        return View("~/Views/Booking/Success.cshtml", result.Value);
    }
}