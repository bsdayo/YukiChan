using Konata.Core.Message;
using SQLite;

#pragma warning disable CS8618

namespace YukiChan.Modules.Bottle;

[Table("bottles")]
public class Bottle
{
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    ///     In seconds.
    /// </summary>
    [Column("timestamp")]
    public long Timestamp { get; set; }

    [Column("context")] public MessageStruct.SourceType Context { get; set; }

    [Column("group_uin")] public uint GroupUin { get; set; }

    [Column("group_name")] public string GroupName { get; set; }

    [Column("user_uin")] public uint UserUin { get; set; }

    [Column("user_name")] public string UserName { get; set; }

    [Column("text")] public string Text { get; set; }

    [Column("image_filename")] public string ImageFilename { get; set; }

    [Column("image_md5")] public string ImageMd5 { get; set; }
}