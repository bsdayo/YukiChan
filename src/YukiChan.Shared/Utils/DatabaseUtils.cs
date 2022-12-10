using Chloe;

namespace YukiChan.Shared.Utils;

public static class DatabaseUtils
{
    public static async Task InsertOrUpdateAsync<TEntity>(this IDbContext ctx, TEntity entity)
    {
        var count = await ctx.UpdateAsync(entity);

        if (count == 0)
            await ctx.InsertAsync(entity);
    }
}