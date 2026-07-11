using Microsoft.AspNetCore.Mvc;

namespace TransitNova.UI.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index() =>
        RedirectToAction("Index", "Home", new { area = "LandingArea" });

    public IActionResult Privacy() =>
        RedirectToAction("Privacy", "Home", new { area = "LandingArea" });
}
