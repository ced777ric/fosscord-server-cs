using Fosscord.Shared.Attributes;
using Fosscord.Util;
using Microsoft.AspNetCore.Mvc;

namespace Fosscord.API.Controllers.API.Users.me.Affinities;

[TokenAuthorization]
public class AffinitiesController : Controller
{
    [HttpGet("/api/users/@me/affinities/users")]
    public IActionResult Users()
    {
        return Json(new
        {
            user_affinities = new List<object>(),
            inverse_user_affinities = new List<object>(),
        });
    }
    
    [HttpGet("/api/users/@me/affinities/guilds")]
    public IActionResult Guilds()
    {
        return Json(new
        {
            guild_affinities = new List<object>(),
        });
    }
}