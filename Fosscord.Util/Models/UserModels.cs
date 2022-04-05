using Fosscord.DbModel.Scaffold;

namespace Fosscord.Util.Models;

public class PublicRelationShip
{
    public string id { get; set; }
    public int type { get; set; }
    public string nickname { get; set; }
    public PublicUser user { get; set; }
}
    
public class PublicUser
{
    public string id { get; set; } = null!;
    public string username { get; set; } = null!;
    public string discriminator { get; set; } = null!;
    public string? avatar { get; set; }
    public int? accent_color { get; set; }
    public string? banner { get; set; }
    public bool bot { get; set; }
    public string bio { get; set; } = null!;
    public int public_flags { get; set; }
    public DateTime? premium_since { get; set; } = null;
}
    
public class PrivateUser
{
    public string id { get; set; } = null!;
    public string username { get; set; } = null!;
    public string discriminator { get; set; } = null!;
    public string? avatar { get; set; }
    public int? accent_color { get; set; }
    public string? banner { get; set; }
    public string? phone { get; set; }
    public bool desktop { get; set; }
    public bool mobile { get; set; }
    public bool premium { get; set; }
    public int premium_type { get; set; }
    public bool bot { get; set; }
    public string bio { get; set; } = null!;
    public bool nsfw_allowed { get; set; }
    public bool mfa_enabled { get; set; }
    public bool verified { get; set; }
    public string? email { get; set; }
    public string flags { get; set; } = null!;
    public int public_flags { get; set; }
    public DateTime? premium_since { get; set; } = null;
}

public static class UserExtensions
{
    public static PublicUser AsPublicUser(this User user)
    {
        return new PublicUser()
        {
            accent_color = user.AccentColor,
            avatar = user.Avatar,
            banner = user.Banner,
            bio = user.Bio,
            bot = user.Bot,
            discriminator = user.Discriminator,
            id = user.Id,
            premium_since = null,
            public_flags = user.PublicFlags,
            username = user.Username
        };
    }
    
    public static PrivateUser AsPrivateUser(this User user)
    {
        return new PrivateUser()
        {
            accent_color = user.AccentColor,
            avatar = user.Avatar,
            banner = user.Banner,
            bio = user.Bio,
            bot = user.Bot,
            desktop = user.Desktop,
            discriminator = user.Discriminator,
            email = user.Email,
            flags = user.Flags,
            id = user.Id,
            username = user.Username,
            mobile = user.Mobile,
            phone = user.Phone,
            premium = user.Premium,
            premium_type = user.PremiumType,
            nsfw_allowed = user.NsfwAllowed,
            mfa_enabled = user.MfaEnabled,
            verified = user.Verified,
            public_flags = user.PublicFlags,
        };
    }

    public static PublicRelationShip AsPublicRelationShip(this Relationship rel)
    {
        return new PublicRelationShip()
        {
            id = rel.Id,
            nickname = rel.Nickname,
            type = rel.Type,
            user = rel.To.AsPublicUser()
        };
    }
}