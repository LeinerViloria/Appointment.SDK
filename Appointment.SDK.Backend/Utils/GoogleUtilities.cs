
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Appointment.SDK.Backend.Utilities;

public static class GoogleUtilities
{
    public static ChallengeResult ExternalSignIn(this ControllerBase Controller, string RedirectUri)
    {
        var props = new AuthenticationProperties { RedirectUri = RedirectUri };
        return Controller.Challenge(props, GoogleDefaults.AuthenticationScheme);
    }
}