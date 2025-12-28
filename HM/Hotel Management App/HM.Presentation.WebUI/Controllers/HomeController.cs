using Microsoft.AspNetCore.Mvc;

namespace HM.Presentation.WebUI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult TermsAndConditions()
    {
        return View();
    }

    public IActionResult AboutUs()
    {
        return View();
    }
}