using Microsoft.AspNetCore.Mvc;

namespace HM.Presentation.WebUI.Controllers;

public class BookingController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}