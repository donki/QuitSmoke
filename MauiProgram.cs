using Microsoft.Extensions.Logging;
using QuitSmoke.Views;
using QuitSmoke.Services;
using QuitSmoke.Helpers;
using Plugin.LocalNotification;
using IAppNotificationService = QuitSmoke.Services.INotificationService;

namespace QuitSmoke;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        // Tipografia del sistema (A.9): no se embeben familias propias.
        builder
            .UseMauiApp<App>()
            .UseLocalNotification();



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