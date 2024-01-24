using Boiler.Server.Framework;
using Boiler.Server.Models.Identity;
using Boiler.Server.Models.Thermometer;

namespace Boiler.Server.Models;

public class TemperatureModel
{
    public long Id { get; set; }
    public string? Value { get; set; }
    public DateTime? Time { get; set; } = TimeExtension.PragueDateNowAsUtc().AddHours(-1);
    public string? ThermometerId { get; set; }
    public virtual ThermometerModel? Thermometer { get; set;}
}
