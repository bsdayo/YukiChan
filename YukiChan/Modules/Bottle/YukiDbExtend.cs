using Konata.Core.Message;
using YukiChan.Modules.Bottle;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Database;

[YukiDatabase(BottleDbName, typeof(Bottle))]
public partial class YukiDbManager
{
    private const string BottleDbName = "Bottles";

    public Bottle? GetRandomBottle()
    {
        return Databases[BottleDbName].FindWithQuery<Bottle>(
            "SELECT * FROM bottles ORDER BY random() LIMIT 1");
    }

    public Bottle? GetBottle(int id)
    {
        return Databases[BottleDbName].FindWithQuery<Bottle>(
            "SELECT * FROM bottles WHERE id = ?", id);
    }

    public Bottle AddBottle(MessageStruct message, string text, string imageFilename, string imageMd5)
    {
        var bottle = new Bottle
        {
            Timestamp = DateTime.Now.GetTimestamp(),
            Context = message.Type,
            GroupUin = message.Receiver.Uin,
            GroupName = message.Receiver.Name,
            UserUin = message.Sender.Uin,
            UserName = message.Sender.Name,
            Text = text,
            ImageFilename = imageFilename,
            ImageMd5 = imageMd5
        };
        Databases[BottleDbName].Insert(bottle);
        return bottle;
    }

    public void UpdateBottle(Bottle bottle)
    {
        Databases[BottleDbName].Update(bottle);
    }

    public int GetNewBottleId()
    {
        return Databases[BottleDbName].FindWithQuery<Bottle>(
            "SELECT id FROM bottles ORDER BY id DESC LIMIT 1").Id;
    }

    public void RemoveBottle(int bottleId)
    {
        Databases[BottleDbName].Execute(
            "DELETE FROM bottles WHERE id = ?", bottleId);
    }

    public List<string> GetAllBottleImageMd5()
    {
        return Databases[BottleDbName].Query<Bottle>(
                "SELECT * FROM bottles")
            .ConvertAll(bottle => bottle.ImageMd5);
    }

    public void FixBottle()
    {
        var all = Databases[BottleDbName].Query<Bottle>("SELECT * FROM bottles");

        foreach (var bottle in all)
            if (!string.IsNullOrWhiteSpace(bottle.ImageFilename))
                try
                {
                    var newFilename = $"{bottle.Id}.{bottle.ImageFilename.Split(".")[1]}";
                    File.Move($"Data/BottleImages/{bottle.ImageFilename}", $"Data/BottleImages/{newFilename}");
                    bottle.ImageFilename = newFilename;
                }
                catch
                {
                    // ignored
                }

        Databases[BottleDbName].UpdateAll(all);
    }
}