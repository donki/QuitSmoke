using QuitSmoke.Models;
using System.Text.Json;

namespace QuitSmoke.Services;

public class SmokingDataService : ISmokingDataService
{
    private readonly string _dataPath;
    private SmokingData? _cachedData;

    // Nuevo: historial por día
    private readonly string _historyPath;

    public SmokingDataService()
    {
        _dataPath = Path.Combine(FileSystem.AppDataDirectory, "smoking_data.json");
        _historyPath = Path.Combine(FileSystem.AppDataDirectory, "smoking_history.json");
    }

    public async Task<SmokingData> GetDataAsync()
    {
        if (_cachedData != null)
        {
            // Reset daily data if it's a new day
            if (_cachedData.LastResetDate.Date < DateTime.Today)
            {
                await PersistTodayIntoHistoryAsync(_cachedData);
                _cachedData.SmokedToday = 0;
                _cachedData.SmokingTimes.Clear();
                _cachedData.LastResetDate = DateTime.Today;
                await SaveDataAsync(_cachedData);
            }

            // Normalizar inconsistencias
            if (NormalizeTodayData(_cachedData))
            {
                await SaveDataAsync(_cachedData);
            }
            return _cachedData;
        }

        try
        {
            if (File.Exists(_dataPath))
            {
                var json = await File.ReadAllTextAsync(_dataPath);
                _cachedData = JsonSerializer.Deserialize<SmokingData>(json) ?? new SmokingData();
            }
            else
            {
                _cachedData = new SmokingData();
            }

            // Reset daily data if it's a new day
            if (_cachedData.LastResetDate.Date < DateTime.Today)
            {
                await PersistTodayIntoHistoryAsync(_cachedData);
                _cachedData.SmokedToday = 0;
                _cachedData.SmokingTimes.Clear();
                _cachedData.LastResetDate = DateTime.Today;
                await SaveDataAsync(_cachedData);
            }

            // Normalizar inconsistencias
            if (NormalizeTodayData(_cachedData))
            {
                await SaveDataAsync(_cachedData);
            }

            return _cachedData;
        }
        catch
        {
            _cachedData = new SmokingData();
            return _cachedData;
        }
    }

    public async Task SaveDataAsync(SmokingData data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_dataPath, json);
            _cachedData = data;
        }
        catch
        {
            // Handle save error silently for now
        }
    }

    public async Task AddSmokedCigaretteAsync()
    {
        var data = await GetDataAsync();
        if (data.SmokedToday < data.MaxCigarettesPerDay)
        {
            var now = DateTime.Now;
            data.SmokingTimes.Add(now);
            data.SmokedToday = data.SmokingTimes.Count; // mantener sincronizado
            await SaveDataAsync(data);
            await AppendToHistoryAsync(now);
        }
    }

    public async Task UpdateMaxCigarettesAsync(int maxCigarettes)
    {
        var data = await GetDataAsync();
        data.MaxCigarettesPerDay = Math.Max(1, maxCigarettes);
        await SaveDataAsync(data);
    }

    public async Task UpdateScheduleAsync(TimeSpan wakeUpTime, TimeSpan sleepTime)
    {
        var data = await GetDataAsync();
        data.WakeUpTime = wakeUpTime;
        data.SleepTime = sleepTime;
        await SaveDataAsync(data);
    }

    // ===== Historial =====
    public async Task<List<DailySmokingRecord>> GetHistoryAsync(int days = 30)
    {
        try
        {
            if (!File.Exists(_historyPath))
                return new();

            var json = await File.ReadAllTextAsync(_historyPath);
            var all = JsonSerializer.Deserialize<List<DailySmokingRecord>>(json) ?? new();
            var fromDate = DateTime.Today.AddDays(-days + 1);
            return all.Where(r => r.Date.Date >= fromDate).OrderBy(r => r.Date).ToList();
        }
        catch
        {
            return new();
        }
    }

    private async Task AppendToHistoryAsync(DateTime time)
    {
        var all = await GetHistoryAsync(3650); // up to 10 years
        var day = time.Date;
        var existing = all.FirstOrDefault(r => r.Date.Date == day);
        if (existing == null)
        {
            existing = new DailySmokingRecord { Date = day };
            all.Add(existing);
        }
        existing.Times.Add(time);
        existing.Count = existing.Times.Count;
        await SaveHistoryAsync(all);
    }

    private async Task PersistTodayIntoHistoryAsync(SmokingData data)
    {
        if (data.SmokedToday <= 0 && data.SmokingTimes.Count == 0) return;
        var all = await GetHistoryAsync(3650);
        var day = data.LastResetDate.Date;
        var existing = all.FirstOrDefault(r => r.Date.Date == day) ?? new DailySmokingRecord { Date = day };
        existing.Times = data.SmokingTimes.ToList();
        existing.Count = existing.Times.Count;
        all.RemoveAll(r => r.Date.Date == day);
        all.Add(existing);
        await SaveHistoryAsync(all);
    }

    private async Task SaveHistoryAsync(List<DailySmokingRecord> all)
    {
        try
        {
            var json = JsonSerializer.Serialize(all.OrderBy(r => r.Date).ToList(), new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_historyPath, json);
        }
        catch
        {
            // ignore
        }
    }

    // Normaliza que SmokedToday refleje la cantidad de registros de hoy y que solo haya tiempos de hoy
    private bool NormalizeTodayData(SmokingData data)
    {
        bool changed = false;
        var today = DateTime.Today;

        // Filtrar solo tiempos de hoy
        if (data.SmokingTimes.Any(t => t.Date != today))
        {
            data.SmokingTimes = data.SmokingTimes.Where(t => t.Date == today).ToList();
            changed = true;
        }

        var count = data.SmokingTimes.Count;
        if (data.SmokedToday != count)
        {
            data.SmokedToday = count;
            changed = true;
        }

        if (data.LastResetDate.Date != today)
        {
            data.LastResetDate = today;
            changed = true;
        }

        return changed;
    }
}