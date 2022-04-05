using Fosscord.DbModel.Scaffold;

namespace Fosscord.DbModel;

public class FosscordConfig
{
    public static int GetInt(string key, int defaultValue = 0)
    {
        var db = Db.GetNewDb();
        var val = db.Configs.FirstOrDefault(x => x.Key == key)?.Value;
        
        if (val == null)
        {
            val = defaultValue+"";
            db.Configs.Add(new Config()
            {
                Key = key,
                Value = defaultValue+""
            });
        }

        db.SaveChanges();
        
        return int.Parse(val);
    }

    public static string GetString(string key, string defaultValue = "")
    {
        var db = Db.GetNewDb();
        var val = db.Configs.FirstOrDefault(x => x.Key == key)?.Value;
        
        if (val == null)
        {
            val = defaultValue+"";
            db.Configs.Add(new Config()
            {
                Key = key,
                Value = defaultValue+""
            });
        }

        db.SaveChanges();
        
        return val;
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        var db = Db.GetNewDb();
        var val = db.Configs.FirstOrDefault(x => x.Key == key)?.Value;
        
        if (val == null)
        {
            val = defaultValue+"";
            db.Configs.Add(new Config()
            {
                Key = key,
                Value = defaultValue+""
            });
        }

        db.SaveChanges();
        
        return bool.Parse(val);
    }
}