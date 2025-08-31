namespace QuitSmoke.Services;

public interface IPowerSettingsService
{
    bool IsIgnoringBatteryOptimizations();
    Task<bool> RequestIgnoreBatteryOptimizationsAsync();
    void OpenBatteryOptimizationSettings();
    void OpenAutostartSettings();
    void OpenBackgroundSettings();
}
