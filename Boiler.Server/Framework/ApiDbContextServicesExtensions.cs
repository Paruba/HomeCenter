using Microsoft.EntityFrameworkCore;

namespace Boiler.Server.Framework;

public static class ApiDbContextServicesExtensions
{
    public static IServiceCollection AddApiDbContexts<TContext>(this IServiceCollection serviceCollection, IConfiguration cfg, string assemblyName) where TContext : DbContext
    {
        TimeSpan commandTimeout = cfg.GetValue<TimeSpan>(Constants.Database.Data_CommandTimeout);
        string connectionString = cfg.GetValue<string>(Constants.Database.DefaultConnection_StringConfig);
        serviceCollection.AddEntityFrameworkNpgsql().AddDbContext<TContext>(ctxOpt =>
        {
            ctxOpt.UseNpgsql(connectionString, x =>
            {
                x.EnableRetryOnFailure(maxRetryCount: 4, maxRetryDelay: TimeSpan.FromSeconds(1), new string[] { });
                x.MigrationsAssembly(assemblyName);
                x.CommandTimeout((int)commandTimeout.TotalMilliseconds);
                x.MigrationsHistoryTable("__EFMMigrationsHistory", Constants.Database.DefaultDbSchema);
            });
        });

        return serviceCollection;
    }
}