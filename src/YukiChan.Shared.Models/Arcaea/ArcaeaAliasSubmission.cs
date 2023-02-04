namespace YukiChan.Shared.Models.Arcaea;

public sealed class ArcaeaAliasSubmission
{
    public int Id { get; init; }

    public required string Platform { get; init; }

    public required string UserId { get; init; }

    public required string SongId { get; init; }

    public required string Alias { get; init; }

    public required DateTime SubmitTime { get; init; }

    public required ArcaeaAliasSubmissionStatus Status { get; set; }
}

public enum ArcaeaAliasSubmissionStatus
{
    Rejected = -1,
    Pending = 0,
    Accepted = 1
}