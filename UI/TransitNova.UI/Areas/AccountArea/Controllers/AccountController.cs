using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransitNova.UI.Infrastructure.Mvc;
using TransitNova.UI.ViewModels;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Segregation;

namespace TransitNova.UI.Areas.AccountArea.Controllers;

[Area("AccountArea")]
[Route("[area]/[controller]/[action]")]
public sealed class AccountController(
    IBackendApiInvoker apiInvoker,
    IUiAuthSessionService authSession,
    IRoleHomeResolver roleHomeResolver,
    IAuthenticationCommand authenticationCommand)
    : AppControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((_, ct) => authenticationCommand.LoginAsync(model.ToDto(), ct), requiresAuthentication: false, cancellationToken);

        if (response.IsFailure || response.Data is null)
        {
            AddApiErrors(response);
            return View(model);
        }

        await authSession.SignInAsync(response.Data, cancellationToken);
        return RedirectToLocalPath(roleHomeResolver.Resolve(response.Data.Roles.FirstOrDefault()));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((_, ct) => authenticationCommand.RegisterAsync(model.ToDto(), ct), requiresAuthentication: false, cancellationToken);

        if (response.IsFailure || response.Data is null)
        {
            AddApiErrors(response);
            return View(model);
        }

        await authSession.SignInAsync(response.Data, cancellationToken);
        return RedirectToLocalPath(roleHomeResolver.Resolve(response.Data.Roles.FirstOrDefault()));
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await apiInvoker.ExecuteAsync((token, ct) => authenticationCommand.ChangePasswordAsync(model.ToDto(), token!, ct), cancellationToken: cancellationToken);

        if (response.IsFailure)
        {
            AddApiErrors(response);
            return View(model);
        }

        Success("Password changed successfully.");
        return RedirectToAction(nameof(ChangePassword));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await apiInvoker.ExecuteAsync((token, ct) => authenticationCommand.SignOutAsync(token!, ct), cancellationToken: cancellationToken);

        await authSession.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied() => View();
}
