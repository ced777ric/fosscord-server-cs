namespace Fosscord.Gateway.Models;

public class Constants
{
    public enum OpCodes {
        Dispatch = 0,
        Heartbeat = 1,
        Identify = 2,
        Presence_Update = 3,
        Voice_State_Update = 4,
        Voice_Server_Ping = 5, // ? What is opcode 5?
        Resume = 6,
        Reconnect = 7,
        Request_Guild_Members = 8,
        Invalid_Session = 9,
        Hello = 10,
        Heartbeat_ACK = 11,
        Guild_Sync = 12,
        DM_Update = 13,
        Lazy_Request = 14,
        Lobby_Connect = 15,
        Lobby_Disconnect = 16,
        Lobby_Voice_States_Update = 17,
        Stream_Create = 18,
        Stream_Delete = 19,
        Stream_Watch = 20,
        Stream_Ping = 21,
        Stream_Set_Paused = 22,
        Request_Application_Commands = 24,
    }
    public enum CloseCodes {
        Unknown_error = 4000,
        Unknown_opcode = 4001,
        Decode_error = 4002,
        Not_authenticated = 4003,
        Authentication_failed = 4004,
        Already_authenticated = 4005,
        Invalid_session = 4006,
        Invalid_seq = 4007,
        Rate_limited = 4008,
        Session_timed_out = 4009,
        Invalid_shard = 4010,
        Sharding_required = 4011,
        Invalid_API_version = 4012,
        Invalid_intent = 4013,
        Disallowed_intent = 4014,
    }
}

public class Payload
{
    public Constants.OpCodes op { get; set; }
    public int? s { get; set; }
    public string? t { get; set; }
    public object? d { get; set; }
}