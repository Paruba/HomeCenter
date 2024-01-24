using Boiler.Server.Models.Thermometer;
using Microsoft.EntityFrameworkCore;

namespace Boiler.Server.Services.Thermometer;

public class ThermometerService : IThermometerService
{
    private readonly ServerDbContext _dbContext;
    private readonly ILogger<ThermometerService> _logger;
    public ThermometerService(ServerDbContext dbContext, ILogger<ThermometerService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<ThermometerModel>?> GetThermometers(string userId)
    {
        try
        {
            var thermometers = await _dbContext.Thermometer.Where(x => x.UserId == userId).ToListAsync();
            return thermometers;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return null;
    }

    public async Task<bool> CreateThermometer(ThermometerModel thermometer)
    {
        try
        {
            _dbContext.Thermometer.Add(thermometer);
            await _dbContext.SaveChangesAsync();
            return true;
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return false;
    }

    public async Task<bool> DeleteThermometer(string thermometerId, string userId)
    {
        try { 
            var record = await _dbContext.Thermometer.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == thermometerId);
            if (record == null)
                return false;
            _dbContext.Thermometer.Remove(record);
            _dbContext.SaveChanges();
        } catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
        return false;
    }
}
