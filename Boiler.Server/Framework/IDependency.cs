namespace Boiler.Server.Framework;

public interface IDependency
{
}

public interface ISingletonDependency : IDependency
{
}

public interface ITransientDependency : IDependency
{
}