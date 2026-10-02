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

        // Botón "Fumar" de la notificación persistente: registra un cigarro sin abrir la app.
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

    private static Task InitPersistentNotificationAsync() => NotificationActions.InitAsync(
        ServiceHelper.GetService<QuitSmoke.Services.INotificationService>(), ServiceHelper.GetService<ISmokingDataService>());

    private async void OnNotificationActionTapped(NotificationActionEventArgs e) =>
        await NotificationActions.HandleAsync(e.ActionId, ServiceHelper.GetService<ISmokingDataService>(),
            ServiceHelper.GetService<QuitSmoke.Services.INotificationService>());
}
