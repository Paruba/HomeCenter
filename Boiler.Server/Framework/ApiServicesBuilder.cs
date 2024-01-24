using Boiler.Server.Framework.Data;
using Boiler.Shared.Attributes;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace Boiler.Server.Framework;

public class ApiServicesBuilder
{
    private const string ValidationServiceInterface = "ValidationService";

    public void ConfigureServices(IServiceCollection services, string[] dependentAssemblies)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (dependentAssemblies == null)
            throw new ArgumentNullException(nameof(dependentAssemblies));

        Assembly[] assemblies = LoadAssemblies(dependentAssemblies);
        Type[] exportedTypes = assemblies.SelectMany(a => a.GetExportedTypes()).ToArray();

        // Discover suppressed dependencies
        HashSet<string> suppresedDependencies = (from t in exportedTypes
                                                 let suppressAttrs = t.GetCustomAttributes<ApiSuppressDependencyAttribute>(false)
                                                 where suppressAttrs.Any()
                                                 from suppressedType in suppressAttrs.Select(a => a.FullName)
                                                 select suppressedType).ToHashSet();

        ConfigureDependencyModules(exportedTypes, services);

        // Register standard dependencies
        foreach (Type depdendency in exportedTypes
            .Where(t => typeof(IDependency).IsAssignableFrom(t) &&
                        !t.IsInterface &&
                        !t.IsAbstract &&
                        !suppresedDependencies.Contains(t.FullName)))
        {
            foreach (var inerfaceType in depdendency.GetInterfaces().Where(itf => typeof(IDependency).IsAssignableFrom(itf)))
            {
                if (typeof(ITransientDependency).IsAssignableFrom(depdendency)) // Register transient dependencies
                {
                    services.AddTransient(inerfaceType, depdendency);
                }
                else if (typeof(ISingletonDependency).IsAssignableFrom(depdendency)) // Register singleton dependencies
                {
                    services.AddSingleton(inerfaceType, depdendency);
                }
                else // Regiser unit of work dependencies (ie. web api request)
                {
                    services.AddScoped(inerfaceType, depdendency);

                    var validationServices = inerfaceType.GetInterfaces().Where(i => i.Name.EndsWith(ValidationServiceInterface));

                    foreach (var eachValidationInterface in validationServices)
                    {
                        services.AddTransient(eachValidationInterface, depdendency);
                    }

                    if (typeof(IAuthorizationHandler).IsAssignableFrom(depdendency))
                    {
                        services.AddScoped(typeof(IAuthorizationHandler), depdendency);
                    }
                }
            }
        }
    }

    public static void EnsureDatabaseMigrations(IServiceProvider serviceProvider, IConfiguration configuration, string[] dependentAssemblies)
    {
        var migrator = new DbContextsMigrator();
        migrator.MigrateDatabase(serviceProvider, configuration, dependentAssemblies);
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

    private void ConfigureDependencyModules(Type[] exportedTypes, IServiceCollection services)
    {
        foreach (
            Type eachType in
            exportedTypes.Where(
                t => typeof(IDepdencencyModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
        {
            var typeInstance = (IDepdencencyModule)Activator.CreateInstance(eachType);
            typeInstance.ConfigureServices(services);
        }
    }
}