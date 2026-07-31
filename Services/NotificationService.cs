using QuitSmoke.Models;
using System.Text.Json;
using Plugin.LocalNotification;

namespace QuitSmoke.Services;

public class NotificationService : INotificationService
{
    private readonly string _tipsHistoryPath;
    private readonly Random _random = new();
    private List<SmokingTip>? _cachedTipsHistory;
    private readonly ILocalizationService _loc;

    private const int PersistentStatusNotificationId = 2000;
    // Id v2: la importancia de un canal es inmutable tras crearse; con un id nuevo se aplica la
    // importancia High necesaria para que la notificación muestre el botón de acción "Fumar"
    // (en MIUI/One UI un canal DEFAULT ongoing oculta la fila de acciones).
    private const string StatusChannelId = "quit_smoke_status_v2";

    // Id de la acción "🚬 Fumar" del botón de la notificación persistente. La categoría con esta
    // acción se registra en MauiProgram y el tap se maneja en App.xaml.cs.
    public const int SmokeActionId = 100;

    public NotificationService(ILocalizationService loc)
    {
        _loc = loc;
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
                Title = _loc.GetString("notif_can_smoke"),
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
                Title = _loc.GetString("notif_next_title"),
                Subtitle = tip.Title,
                Description = string.Format(_loc.GetString("notif_next_body"), nextTime.ToString("HH:mm"), tip.Message),
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
            string title = string.Format(_loc.GetString("notif_status_title"), data.SmokedToday, data.MaxCigarettesPerDay);
            string desc;
            if (data.NextRecommendedTime.HasValue)
            {
                var next = data.NextRecommendedTime.Value;
                if (next <= DateTime.Now)
                    desc = string.Format(_loc.GetString("notif_next_now"), DateTime.Now.ToString("HH:mm"));
                else
                    desc = string.Format(_loc.GetString("notif_next_at"), next.ToString("HH:mm"));
            }
            else
            {
                desc = _loc.GetString("notif_limit");
            }

            var request = new NotificationRequest
            {
                NotificationId = PersistentStatusNotificationId,
                Title = title,
                Description = desc,
                // Categoría que aporta el botón de acción "🚬 Fumar" (registrada en MauiProgram).
                CategoryType = NotificationCategoryType.Status,
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
        var allTips = SmokingTips.GetAllTips(_loc.GetCurrentLanguage());
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