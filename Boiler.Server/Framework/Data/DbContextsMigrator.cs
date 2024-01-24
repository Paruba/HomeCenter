using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Reflection;

namespace Boiler.Server.Framework.Data;

public class DbContextsMigrator
{
    public void MigrateDatabase(IServiceProvider serviceProvider, IConfiguration configuration, string[] dependentAssemblies)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (dependentAssemblies == null)
            throw new ArgumentNullException(nameof(dependentAssemblies));

        string connectionString = configuration.GetValue<string>("Data:DefaultConnection:ConnectionString");
        var connStrBuilder = new NpgsqlConnectionStringBuilder(connectionString);

        var logger = (ILogger<DbContextsMigrator>)serviceProvider.GetService(typeof(ILogger<DbContextsMigrator>));
        logger.LogInformation($"Applying database migrations. Server={connStrBuilder.Host}, Database={connStrBuilder.Database}");

        Assembly[] assemblies = LoadAssemblies(dependentAssemblies);

        Type[] dbContextTypes = assemblies.SelectMany(a => a.GetExportedTypes())
            .Where(t => typeof(DbContext).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToArray();

        foreach (Type eachDbContextType in dbContextTypes)
        {
            var genericDbContextOptionsType = typeof(DbContextOptions<>).MakeGenericType(eachDbContextType);

            object dbContextOptionsInstance = serviceProvider.GetService(genericDbContextOptionsType);

            using (DbContext contextInstance = (DbContext)Activator.CreateInstance(eachDbContextType, dbContextOptionsInstance))
            {
                contextInstance.Database.Migrate();
            }
        }
    }

    private Assembly[] LoadAssemblies(string[] dependentAssemblies)
    {
        var assemblies = new Assembly[dependentAssemblies.Length];

        for (int i = 0; i < dependentAssemblies.Length; i++)
        {
            assemblies[i] = Assembly.Load(dependentAssemblies[i]);
        }

        return assemblies;
    }
}
