using Fosscord.DbModel;
using Fosscord.Shared.Attributes;
using Fosscord.Util;
using Microsoft.AspNetCore.Mvc;

namespace Fosscord.API.Controllers.API.Users.me;

[TokenAuthorization]
public class LibraryController: Controller
{
   [HttpGet("/api/users/@me/library")]
    public IActionResult Library()
    {
        return Json(new List<object>());
    }
}