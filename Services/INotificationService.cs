using QuitSmoke.Models;

namespace QuitSmoke.Services;

public interface INotificationService
{
    Task<bool> RequestPermissionAsync();
    Task ShowSmokingAvailableNotificationAsync();
    Task ScheduleNextNotificationAsync(DateTime nextTime);
    Task UpdatePersistentStatusAsync(SmokingData data);
    SmokingTip GetRandomTip();
    Task<List<SmokingTip>> GetTipsHistoryAsync();
    Task SaveTipShownAsync(SmokingTip tip);
}