using QuitSmoke.Models;

namespace QuitSmoke.Services;

public interface ISmokingDataService
{
    Task<SmokingData> GetDataAsync();
    Task SaveDataAsync(SmokingData data);
    Task AddSmokedCigaretteAsync();
    Task UpdateMaxCigarettesAsync(int maxCigarettes);
    Task UpdateScheduleAsync(TimeSpan wakeUpTime, TimeSpan sleepTime);
    
    // Nuevos métodos para precios
    Task UpdatePriceConfigurationAsync(decimal packPrice, int cigarettesPerPack, string currency);
    Task AddSmokedCigaretteWithPriceAsync(decimal price, string currency);

    // Historial
    Task<List<DailySmokingRecord>> GetHistoryAsync(int days = 30);
}