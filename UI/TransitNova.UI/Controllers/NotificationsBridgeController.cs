using Microsoft.AspNetCore.Mvc;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer.Common.APIHelper;

namespace TransitNova.UI.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("notifications/bridge")]
public sealed class NotificationsBridgeController(IUiAuthSessionService authSession) : ControllerBase
{
    [HttpGet("session")]
    public async Task<IActionResult> Session(CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
            return Unauthorized();

        var accessToken = await authSession.GetValidAccessTokenAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(accessToken))
            return Unauthorized();

        return Ok(new NotificationBridgeSessionResponse(
            ApiHelper.PublicBaseUrl.TrimEnd('/'),
            accessToken));
    }
}

internal sealed record NotificationBridgeSessionResponse(string ApiBaseUrl, string AccessToken);
