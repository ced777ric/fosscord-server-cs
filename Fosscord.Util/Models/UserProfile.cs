using Fosscord.DbModel.Scaffold;

namespace Fosscord.Util.Models;

public class UserProfile
{
    public class PublicConnectedAccount
    {
        public string type { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public bool verified { get; set; }
    }

    public class UserProfileResponseNoMutual
    {
        public PublicUser user { get; set; }
        public List<PublicConnectedAccount> connected_accounts { get; set; }
        public DateTime? premium_since { get; set; }
        public DateTime? premium_guild_since { get; set; }
    }
    
    public class MutualGuild
    {
        public string id { get; set; }
        public string nick { get; set; }
    }
    
    public class UserProfileResponseMutual
    {
        public PublicUser user { get; set; }
        public List<PublicConnectedAccount> connected_accounts { get; set; }
        public DateTime? premium_since { get; set; }
        public DateTime? premium_guild_since { get; set; }
        public List<MutualGuild> mutual_guilds { get; set; }
    }
}

public static class UserProfileExtensions
{
    public static UserProfile.PublicConnectedAccount AsPublicConnection(this ConnectedAccount connectedAccount)
    {
        return new UserProfile.PublicConnectedAccount()
        {
            type = connectedAccount.Type,
            id = connectedAccount.Id,
            name = connectedAccount.Name,
            verified = connectedAccount.Verifie
        };
    }
    
    public static UserProfile.MutualGuild AsMutualGuild(this Member member)
    {
        return new UserProfile.MutualGuild()
        {
            id = member.Guild.Id,
            nick = string.IsNullOrEmpty(member.Nick) ? member.IdNavigation.Username : member.Nick
        };
    }
}