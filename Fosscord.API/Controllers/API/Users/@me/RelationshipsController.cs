using System.Linq;
using Fosscord.DbModel;
using Fosscord.DbModel.Scaffold;
using Fosscord.Shared.Attributes;
using Fosscord.Util;
using Fosscord.Util.Models;
using Fosscord.Util.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fosscord.API.Controllers.API.Users.me;

[Route("/api/users/@me/relationships")]
[TokenAuthorization]
public class RelationshipsController: Controller
{
    private readonly Db _db;

    public RelationshipsController()
    {
        _db = Db.GetNewDb();
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = _db.Users.FirstOrDefault(s => s.Id == HttpContext.User.Identity.Name);
        return Json(_db.Relationships.Include(s => s.To).Where(s => s.FromId == user.Id).ToList().Select(s => s.AsPublicRelationShip()));
    }
}