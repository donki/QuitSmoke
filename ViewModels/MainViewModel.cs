using QuitSmoke.Helpers;
using QuitSmoke.Models;
using QuitSmoke.Services;

namespace QuitSmoke.ViewModels;

/// <summary>Inicio: cigarros de hoy, tiempos, consejo y el boton de fumar.</summary>
public sealed class MainViewModel : ViewModelBase
{
    private readonly ISmokingDataService _data;
    private readonly INotificationService _notifications;
    private readonly Func<DateTime> _now;
    private readonly Random _random;
    private SmokingData _current = new();

    public MainViewModel(ISmokingDataService data, INotificationService notifications, ILocalizationService loc,
        Func<DateTime>? now = null, Random? random = null) : base(loc)
    {
        _data = data;
        _notifications = notifications;
        _now = now ?? (() => DateTime.Now);
        _random = random ?? new Random();
    }

    public SmokingData Data => _current;

    // Textos fijos
    public string TipSmart => L("main_tip_smart");
    public string DailyStats => L("main_daily_stats");
    public string RefreshText => L("main_refresh");

    // Consejo
    public string TipIcon { get; private set; } = "ic_tip_bulb.png";
    public string TipTitle { get; private set; } = "";
    public string TipText { get; private set; } = "";

    private string NotAvailable => L("main_placeholder");

    public double Progress => _current.MaxCigarettesPerDay > 0
        ? Math.Min((double)_current.SmokedToday / _current.MaxCigarettesPerDay, 1.0)
        : 0;

    public string ProgressText => $"{L("main_smoked")}: {_current.SmokedToday}/{_current.MaxCigarettesPerDay}";
    public string RemainingText => $"{L("main_remaining")}: {Math.Max(0, _current.RemainingCigarettes)}";

    private DateTime LastSmoke => _current.SmokingTimes.LastOrDefault();

    public string LastText => LastSmoke != default
        ? $"{L("main_last")}: {LastSmoke:HH:mm}"
        : $"{L("main_last")}: {NotAvailable}";

    public string SinceText => LastSmoke != default
        ? $"{L("main_time_since")}: {TimeFormat.Elapsed(_now() - LastSmoke)}"
        : $"{L("main_time_since")}: {NotAvailable}";

    public string NextText => _current.NextRecommendedTime is { } next
        ? $"{L("main_next")}: {next:HH:mm}"
        : $"{L("main_next")}: {NotAvailable}";

    public string TimeBetweenText => $"{L("main_time_between")}: {_current.TimeBetweenCigarettes:hh\\:mm}";
    public string AwakeText => $"{L("main_awake_hours")}: {_current.AwakeHours:hh\\:mm}";

    /// <summary>Ya se ha llegado al maximo de hoy: el boton cambia de texto y de color.</summary>
    public bool LimitReached => _current.SmokedToday >= _current.MaxCigarettesPerDay;

    public string SmokeButtonText => LimitReached ? L("main_limit_reached_button") : L("main_smoke_button");

    /// <summary>Lee los datos, pone un consejo y deja al dia la notificacion persistente (la del boton «Fumar»).</summary>
    public async Task LoadAsync(IUserDialogs dialogs)
    {
        try
        {
            _current = await _data.GetDataAsync();
            ShowRandomTip();
            await _notifications.UpdatePersistentStatusAsync(_current);
        }
        catch (Exception ex)
        {
            await dialogs.AlertAsync(L("error"), $"{L("main_error_loading")}: {ex.Message}", L("ok"));
        }
        RaiseAllChanged();
    }

    /// <summary>Un consejo al azar en el idioma configurado.</summary>
    public void ShowRandomTip()
    {
        var tips = SmokingTips.GetAllTips(Loc.GetCurrentLanguage());
        if (tips.Count == 0)
            return;
        var tip = tips[_random.Next(tips.Count)];
        (TipIcon, TipTitle, TipText) = (tip.Icon, tip.Title, tip.Message);
        RaiseAllChanged();
    }

    /// <summary>
    /// Apunta un cigarro. Pasado el maximo de hoy se pide confirmacion dos veces: es justo cuando
    /// mas cuesta pararse a pensarlo.
    /// </summary>
    public async Task SmokeAsync(IUserDialogs dialogs)
    {
        try
        {
            if (LimitReached)
            {
                if (!await dialogs.ConfirmAsync(L("main_limit_title"),
                        string.Format(L("main_limit_question"), _current.MaxCigarettesPerDay),
                        L("main_limit_yes"), L("main_limit_no")))
                    return;

                if (!await dialogs.ConfirmAsync(L("main_confirm_title"), L("main_confirm_question"),
                        L("main_confirm_yes"), L("cancel")))
                    return;
            }

            await _data.AddSmokedCigaretteAsync();
            await LoadAsync(dialogs);
            await dialogs.AlertAsync(L("main_registered_title"), L("main_registered_message"), L("ok"));
        }
        catch (Exception ex)
        {
            await dialogs.AlertAsync(L("error"), $"{L("main_error_register")}: {ex.Message}", L("ok"));
        }
    }

    public async Task RefreshAsync(IUserDialogs dialogs)
    {
        await LoadAsync(dialogs);
        ShowRandomTip();
    }
}
