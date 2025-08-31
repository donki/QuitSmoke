using QuitSmoke.Models;

namespace QuitSmoke.Services;

public interface ISmokingDataService
{
    Task<SmokingData> GetDataAsync();
    Task SaveDataAsync(SmokingData data);
    Task AddSmokedCigaretteAsync();
    Task UpdateMaxCigarettesAsync(int maxCigarettes);
    Task UpdateScheduleAsync(TimeSpan wakeUpTime, TimeSpan sleepTime);

    // Historial
    Task<List<DailySmokingRecord>> GetHistoryAsync(int days = 30);
}