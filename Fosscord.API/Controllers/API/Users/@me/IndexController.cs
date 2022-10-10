using Fosscord.DbModel;
using Fosscord.Shared.Attributes;
using Fosscord.Util;
using Fosscord.Util.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fosscord.API.Controllers.API.Users.me;

[TokenAuthorization]
public class IndexController: Controller
{
    private readonly Db _db;

    public IndexController()
    {
        _db = Db.GetNewDb();
    }
    
    [HttpGet("/api/users/@me")]
    public IActionResult CountryCode()
    {
        var user = _db.Users.FirstOrDefault(s => s.Id == HttpContext.User.Identity.Name);
        return Json(user.AsPrivateUser());
    }
}