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
        FooterVersionLabel.Text = $"v{AppInfo.Current.VersionString}";
    }

    /// <summary>
    /// Atrás (Mobile §7, referencia File Manager). Con targetSdk 36 hace falta
    /// enableOnBackInvokedCallback="false" en el manifiesto para que el botón llegue aquí. Orden:
    /// menú lateral abierto → se cierra; diálogo a la vista → se descarta; Historial, Configuración o
    /// Acerca de → vuelve a Inicio; en Inicio la aplicación pasa a segundo plano (no se cierra).
    /// </summary>
    protected override bool OnBackButtonPressed()
    {
        if (FlyoutIsPresented)
        {
            FlyoutIsPresented = false;
            return true;
        }

        if (CurrentPage is ContentPage page && DialogBack.TryDismiss(page))
            return true;

        if (Navigation.NavigationStack.Count > 1 || Navigation.ModalStack.Count > 0)
            return base.OnBackButtonPressed();

        if (CurrentItem != HomeItem)
        {
            CurrentItem = HomeItem;
            return true;
        }

#if ANDROID
        Platform.CurrentActivity?.MoveTaskToBack(true);
        return true;
#else
        return base.OnBackButtonPressed();
#endif
    }
}
