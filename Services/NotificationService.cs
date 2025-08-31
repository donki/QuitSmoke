using QuitSmoke.Models;
using System.Text.Json;
using Plugin.LocalNotification;

namespace QuitSmoke.Services;

public class NotificationService : INotificationService
{
    private readonly string _tipsHistoryPath;
    private readonly Random _random = new();
    private List<SmokingTip>? _cachedTipsHistory;

    private const int PersistentStatusNotificationId = 2000;
    private const string StatusChannelId = "quit_smoke_status";

    public NotificationService()
    {
        _tipsHistoryPath = Path.Combine(FileSystem.AppDataDirectory, "tips_history.json");
    }

    public async Task<bool> RequestPermissionAsync()
    {
        try
        {
            var result = await LocalNotificationCenter.Current.RequestNotificationPermission();
            return result;
        }
        catch
        {
            return false;
        }
    }

    public async Task ShowSmokingAvailableNotificationAsync()
    {
        try
        {
            var tip = GetRandomTip();
            await SaveTipShownAsync(tip);

            var request = new NotificationRequest
            {
                NotificationId = 1001,
                Title = "🚬 Puedes fumar ahora",
                Subtitle = tip.Title,
                Description = tip.Message,
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
        catch
        {
            // Manejar error silenciosamente
        }
    }

    public async Task ScheduleNextNotificationAsync(DateTime nextTime)
    {
        try
        {
            var tip = GetRandomTip();
            var request = new NotificationRequest
            {
                NotificationId = 1002,
                Title = "⏰ Próximo cigarro",
                Subtitle = tip.Title,
                Description = $"Podrás fumar a las {nextTime:HH:mm}. {tip.Message}",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = nextTime,
                    RepeatType = NotificationRepeat.No
                }
            };
            await LocalNotificationCenter.Current.Show(request);
        }
        catch
        {
            // Manejar error silenciosamente
        }
    }

    public async Task UpdatePersistentStatusAsync(SmokingData data)
    {
        try
        {
            string title = $"QuitSmoke: {data.SmokedToday}/{data.MaxCigarettesPerDay} hoy";
            string desc;
            if (data.NextRecommendedTime.HasValue)
            {
                var next = data.NextRecommendedTime.Value;
                if (next <= DateTime.Now)
                    desc = $"Siguiente: ahora ({DateTime.Now:HH:mm})";
                else
                    desc = $"Siguiente: {next:HH:mm}";
            }
            else
            {
                desc = "Límite diario alcanzado";
            }

            var request = new NotificationRequest
            {
                NotificationId = PersistentStatusNotificationId,
                Title = title,
                Description = desc,
                Android = new()
                {
                    ChannelId = StatusChannelId,
                    AutoCancel = false,
                    Ongoing = true
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
        catch
        {
            // ignore
        }
    }

    public SmokingTip GetRandomTip()
    {
        var allTips = SmokingTips.GetAllTips();
        var recentTips = GetRecentTipsSync();
        var availableTips = allTips.Where(t => !recentTips.Contains(t.Message)).ToList();
        if (!availableTips.Any())
            availableTips = allTips;
        return availableTips[_random.Next(availableTips.Count)];
    }

    private HashSet<string> GetRecentTipsSync()
    {
        try
        {
            if (_cachedTipsHistory != null)
            {
                return _cachedTipsHistory
                    .Where(t => DateTime.Now.Subtract(DateTime.Today).TotalDays <= 7)
                    .Select(t => t.Message)
                    .ToHashSet();
            }

            if (File.Exists(_tipsHistoryPath))
            {
                var json = File.ReadAllText(_tipsHistoryPath);
                var history = JsonSerializer.Deserialize<List<TipHistoryEntry>>(json) ?? new();
                _cachedTipsHistory = history.Select(h => new SmokingTip
                {
                    Icon = h.Icon,
                    Title = h.Title,
                    Message = h.Message
                }).ToList();
                return _cachedTipsHistory.Select(t => t.Message).ToHashSet();
            }
        }
        catch
        {
        }
        return new HashSet<string>();
    }

    public async Task<List<SmokingTip>> GetTipsHistoryAsync()
    {
        try
        {
            if (_cachedTipsHistory != null)
                return _cachedTipsHistory;

            if (File.Exists(_tipsHistoryPath))
            {
                var json = await File.ReadAllTextAsync(_tipsHistoryPath);
                var history = JsonSerializer.Deserialize<List<TipHistoryEntry>>(json) ?? new();
                _cachedTipsHistory = history.Select(h => new SmokingTip
                {
                    Icon = h.Icon,
                    Title = h.Title,
                    Message = h.Message
                }).ToList();
                return _cachedTipsHistory;
            }

            _cachedTipsHistory = new();
            return _cachedTipsHistory;
        }
        catch
        {
            _cachedTipsHistory = new();
            return _cachedTipsHistory;
        }
    }

    public async Task SaveTipShownAsync(SmokingTip tip)
    {
        try
        {
            var history = new List<TipHistoryEntry>();
            if (File.Exists(_tipsHistoryPath))
            {
                var json = await File.ReadAllTextAsync(_tipsHistoryPath);
                history = JsonSerializer.Deserialize<List<TipHistoryEntry>>(json) ?? new();
            }
            history.Add(new TipHistoryEntry
            {
                Date = DateTime.Now,
                Icon = tip.Icon,
                Title = tip.Title,
                Message = tip.Message
            });
            var newJson = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_tipsHistoryPath, newJson);
            _cachedTipsHistory = history.Select(h => new SmokingTip
            {
                Icon = h.Icon,
                Title = h.Title,
                Message = h.Message
            }).ToList();
        }
        catch
        {
            // ignore
        }
    }

    private record TipHistoryEntry
    {
        public DateTime Date { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}