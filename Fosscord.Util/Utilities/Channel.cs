using Fosscord.API;
using Fosscord.DbModel;
using Fosscord.DbModel.Scaffold;

namespace Fosscord.Util.Utilities;

public class Channel
{
    public static async Task<DbModel.Scaffold.Channel> CreateDMChannel(Db db, string[] recipient, string ownerid, string? name)
    {
        List<User> recipients = new List<User>();
        foreach (var user in recipient)
        {
            var usr = db.Users.FirstOrDefault(s => s.Id == user);
            if (usr == null)
                continue;
            recipients.Add(usr);
        }

        if (recipient.Length != recipients.Count)
        {
            throw new Exception("Recipient/s not found");
        }

        int max = 10; //todo config

        if (recipients.Count > max)
        {
            throw new Exception($"A group chat can only have a maximum of {max} members");
        }

        int type = recipients.Count == 1 ? 1 : 3;

        var channel = db.Channels.Add(new DbModel.Scaffold.Channel()
        {
            Name = name,
            Type = type,
            OwnerId = ownerid,
            CreatedAt = DateTime.UtcNow,
            LastMessageId = null,
            Recipients = new List<Recipient>(),
            Id = new IdGen.IdGenerator(0).CreateId() + ""
        });
        await db.SaveChangesAsync();

        foreach (var recip in recipients)
        {
            db.Recipients.Add(new Recipient()
            {
                ChannelId = channel.Entity.Id,
                Closed = false,
                Id = new IdGen.IdGenerator(0).CreateId() + "",
                UserId = recip.Id
            });
        }

        await db.SaveChangesAsync();

        return channel.Entity;
    }
}