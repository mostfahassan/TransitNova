using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.UI.Infrastructure.Mvc;

namespace TransitNova.UI.Areas.AccountArea.Controllers;

[AllowAnonymous]
[Area("AccountArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ErrorsController : AppControllerBase
{
    [HttpGet]
    public new IActionResult NotFound() => View();

    [HttpGet]
    public IActionResult ServerError() => View();
}

