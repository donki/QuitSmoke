using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using QuitSmoke.Services;
using QuitSmoke.Helpers;

namespace QuitSmoke;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Botón "🚬 Fumar" de la notificación persistente: registra un cigarro sin abrir la app.
        LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());
#if DEBUG
        SocShared.AuthorNotes.Attach(window);   // notas de autor: SOLO Debug, desactivado en Release/produccion
#endif

        // Pedir permiso de notificaciones y mostrar/actualizar la notificación persistente al arrancar.
        _ = InitPersistentNotificationAsync();
        return window;
    }

    private static async Task InitPersistentNotificationAsync()
    {
        try
        {
            var notif = ServiceHelper.GetService<QuitSmoke.Services.INotificationService>();
            var dataService = ServiceHelper.GetService<ISmokingDataService>();
            if (notif is null || dataService is null) return;

            await notif.RequestPermissionAsync();
            var data = await dataService.GetDataAsync();
            await notif.UpdatePersistentStatusAsync(data);
        }
        catch
        {
            // no bloquear el arranque si falla la notificación
        }
    }

    private async void OnNotificationActionTapped(NotificationActionEventArgs e)
    {
        if (e.ActionId != NotificationService.SmokeActionId) return;
        try
        {
            var dataService = ServiceHelper.GetService<ISmokingDataService>();
            var notif = ServiceHelper.GetService<QuitSmoke.Services.INotificationService>();
            if (dataService is null || notif is null) return;

            await dataService.AddSmokedCigaretteAsync();
            var data = await dataService.GetDataAsync();
            await notif.UpdatePersistentStatusAsync(data);
        }
        catch
        {
            // ignore
        }
    }
}
