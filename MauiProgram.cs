using Microsoft.Extensions.Logging;
using QuitSmoke.Views;
using QuitSmoke.Services;
using QuitSmoke.Helpers;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using IAppNotificationService = QuitSmoke.Services.INotificationService;

namespace QuitSmoke;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Gestor global de excepciones (General 6.12): un error inesperado se registra y se avisa en
        // el idioma elegido en la app (no toca la cultura), y la app sigue.
        SocShared.CrashGuard.Install("QuitSmoke", language: () =>
            IPlatformApplication.Current?.Services.GetService<ILocalizationService>()?.GetCurrentLanguage());

        var builder = MauiApp.CreateBuilder();
        // La categoría y el canal de notificaciones se registran antes que el contenedor de
        // servicios: se usa una instancia propia del servicio de idioma (lee el idioma guardado).
        var startupLoc = new LocalizationService();
        // Tipografia del sistema (A.9): no se embeben familias propias.
        builder
            .UseMauiApp<App>()
            .UseLocalNotification(config =>
            {
                // Categoría "Status" con el botón "Fumar": permite registrar un cigarro
                // directamente desde la notificación persistente, sin abrir la app.
                config.AddCategory(new NotificationCategory(NotificationCategoryType.Status)
                {
                    ActionList = new HashSet<NotificationAction>
                    {
                        new NotificationAction(Services.NotificationService.SmokeActionId)
                        {
                            Title = startupLoc.GetString("notif_smoke_action"),
                            Android = new AndroidAction
                            {
                                LaunchAppWhenTapped = false
                            }
                        }
                    }
                });

                // Canal de la notificación de estado con importancia HIGH: sin esto (canal DEFAULT
                // auto-creado) MIUI/One UI ocultan la fila de acciones y el botón "Fumar" no salía.
                // Sin sonido/vibración para que las actualizaciones no molesten.
                config.AddAndroid(android =>
                    android.AddChannel(new NotificationChannelRequest
                    {
                        Id = "quit_smoke_status_v2",
                        Name = startupLoc.GetString("notif_channel_status"),
                        Importance = AndroidImportance.High,
                        EnableSound = false,
                        EnableVibration = false,
                    }));
            });



        // Services
        builder.Services.AddSingleton<ISmokingDataService, SmokingDataService>();
        builder.Services.AddSingleton<IAppNotificationService, NotificationService>();
        builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
        builder.Services.AddSingleton<UpdateService>();
#if ANDROID
        builder.Services.AddSingleton<IPowerSettingsService, QuitSmoke.Platforms.Android.Services.PowerSettingsService>();
        builder.Services.AddSingleton<IScreenService, QuitSmoke.Platforms.Android.Services.ScreenService>();
        builder.Services.AddSingleton<IEmailService, QuitSmoke.Platforms.Android.Services.EmailService>();
#endif

        // Views
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<HistoryPage>();
        
        // Pages
        builder.Services.AddTransient<QuitSmoke.Pages.AboutPage>();

#if DEBUG
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        var app = builder.Build();
        ServiceHelper.Initialize(app.Services);
        return app;
    }
}