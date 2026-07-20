using QuitSmoke.Pages;
using QuitSmoke.Services;
using QuitSmoke.Helpers;

namespace QuitSmoke;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rutas para navegación
        Routing.RegisterRoute("AboutPage", typeof(AboutPage));

        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        var loc = ServiceHelper.GetService<ILocalizationService>();
        HomeItem.Title = loc.GetString("nav_home");
        HistoryItem.Title = loc.GetString("nav_history");
        SettingsItem.Title = loc.GetString("nav_settings");
        AboutItem.Title = loc.GetString("nav_about");
        HeaderSubtitleLabel.Text = loc.GetString("flyout_subtitle");
        FooterVersionLabel.Text = $"v{AppInfo.Current.VersionString} - sOCratic";
    }
}
