using Boiler.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Boiler.Server.Services;

public class TemperatureService : ITemperatureService
{
    private readonly ServerDbContext _dbContext;
    private readonly ILogger<TemperatureService> _logger;
    public TemperatureService(ServerDbContext dbContext, ILogger<TemperatureService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<TemperatureModel>?> GetTemperatures(string userId, string thermometerId)
    {
        try {
            var thermometer = await _dbContext.Thermometer.Include(x => x.Temperature).FirstOrDefaultAsync(x => x.UserId == userId && x.Id == thermometerId);
            return thermometer.Temperature.OrderByDescending(x => x.Time).ToList();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return null;
        }
    }

    public async Task<TemperatureModel?> GetCurrentTemperature(string userId, string thermometerId)
    {
        try {
            var temperature = await _dbContext.Temperatures
                                 .Include(x => x.Thermometer)
                                 .Where(x => x.Thermometer.UserId == userId)
                                 .Where(x => x.Thermometer.Id == thermometerId)
                                 .OrderByDescending(t => t.Time)
                                 .FirstOrDefaultAsync();

            return temperature;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return null;
        }
    }

    public async Task<bool> SetTemperature(TemperatureModel temperature)
    {
        try
        {
           _dbContext.Temperatures.Add(temperature);
            await _dbContext.SaveChangesAsync();
            return true;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return false;
        }
    }
}
