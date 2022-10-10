using Fosscord.DbModel;
using Fosscord.Shared.Attributes;
using Fosscord.Util;
using Microsoft.AspNetCore.Mvc;

namespace Fosscord.API.Controllers.API.Users.me;

[TokenAuthorization]
public class BillingController: Controller
{
    [HttpGet("/api/users/@me/billing/country-code")]
    public IActionResult CountryCode()
    {
        return Json(new
        {
            country_code = "US"
        });
    }
    
    [HttpGet("/api/users/@me/billing/payment-sources")]
    public IActionResult PaymentSources()
    {
        return Json(new List<object>());
    }
    
    [HttpGet("/api/users/@me/billing/subscriptions")]
    public IActionResult Subscriptions()
    {
        return Json(new List<object>());
    }
}