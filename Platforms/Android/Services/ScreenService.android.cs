using Android.Views;
using QuitSmoke.Services;

namespace QuitSmoke.Platforms.Android.Services;

public class ScreenService : IScreenService
{
    public void KeepScreenOn(bool keepOn)
    {
        var activity = Platform.CurrentActivity ?? Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity?.Window != null)
        {
            if (keepOn)
            {
                activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
            else
            {
                activity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
            }
        }
    }
}