using System.Linq;
using Fosscord.DbModel;
using Fosscord.Shared.Attributes;
using Fosscord.Util;
using Fosscord.Util.Models;
using Fosscord.Util.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fosscord.API.Controllers.API.Users.me;

[TokenAuthorization]
public class ChannelsController: Controller
{
    
    private readonly Db _db;

    public ChannelsController()
    {
        _db = Db.GetNewDb();
    }
    
    [HttpGet("/api/users/@me/channels")]
    public async Task<IActionResult> Index()
    {
        var user = _db.Users.FirstOrDefault(s => s.Id == HttpContext.User.Identity.Name);
        var dmChannels = _db.Channels.Include(s => s.Recipients).ThenInclude(s => s.User).Where(s => (s.Type == 1 || s.Type == 3) && s.Recipients.Any(s => s.Id == user.Id)).ToList();
        return Json(dmChannels);
    }
    
    [HttpPost("/api/users/@me/channels")]
    public async Task<IActionResult> Index([FromBody] DmChannelCreate model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
            {
                {
                    "name", new FieldError()
                    {
                        _errors = new List<FieldErrorInner>()
                        {
                            new FieldErrorInner()
                            {
                                code = "BASE_TYPE_REQUIRED",
                                message = "This field is required" //todo translations
                            }
                        }
                    }
                }
            }));
        }
        var user = _db.Users.FirstOrDefault(s => s.Id == HttpContext.User.Identity.Name);
        try
        {
            return Json(await Channel.CreateDMChannel(_db, model.recipients, user.Id.ToString(), model.name));
        }
        catch (Exception ex)
        {
            return BadRequest(new FieldValidationError(new Dictionary<string, FieldError>()
            {
                {
                    "data", new FieldError()
                    {
                        _errors = new List<FieldErrorInner>()
                        {
                            new FieldErrorInner()
                            {
                                code = "ERROR",
                                message = $"{ex}" //todo translations
                            }
                        }
                    }
                }
            }));
        }
        
        return Json("");
    }
}