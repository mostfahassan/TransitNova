using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TransitNova.UI.Areas.WarehouseManagerArea.Controllers;

[AllowAnonymous]
[Area("WarehouseManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Warehouse Manager Dashboard";
        return View();
    }
}
