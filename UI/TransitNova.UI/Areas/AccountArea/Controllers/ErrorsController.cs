using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.UI.Infrastructure.Mvc.Common;

namespace TransitNova.UI.Areas.AccountArea.Controllers;

[AllowAnonymous]
[Area("AccountArea")]
[Route("[area]/[controller]/[action]")]
public sealed class ErrorsController : AppControllerBase
{
    [HttpGet]
    public new IActionResult StatusCode(int statusCode) => statusCode switch
    {
        StatusCodes.Status404NotFound => View("NotFound"),
        StatusCodes.Status500InternalServerError => View("ServerError"),
        _ => statusCode >= StatusCodes.Status500InternalServerError ? View("ServerError") : View("NotFound")
    };

    [HttpGet]
    public new IActionResult NotFound() => View();

    [HttpGet]
    public IActionResult ServerError() => View();
}