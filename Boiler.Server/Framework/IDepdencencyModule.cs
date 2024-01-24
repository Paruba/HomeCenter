namespace Boiler.Server.Framework;

public interface IDepdencencyModule
{
    void ConfigureServices(IServiceCollection services);
}