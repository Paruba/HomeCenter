using Boiler.Server.Framework;
using Boiler.Server.Models.Thermometer;

namespace Boiler.Server.Services.Thermometer;

public interface IThermometerService : IDependency
{
    Task<List<ThermometerModel>?> GetThermometers(string userId);
    Task<bool> CreateThermometer(ThermometerModel thermometer);
    Task<bool> DeleteThermometer(string thermometerId, string userId);
}
