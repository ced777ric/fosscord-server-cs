using Fosscord.API.Helpers;
using Fosscord.DbModel;
using Microsoft.AspNetCore.Mvc;

namespace Fosscord.API.Controllers;

[Controller]
[Route("/")]
public class FrontendController : Controller
{
    private readonly Db _db;

    public FrontendController(Db db)
    {
        _db = Db.GetNewDb();
    }

    [HttpGet]
    [HttpGet("/app")]
    [HttpGet("/register")]
    [HttpGet("/channels/{*channel}")]
    public async Task<object> Home()
    {
        return Redirect("/login");
    }
    
    [HttpGet("/login")]
    public async Task<object> Login()
    {
        if (FosscordConfig.GetBool("client_testClient_debug", false))
            return Resolvers.ReturnFileWithVars("Resources/Pages/index-dbg.html", _db);
        if (FosscordConfig.GetBool("client_testClient_latest", false))
            return Resolvers.ReturnFileWithVars("Resources/Pages/index-updated.html", _db);
        return Resolvers.ReturnFileWithVars("Resources/Pages/index.html", _db);
    }
    
    [HttpGet("/developers")]
    public async Task<object> Developers()
    {
        return Resolvers.ReturnFileWithVars("Resources/Pages/developers.html", _db);
    }
}