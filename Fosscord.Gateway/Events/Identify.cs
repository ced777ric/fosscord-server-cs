using System.Net.WebSockets;
using System.Security.Claims;
using Fosscord.API.Classes;
using Fosscord.API.Utilities;
using Fosscord.DbModel;
using Fosscord.DbModel.Scaffold;
using Fosscord.Gateway.Controllers;
using Fosscord.Gateway.Models;
using Fosscord.Util.Models;
using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Constants = Fosscord.Gateway.Models.Constants;

namespace Fosscord.Gateway.Events;

public class Identify: IGatewayMessage
{
    private readonly JWTAuthenticationManager _auth;
    public Identify()
    {
        _auth = new JWTAuthenticationManager();
    }
    
    public Constants.OpCodes OpCode { get; } = Constants.OpCodes.Identify;

    public async Task Invoke(Payload payload, Websocket client)
    {
        if (payload.d is JObject jObject)
        {
            var identify = jObject.ToObject<Models.Identify>();
            User user = null;
            try
            {
                 user = _auth.GetUserFromToken(identify.token, out ClaimsPrincipal claimsPrincipal);
            }
            catch (Exception e)
            {
                if (GatewayController.Clients.ContainsKey(client))
                    await GatewayController.Clients[client].CloseAsync(WebSocketCloseStatus.NormalClosure, ((int)Constants.CloseCodes.Authentication_failed).ToString(), client.CancellationToken);
                return;
            }

            if (user == null)
            {
                if (GatewayController.Clients.ContainsKey(client))
                    await GatewayController.Clients[client].CloseAsync(WebSocketCloseStatus.NormalClosure, ((int)Constants.CloseCodes.Authentication_failed).ToString(), client.CancellationToken);
                return;
            }

            Db db = Db.GetNewDb();
            client.session_id = RandomStringGenerator.Generate(32);
            List<PublicUser> users = new List<PublicUser>();

            foreach (var relation in db.Relationships.Include(s => s.To).Where(s => s.FromId == user.Id).ToList())
            {
                if (users.Any(s => s.id == relation.ToId))
                    continue;
                users.Add(relation.To.AsPublicUser());
            }

            var dmChannels = db.Channels.Include(s => s.Recipients).ThenInclude(s => s.User).Where(s => (s.Type == 1 || s.Type == 3) && s.Recipients.Any(s => s.Id == user.Id)).ToList();
            foreach (var channel in dmChannels)
            {
                foreach (var recipient in channel.Recipients)
                {
                    if (users.Any(s => s.id == recipient.UserId))
                        continue;
                    users.Add(recipient.User.AsPublicUser());
                }
            }
            
            var session = db.Sessions.Add(new Session()
            {
                Id = new IdGenerator(0).CreateId() + "",
                UserId = user.Id,
                SessionId = client.session_id,
                Status = identify.presence.status,
                ClientInfo = JsonConvert.SerializeObject(new
                {
                    client = "desktop", //todo implement other clients
                    os = identify.properties.os,
                    version = 0
                }),
                Activities = "[]",
            });
            await db.SaveChangesAsync();
            
            await GatewayController.Send(client, new Payload()
            {
                d = new List<object>()
                {
                    {new
                    {
                        id = session.Entity.Id,
                        user_id = session.Entity.UserId,
                        session_id = session.Entity.SessionId,
                        activities = JsonConvert.DeserializeObject<List<Activity>>(session.Entity.Activities),
                        client_info = JsonConvert.DeserializeObject<object>(session.Entity.ClientInfo),
                        status = session.Entity.Status
                    }}
                },
                op = Constants.OpCodes.Dispatch,
                t = "SESSIONS_REPLACE",
                s = client.sequence++
            });
            
            await GatewayController.Send(client, new Payload()
            {
                d = new
                {
                    user = user.AsPublicUser(),
                    activities = JsonConvert.DeserializeObject<List<Activity>>(session.Entity.Activities),
                    client_status = JsonConvert.DeserializeObject<object>(session.Entity.ClientInfo),
                    status = session.Entity.Status
                },
                op = Constants.OpCodes.Dispatch,
                t = "PRESENCE_UPDATE",
                s = client.sequence++
            });
            
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(user.Settings);
            var readyEventData = new ReadyEvent.ReadyEventData()
            {
                v = 9,
                application = db.Applications.FirstOrDefault(s => s.Id == user.Id),
                user = user.AsPrivateUser(),
                user_settings = user.Settings,
                guilds = db.Members.Where(s => s.Id == user.Id).Select(s => s.Guild).ToList(),
                relationships = db.Relationships.Include(s => s.To).Where(s => s.FromId == user.Id).ToList().Select(s => s.AsPublicRelationShip()).ToList(),
                read_state = new ReadyEvent.ReadState()
                {
                    entries = db.ReadStates.Where(s => s.User.Id == user.Id).ToList(),
                    partial = false,
                    version = 304128,
                },
                user_guild_settings = new ReadyEvent.GuildMemberSettings()
                {
                    entries = db.Members.Where(s => s.Id == user.Id).Select(s => s.Settings).ToList(),
                    partial = false,
                    version = 642,
                },
                private_channels = dmChannels,
                session_id = client.session_id,
                analytics_token = "",
                connected_accounts = db.ConnectedAccounts.Where(s => s.User.Id == user.Id).ToList(),
                consents = new ReadyEvent.Consents()
                {
                    personalization = new ReadyEvent.PersonalizationConsents()
                    {
                        consented = false,
                    }
                },
                country_code = settings.ContainsKey("locale") ? settings["locale"] : "en-us",
                friend_suggestions = 0,
                experiments = new List<object>(),
                guild_join_requests = new List<object>(),
                users = users,
                merged_members = db.Members.Where(s => s.Id == user.Id).ToList()
            };

            await GatewayController.Send(client, new Payload()
            {
                d = readyEventData,
                op = Constants.OpCodes.Dispatch,
                t = "READY",
                s = client.sequence++
            });
            client.is_ready = true;

            foreach (var relationship in db.Relationships.Include(s => s.To).Where(s => s.FromId == user.Id).ToList())
            {
                await GatewayController.Send(client, new Payload()
                {
                    d = new
                    {
                        data = relationship.AsPublicRelationShip(),
                        user_id = relationship.ToId
                    },
                    op = Constants.OpCodes.Dispatch,
                    t = "RELATIONSHIP_ADD",
                    s = client.sequence++
                });
            }
            
            foreach (var privateChannels in dmChannels)
            {
                await GatewayController.Send(client, new Payload()
                {
                    d = new
                    {
                        data = privateChannels
                    },
                    op = Constants.OpCodes.Dispatch,
                    t = "CHANNEL_CREATE",
                    s = client.sequence++
                });
            }
            
            Console.WriteLine($"Got user {user.Id} {user.Email}");
        }
    }
}