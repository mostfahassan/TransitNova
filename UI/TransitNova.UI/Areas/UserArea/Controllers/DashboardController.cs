using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[AllowAnonymous]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Customer Dashboard";
        return View();
    }
}
