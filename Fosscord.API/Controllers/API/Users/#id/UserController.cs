using Fosscord.DbModel;
using Fosscord.DbModel.Scaffold;
using Fosscord.Util;
using Fosscord.Util.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fosscord.API.Controllers.API.Users._id;

[Route("/users/{id}")]
[TokenAuthorization]
public class UserController: Controller
{
    private readonly Db _db;

    public UserController(Db db)
    {
        _db = db;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(string id)
    {
        var user = _db.Users.FirstOrDefault(s => s.Id == id);
        if (user == null)
            return NotFound();
        return Json(user.AsPublicUser());
    }

    [HttpGet]
    public async Task<IActionResult>  Profile(string id, [FromQuery] bool with_mutual_guilds = false)
    {
        var user = _db.Users.FirstOrDefault(s => s.Id == id);
        var invoker = _db.Users.FirstOrDefault(s => s.Id == HttpContext.User.Identity.Name);
        if (invoker.Bot) //maybe config option to allow bots to fetch profile?
            return Unauthorized();
        if (user == null)
            return NotFound();

        List<UserProfile.PublicConnectedAccount> accounts = _db.ConnectedAccounts.Where(s => s.User.Id == id && s.Visibility == 1).ToList().Select(s => s.AsPublicConnection()).ToList();
        List<Member> membersUser = _db.Members.Where(s => s.Id == user.Id).ToList();
        List<Member> membersInvoker = _db.Members.Include(s => s.Guild).Where(s => s.Id == invoker.Id).ToList();
        List<UserProfile.MutualGuild> mutualGuilds = new List<UserProfile.MutualGuild>();
        foreach (var member in membersInvoker)
        {
            if (membersUser.Any(s => s.GuildId == member.GuildId))
                mutualGuilds.Add(member.AsMutualGuild());
        }

        if (mutualGuilds.Count == 0)
            return NotFound(); //pretend the user doesnt exist if we dont share a mutual guild, discord does this
        if (with_mutual_guilds)
        {
            return Json(new UserProfile.UserProfileResponseMutual()
            {
                user = user.AsPublicUser(),
                connected_accounts = accounts,
                mutual_guilds = mutualGuilds,
                premium_guild_since = null, //todo
                premium_since = null, //todo
            });
        }
        else
        {
            return Json(new UserProfile.UserProfileResponseNoMutual()
            {
                user = user.AsPublicUser(),
                connected_accounts = accounts,
                premium_guild_since = null, //todo
                premium_since = null, //todo
            });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult>  Relationships(string id)
    {
        var user = _db.Users.FirstOrDefault(s => s.Id == id);
        var invoker = _db.Users.FirstOrDefault(s => s.Id == HttpContext.User.Identity.Name);
        if (invoker.Bot) //bots cant have friends
            return Unauthorized();
        if (user == null)
            return NotFound();

        List<Member> membersUser = _db.Members.Where(s => s.Id == user.Id).ToList();
        List<Member> membersInvoker = _db.Members.Include(s => s.Guild).Where(s => s.Id == invoker.Id).ToList();
        List<UserProfile.MutualGuild> mutualGuilds = new List<UserProfile.MutualGuild>();
        foreach (var member in membersInvoker)
        {
            if (membersUser.Any(s => s.GuildId == member.GuildId))
                mutualGuilds.Add(member.AsMutualGuild());
        }

        if (mutualGuilds.Count == 0)
            return NotFound(); //pretend the user doesnt exist if we dont share a mutual guild, discord does this
        
        List<Relationship> friendsMember = _db.Relationships.Where(s => s.FromId == user.Id && s.Type == 1).ToList();
        List<Relationship> friendsInvoker = _db.Relationships.Include(s => s.To).Where(s => s.FromId == invoker.Id && s.Type == 1).ToList();
        List<PublicUser> mutualFriends = new List<PublicUser>();

        foreach (var friend in friendsInvoker)
        {
            if (friendsMember.Any(s => s.ToId == friend.ToId))
                mutualFriends.Add(friend.To.AsPublicUser());
        }

        return Json(mutualFriends);
    }
}