using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.Shared.Data.Console.Arcaea;

public sealed class ArcaeaPreferencesResponse
{
    public required ArcaeaUserPreferences Preferences { get; init; }
}