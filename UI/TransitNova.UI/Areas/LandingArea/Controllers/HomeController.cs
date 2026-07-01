using Microsoft.AspNetCore.Mvc;

namespace TransitNova.UI.Areas.LandingArea.Controllers;

[Area("LandingArea")]
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
}
