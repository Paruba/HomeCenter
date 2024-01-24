using Microsoft.EntityFrameworkCore;

namespace Boiler.Server.Framework;

public static class EntityExtension
{
    public static void ReloadEntity<TEntity>(this DbContext context, TEntity entity) where TEntity : class
    {
        context.Entry(entity).Reload();
    }
}
