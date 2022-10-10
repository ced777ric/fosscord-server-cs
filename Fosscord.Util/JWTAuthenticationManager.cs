using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fosscord.DbModel;
using Fosscord.DbModel.Scaffold;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Fosscord.API.Classes;

public class JwtAuthenticationManager
{
    private readonly Db _db = Db.GetNewDb();
 
    private readonly string _tokenKey = Static.Config.Security.JwtSecret;

    public User GetUserFromToken(string token, out ClaimsPrincipal tokenClaim)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_tokenKey);
        var validationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false,
        };
        var claim = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken tokenValidated);
        tokenClaim = claim;
        return _db.Users.Include(s => s.Settings).FirstOrDefault(x => x.Id == claim.Identity.Name);
    }
 
    public string? Authenticate(string username, string password)
    {
        var user = _db.Users.FirstOrDefault(x => x.Email == username);
        if (user == null) return null;
        var hash = ((dynamic) JsonConvert.DeserializeObject(user.Data)).hash;
        if (!BCrypt.Net.BCrypt.Verify(password, hash.ToString())) return null;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_tokenKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id)
            }),
            Expires = DateTime.UtcNow.AddDays(30),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}