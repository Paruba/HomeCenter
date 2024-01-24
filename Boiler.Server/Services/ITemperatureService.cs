using Boiler.Server.Framework;
using Boiler.Server.Models;

namespace Boiler.Server.Services;

public interface ITemperatureService : IDependency
{
    Task<List<TemperatureModel>?> GetTemperatures(string userId, string thermometerId);
    Task<TemperatureModel?> GetCurrentTemperature(string userId, string thermometerId);
    Task<bool> SetTemperature(TemperatureModel temperature);
}
