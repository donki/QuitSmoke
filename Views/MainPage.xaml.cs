using QuitSmoke.Services;
using QuitSmoke.Models;
using QuitSmoke.Helpers;

namespace QuitSmoke.Views;

public partial class MainPage : ContentPage
{
    private readonly ISmokingDataService _smokingDataService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _loc;
    private readonly UpdateService _updateService;
    private SmokingData _smokingData = new();

    public MainPage()
    {
        InitializeComponent();
        _smokingDataService = ServiceHelper.GetService<ISmokingDataService>()!;
        _notificationService = ServiceHelper.GetService<INotificationService>()!;
        _loc = ServiceHelper.GetService<ILocalizationService>();
        _updateService = ServiceHelper.GetService<UpdateService>();

        ApplyLocalization();
        _ = LoadDataAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();
        await LoadDataAsync();

        // §15: comprobacion de version al arrancar, no bloqueante y silenciosa.
        _ = _updateService.CheckAndPromptAsync(this);
    }

    private void ApplyLocalization()
    {
        string L(string key) => _loc.GetString(key);
        TipSmartLabel.Text = L("main_tip_smart");
        DailyStatsLabel.Text = L("main_daily_stats");
        RefreshBtn.Text = L("main_refresh");
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _smokingData = await _smokingDataService.GetDataAsync();
            UpdateUI();
            ShowRandomTip();

            // Mantener la notificación persistente (con el botón "🚬 Fumar") al día.
            await _notificationService.UpdatePersistentStatusAsync(_smokingData);
        }
        catch (Exception ex)
        {
            await SocShared.ModernDialog.AlertAsync(this,_loc.GetString("error"), $"{_loc.GetString("main_error_loading")}: {ex.Message}", _loc.GetString("ok"));
        }
    }

    private void UpdateUI()
    {
        string L(string key) => _loc.GetString(key);
        var na = L("main_placeholder");

        // Actualizar progreso
        var progress = _smokingData.MaxCigarettesPerDay > 0
            ? (double)_smokingData.SmokedToday / _smokingData.MaxCigarettesPerDay
            : 0;
        DayProgressBar.Progress = Math.Min(progress, 1.0);

        ProgressLabel.Text = $"{L("main_smoked")}: {_smokingData.SmokedToday}/{_smokingData.MaxCigarettesPerDay}";
        RemainingLabel.Text = $"{L("main_remaining")}: {Math.Max(0, _smokingData.RemainingCigarettes)}";

        // Actualizar tiempos
        var lastSmoke = _smokingData.SmokingTimes.LastOrDefault();
        if (lastSmoke != default)
        {
            LastCigaretteLabel.Text = $"{L("main_last")}: {lastSmoke:HH:mm}";
            var timeSince = DateTime.Now - lastSmoke;
            TimeSinceLastLabel.Text = $"{L("main_time_since")}: {FormatTimeSpan(timeSince)}";
        }
        else
        {
            LastCigaretteLabel.Text = $"{L("main_last")}: {na}";
            TimeSinceLastLabel.Text = $"{L("main_time_since")}: {na}";
        }

        var nextTime = _smokingData.NextRecommendedTime;
        NextCigaretteLabel.Text = nextTime.HasValue
            ? $"{L("main_next")}: {nextTime.Value:HH:mm}"
            : $"{L("main_next")}: {na}";

        // Actualizar estadísticas
        TimeBetweenLabel.Text = $"{L("main_time_between")}: {_smokingData.TimeBetweenCigarettes:hh\\:mm}";
        AwakeHoursLabel.Text = $"{L("main_awake_hours")}: {_smokingData.AwakeHours:hh\\:mm}";

        // Actualizar botón
        SmokeBtn.IsEnabled = true;
        if (_smokingData.SmokedToday >= _smokingData.MaxCigarettesPerDay)
        {
            SmokeBtn.Text = L("main_limit_reached_button");
            SmokeBtn.BackgroundColor = (Color)Application.Current!.Resources["Accent"];
        }
        else
        {
            SmokeBtn.Text = L("main_smoke_button");
            SmokeBtn.BackgroundColor = (Color)Application.Current!.Resources["Primary"];
        }
    }

    private void ShowRandomTip()
    {
        // Consejos en el idioma configurado (antes eran 5 literales en español; ahora usa el
        // catálogo completo de SmokingTips localizado es/en).
        var tips = Models.SmokingTips.GetAllTips(_loc.GetCurrentLanguage());
        if (tips.Count == 0)
            return;
        var tip = tips[new Random().Next(tips.Count)];
        TipIcon.Text = tip.Icon;
        TipTitle.Text = tip.Title;
        TipText.Text = tip.Message;
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalMinutes < 1)
            return "< 1 min";
        if (timeSpan.TotalHours < 1)
            return $"{(int)timeSpan.TotalMinutes} min";
        if (timeSpan.TotalDays < 1)
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}min";

        return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h";
    }

    private async void OnSmokeClicked(object sender, EventArgs e)
    {
        string L(string key) => _loc.GetString(key);
        try
        {
            if (_smokingData.SmokedToday >= _smokingData.MaxCigarettesPerDay)
            {
                var confirm = await SocShared.ModernDialog.AlertAsync(this,
                    L("main_limit_title"),
                    string.Format(L("main_limit_question"), _smokingData.MaxCigarettesPerDay),
                    L("main_limit_yes"),
                    L("main_limit_no"));

                if (!confirm) return;

                var doubleConfirm = await SocShared.ModernDialog.AlertAsync(this,
                    L("main_confirm_title"),
                    L("main_confirm_question"),
                    L("main_confirm_yes"),
                    L("cancel"));

                if (!doubleConfirm) return;
            }

            await _smokingDataService.AddSmokedCigaretteAsync();
            await LoadDataAsync();

            await SocShared.ModernDialog.AlertAsync(this,L("main_registered_title"), L("main_registered_message"), L("ok"));
        }
        catch (Exception ex)
        {
            await SocShared.ModernDialog.AlertAsync(this,L("error"), $"{L("main_error_register")}: {ex.Message}", L("ok"));
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadDataAsync();
        ShowRandomTip();
    }
}
