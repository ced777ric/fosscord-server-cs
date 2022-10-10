using Fosscord.API.Classes;
using Fosscord.API.PostData;
using Fosscord.DbModel;
using Fosscord.DbModel.Scaffold;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fosscord.API.Controllers.API.Auth;

//[Authorize]
[Controller]
[Route("/")]
public class AuthController : Controller
{
    private readonly Db _db;
    private readonly JwtAuthenticationManager _auth;
    private static readonly Random Rnd = new Random();
    
    public AuthController(Db db, JwtAuthenticationManager auth)
    {
        _db = db;
        _auth = auth;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <returns>Json object with token</returns>
    [HttpPost("/api/auth/register")]
    public async Task<object> Register()
    {
        var data = JsonConvert.DeserializeObject<RegisterData>(await new StreamReader(Request.Body).ReadToEndAsync());
        Console.WriteLine(JsonConvert.SerializeObject(data));
        string discrim = Rnd.Next(10000).ToString();
        if (_db.Users.Any(x => x.Email == data.Email)) return new StatusCodeResult(403);
        var userId = new IdGen.IdGenerator(0).CreateId() + "";
        var user = new User()
        {
            CreatedAt = DateTime.Now,
            Username = data.Username,
            Discriminator = discrim,
            Id = userId,
            Bot = false,
            System = false,
            Desktop = false,
            Mobile = false,
            Premium = true,
            PremiumType = 2,
            Bio = "",
            MfaEnabled = false,
            Verified = true,
            Disabled = false,
            Deleted = false,
            Email = data.Email,
            Rights = Static.Config.Security.Register.DefaultRights,
            NsfwAllowed = true, // TODO = depending on age
            PublicFlags = 0,
            Flags = "0", // TODO = generate
            Data = JsonConvert.SerializeObject(new
            {
                hash = BCrypt.Net.BCrypt.HashPassword(data.Password, 12),
                valid_tokens_since = DateTime.Now,
            }),
            Settings = new UserSetting()
            {
                Id = userId,
            },
            ExtendedSettings = "",
            SettingsId = userId,
            Fingerprints = "",
            ExtendedSettings = ""
        };
        user.Settings.Id = user.Id;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        
        var token = _auth.Authenticate(data.Email, data.Password);
        if (token == null) return new StatusCodeResult(500);
        return new {token};
    }
    /// <summary>
    /// Log a user in
    /// </summary>
    /// <returns>Json object with token</returns>
    [HttpPost("/api/auth/login")]
    public async Task<object> Login()
    {
        var data = JsonConvert.DeserializeObject<LoginData>(await new StreamReader(Request.Body).ReadToEndAsync());
        Console.WriteLine(JsonConvert.SerializeObject(data));
        
        var token = _auth.Authenticate(data.Login, data.Password);
        if (token == null) return StatusCode(403, "Invalid username or password!");
        return new {token};
    }
}