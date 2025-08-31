using AContext = Android.Content.Context;
using Android.Content;
using Android.OS;
using Android.Provider;
using QuitSmoke.Services;

namespace QuitSmoke.Platforms.Android.Services;

public class PowerSettingsService : IPowerSettingsService
{
    private readonly AContext _context;

    public PowerSettingsService()
    {
        _context = global::Android.App.Application.Context;
    }

    public bool IsIgnoringBatteryOptimizations()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.M) return true;
        var pm = (PowerManager)_context.GetSystemService(AContext.PowerService)!;
        return pm.IsIgnoringBatteryOptimizations(_context.PackageName);
    }

    public async Task<bool> RequestIgnoreBatteryOptimizationsAsync()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.M) return true;
        if (IsIgnoringBatteryOptimizations()) return true;

        try
        {
            var intent = new Intent(Settings.ActionRequestIgnoreBatteryOptimizations);
            intent.SetData(global::Android.Net.Uri.Parse("package:" + _context.PackageName));
            intent.AddFlags(ActivityFlags.NewTask);
            _context.StartActivity(intent);
            await Task.Delay(500);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void OpenBatteryOptimizationSettings()
    {
        try
        {
            var intent = new Intent(Settings.ActionIgnoreBatteryOptimizationSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            _context.StartActivity(intent);
        }
        catch { }
    }

    public void OpenAutostartSettings()
    {
        try
        {
            var intent = new Intent();
            intent.AddFlags(ActivityFlags.NewTask);
            intent.SetComponent(new ComponentName("com.miui.securitycenter", "com.miui.permcenter.autostart.AutoStartManagementActivity"));
            _context.StartActivity(intent);
        }
        catch
        {
            try
            {
                var intent = new Intent(Settings.ActionApplicationDetailsSettings);
                intent.SetData(global::Android.Net.Uri.Parse("package:" + _context.PackageName));
                intent.AddFlags(ActivityFlags.NewTask);
                _context.StartActivity(intent);
            }
            catch { }
        }
    }

    public void OpenBackgroundSettings()
    {
        try
        {
            var intent = new Intent(Settings.ActionApplicationDetailsSettings);
            intent.SetData(global::Android.Net.Uri.Parse("package:" + _context.PackageName));
            intent.AddFlags(ActivityFlags.NewTask);
            _context.StartActivity(intent);
        }
        catch { }
    }
}
