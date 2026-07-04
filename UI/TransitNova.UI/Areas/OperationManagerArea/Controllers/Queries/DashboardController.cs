using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TransitNova.UI.Areas.OperationManagerArea.Controllers.Queries;

[AllowAnonymous]
[Area("OperationManagerArea")]
[Route("[area]/[controller]/[action]")]
public sealed class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Operation Manager Dashboard";
        return View();
    }
}
