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

    public int AddBottle(MessageStruct message, string text, string imageFilename)
    {
        Databases[BottleDbName].Insert(new Bottle
        {
            Timestamp = DateTime.Now.GetTimestamp(),
            Context = message.Type,
            GroupUin = message.Receiver.Uin,
            GroupName = message.Receiver.Name,
            UserUin = message.Sender.Uin,
            UserName = message.Sender.Name,
            Text = text,
            ImageFilename = imageFilename
        });
        return Databases[BottleDbName].FindWithQuery<Bottle>(
            "SELECT id FROM bottles ORDER BY id DESC LIMIT 1").Id;
    }

    public void RemoveBottle(int bottleId)
    {
        Databases[BottleDbName].Execute(
            "DELETE FROM bottles WHERE id = ?", bottleId);
    }

    public void FixBottle()
    {
        var all = Databases[BottleDbName].Query<Bottle>("SELECT * FROM bottles");

        foreach (var bottle in all)
        {
            bottle.ImageFilename = bottle.ImageFilename
                .Replace(".0", ".jpg")
                .Replace(".pjpeg", ".jpg");
        }

        Databases[BottleDbName].UpdateAll(all);

        foreach (var file in Directory.GetFiles("Data/BottleImages"))
        {
            File.Move(file, file
                .Replace(".0", ".jpg")
                .Replace(".pjpeg", ".jpg"), true);
        }
    }
}