using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using AndroidView = Android.Views.View;

namespace QuitSmoke;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
      ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Mantener la pantalla encendida mientras se prueba por USB.
        Window?.AddFlags(WindowManagerFlags.KeepScreenOn);

        // Android 15 dibuja de borde a borde: se separa el contenido con el tamaño real de las
        // barras de estado y navegación (antes se solapaban con el reloj y la barra inferior).
        ApplySystemBarInsets();
    }

    private void ApplySystemBarInsets()
    {
        var content = FindViewById(global::Android.Resource.Id.Content);
        if (content is null)
            return;

        content.SetBackgroundColor(global::Android.Graphics.Color.ParseColor("#2A1CB8")); // indigo de marca

        ViewCompat.SetOnApplyWindowInsetsListener(content, new SystemBarInsetsListener());

        var controller = Window is not null
            ? WindowCompat.GetInsetsController(Window, Window.DecorView)
            : null;
        if (controller is not null)
        {
            controller.AppearanceLightStatusBars = false;
            controller.AppearanceLightNavigationBars = false;
        }
    }

    private class SystemBarInsetsListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat OnApplyWindowInsets(AndroidView? view, WindowInsetsCompat? insets)
        {
            var consumed = WindowInsetsCompat.Consumed!;
            if (view is null || insets is null)
                return consumed;
            var bars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout());
            if (bars is not null)
                view.SetPadding(bars.Left, bars.Top, bars.Right, bars.Bottom);
            return consumed;
        }
    }
}
