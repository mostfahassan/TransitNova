using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Domain.Contracts.Roles;
 
 
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Pricing.Segregation;

namespace TransitNova.UI.Areas.UserArea.Controllers;

[Authorize(Roles = Role.User)]
[Area("UserArea")]
[Route("[area]/[controller]/[action]")]
public sealed class PricingController(
    IBackendApiInvoker apiInvoker,
    IUserPricingCommand userPricingCommand)
    : AppControllerBase
{
    [HttpGet]
    public IActionResult RateCalculator() => View(new RateCalculatorViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RateCalculator(RateCalculatorViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((_, ct) => userPricingCommand.CalculateRateAsync(model.ToDto(), ct), requiresAuthentication: false, cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        ViewData["CalculatedRate"] = response.Data;
        return View(model);
    }
}
