using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TransitNova.UI.Areas.CarrierArea.Controllers.Queries;

[AllowAnonymous]
[Area("CarrierArea")]
[Route("[area]/[controller]/[action]")]
public sealed class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Carrier Dashboard";
        return View();
    }
}
