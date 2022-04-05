using Fosscord.DbModel.Scaffold;

namespace Fosscord.Gateway.Models;

public class Identify
{
    public string token;
    public IdentifyProperties properties;
    public Intents intents;
    public ActivityBase presence;
    public bool compress;
    public int large_threshold;
    public long shard;
    public bool guild_subscriptions;
    public int capabilities;
    public IdentifyClientState client_state;
    public int v;
}

public class IdentifyClientState
{
    //todo: implement
}

public class IdentifyProperties
{
    public string os;
    public string os_atch;
    public string browser;
    public string device;
    public string browser_user_agent;
    public string browser_version;
    public string os_version;
    public string referrer;
    public string referring_domain;
    public string referrer_current;
    public string referring_domain_current;
    public string release_channel;
    public int client_build_number;
    public object client_event_source;
    public string client_version;
    public string system_locale;
}

public class ActivityBase
{
    public bool afk;
    public string status;
    public int since;
    public List<Activity> activities;
}

public class Activity
{
    public string name;
    public int type;
    public string url;
    public DateTime created_at;
    public Timestamps Timestamps;
    public int application_id;
    public string details;
    public Emoji emoji;
    public Party party;
    public Assets assets;
    public Secrets secrets;
    public bool instance;
    public int since;
}

public class Assets
{
    public string large_image;
    public string large_text;
    public string small_image;
    public string small_text;
}

public class Secrets
{
    public string join;
    public string spectate;
    public string match;
}

public class Party
{
    public string id;
    public List<int> size;
}

public class Emoji
{
    public string name;
    public string id;
    public bool animated;
}

public class Timestamps
{
    public int start;
    public int end;
}