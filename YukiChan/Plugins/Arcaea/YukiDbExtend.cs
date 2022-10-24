using YukiChan.Plugins.Arcaea.Models.Database;
using IDbContext = Chloe.IDbContext;

// ReSharper disable CheckNamespace

namespace YukiChan.Database;

[YukiDatabase(ArcaeaDbName, typeof(ArcaeaDatabaseUser), typeof(ArcaeaUserPreferences) /*, typeof(ArcaeaGuessUser)*/)]
public partial class YukiDbManager
{
    private const string ArcaeaDbName = "arcaea";

    public async Task<ArcaeaDatabaseUser?> GetArcaeaUser(string platform, string userId, IDbContext? dbCtx = null)
    {
        var ctx = dbCtx ?? GetDbContext(ArcaeaDbName);
        var user = await ctx.Query<ArcaeaDatabaseUser>()
            .Where(user => user.Platform == platform)
            .Where(user => user.UserId == userId)
            .FirstOrDefaultAsync();

        if (dbCtx is null) ctx.Dispose();
        return user;
    }

    public async Task AddOrUpdateArcaeaUser(string platform, string userId, string arcId, string arcName)
    {
        using var ctx = GetDbContext(ArcaeaDbName);

        var old = await GetArcaeaUser(platform, userId, ctx);

        var user = new ArcaeaDatabaseUser
        {
            Platform = platform,
            UserId = userId,
            ArcaeaId = arcId,
            ArcaeaName = arcName
        };

        if (old is not null)
        {
            user.Id = old.Id;
            await ctx.UpdateAsync(user);
        }
        else
        {
            await ctx.InsertAsync(user);
        }
    }

    public async Task<bool> DeleteArcaeaUser(string platform, string userId)
    {
        using var ctx = GetDbContext(ArcaeaDbName);
        var user = await GetArcaeaUser(platform, userId, ctx);
        if (user is null) return false;
        await ctx.DeleteAsync(user);
        return true;
    }

    public async Task<ArcaeaUserPreferences?> GetArcaeaUserPreferences(string platform, string userId,
        IDbContext? dbCtx = null)
    {
        var ctx = dbCtx ?? GetDbContext(ArcaeaDbName);
        var pref = await ctx.Query<ArcaeaUserPreferences>()
            .Where(pref => pref.Platform == platform)
            .Where(pref => pref.UserId == userId)
            .FirstOrDefaultAsync();
        if (dbCtx is null) ctx.Dispose();
        return pref;
    }

    public async Task AddOrUpdateArcaeaUserPreferences(string platform, string userId, ArcaeaUserPreferences pref)
    {
        pref.Platform = platform;
        pref.UserId = userId;

        using var ctx = GetDbContext(ArcaeaDbName);

        var old = await GetArcaeaUserPreferences(platform, userId);

        if (old is not null)
        {
            pref.Id = old.Id;
            await ctx.UpdateAsync(pref);
        }
        else
        {
            await ctx.InsertAsync(pref);
        }
    }
}