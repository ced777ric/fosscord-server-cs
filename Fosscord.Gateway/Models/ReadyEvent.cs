using Fosscord.DbModel.Scaffold;
using Fosscord.Util.Models;

namespace Fosscord.Gateway.Models;

public class ReadyEvent
{ public class ReadyEventData
    {
        public int v { get; set; }
        public Application application { get; set; }
        public PrivateUser user { get; set; }
        public object user_settings { get; set; }
        public List<Guild> guilds { get; set; }
        public List<object> guild_experiments { get; set; } //todo
        public List<object> geo_ordered_rtc_regions { get; set; } //todo
        public List<PublicRelationShip> relationships { get; set; }
        public ReadState read_state { get; set; }
        public GuildMemberSettings user_guild_settings { get; set; }
        public List<Channel> private_channels { get; set; }
        public string session_id { get; set; }
        public string analytics_token { get; set; }
        public List<ConnectedAccount> connected_accounts { get; set; }
        public Consents consents { get; set; }
        public string country_code { get; set; }
        public int friend_suggestions { get; set; }
        public List<object> experiments { get; set; }
        public List<object> guild_join_requests { get; set; }
        public List<PublicUser> users { get; set; }
        public List<Member> merged_members { get; set; }
    }

    public class Consents
    {
        public PersonalizationConsents personalization { get; set; }
    }

    public class PersonalizationConsents
    {
        public bool consented { get; set; }
    }

    public class ReadState
    {
        public List<DbModel.Scaffold.ReadState> entries { get; set; }
        public bool partial { get; set; }
        public int version { get; set; }
    }
    
    public class GuildMemberSettings
    {
        public List<string> entries { get; set; }
        public bool partial { get; set; }
        public int version { get; set; }
    }
}