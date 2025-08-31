using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuitSmoke.Models;
using QuitSmoke.Services;
using System.Text;
using System.Timers;

namespace QuitSmoke.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly ISmokingDataService _smokingDataService;
    private readonly INotificationService _notificationService;
    private readonly System.Timers.Timer _timer;

    [ObservableProperty]
    private SmokingData _smokingData = new();

    [ObservableProperty]
    private string _nextRecommendedTimeText = "";

    [ObservableProperty]
    private string _lastCigaretteTimeText = "";

    [ObservableProperty]
    private bool _canSmoke = true;

    [ObservableProperty]
    private string _progressText = "";

    [ObservableProperty]
    private string _randomTipText = "";

    [ObservableProperty]
    private string _randomTipIcon = "";

    [ObservableProperty]
    private string _timeSinceLastSmokeText = "";

    [ObservableProperty]
    private string _healthBenefitsText = "";

    public double ProgressPercentage => SmokingData.MaxCigarettesPerDay > 0 
        ? (double)SmokingData.SmokedToday / SmokingData.MaxCigarettesPerDay 
        : 0;

    [ObservableProperty]
    private SmokingTip? _currentTip;

    [ObservableProperty]
    private bool _showTipDialog = false;

    public MainPageViewModel(ISmokingDataService smokingDataService, INotificationService notificationService)
    {
        _smokingDataService = smokingDataService;
        _notificationService = notificationService;
        _timer = new System.Timers.Timer(1000); // Update every second
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
        
        _ = LoadDataAsync();
        _ = _notificationService.RequestPermissionAsync();
        _ = LoadRandomTipAsync();
    }

    private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(UpdateTimeDisplay);
    }

    private async Task LoadDataAsync()
    {
        SmokingData = await _smokingDataService.GetDataAsync();
        UpdateDisplay();
        await _notificationService.UpdatePersistentStatusAsync(SmokingData);
    }

    private async void UpdateDisplay()
    {
        UpdateTimeDisplay();
        UpdateProgressText();
        UpdateLastCigaretteTime();
        CanSmoke = SmokingData.RemainingCigarettes > 0;
        await _notificationService.UpdatePersistentStatusAsync(SmokingData);
    }

    private void UpdateTimeDisplay()
    {
        if (SmokingData.NextRecommendedTime.HasValue)
        {
            var nextTime = SmokingData.NextRecommendedTime.Value;
            var now = DateTime.Now;
            
            if (nextTime <= now)
            {
                NextRecommendedTimeText = $"Próximo cigarro: ahora ({now:dd/MM/yyyy HH:mm})";
                _ = _notificationService.ShowSmokingAvailableNotificationAsync();
            }
            else
            {
                NextRecommendedTimeText = $"Próximo cigarro: {nextTime:dd/MM/yyyy HH:mm}";
                _ = _notificationService.ScheduleNextNotificationAsync(nextTime);
            }
        }
        else
        {
            NextRecommendedTimeText = "Límite diario alcanzado";
        }
    }

    private void UpdateProgressText()
    {
        var percentage = SmokingData.MaxCigarettesPerDay > 0 
            ? (double)SmokingData.SmokedToday / SmokingData.MaxCigarettesPerDay * 100 
            : 0;
        
        ProgressText = $"{SmokingData.SmokedToday} / {SmokingData.MaxCigarettesPerDay} cigarros ({percentage:F0}%)";
        OnPropertyChanged(nameof(ProgressPercentage));
    }

    private void UpdateLastCigaretteTime()
    {
        if (SmokingData.SmokingTimes.Any())
        {
            var lastTime = SmokingData.SmokingTimes.Last();
            LastCigaretteTimeText = $"Último cigarro: {lastTime:dd/MM/yyyy HH:mm}";

            var since = DateTime.Now - lastTime;
            TimeSinceLastSmokeText = $"Tiempo sin fumar: {FormatTimeSpan(since)}";
            HealthBenefitsText = ComputeHealthBenefits(since);
        }
        else
        {
            LastCigaretteTimeText = "Último cigarro: No hay registros";
            TimeSinceLastSmokeText = "Tiempo sin fumar: --";
            HealthBenefitsText = "Sin datos sobre beneficios. Registra tu primer cigarro o comienza ahora.";
        }
    }

    private string FormatTimeSpan(TimeSpan span)
    {
        if (span.TotalSeconds < 60)
            return $"{(int)span.TotalSeconds} seg";

        var parts = new List<string>();
        if (span.Days > 0) parts.Add($"{span.Days} d");
        if (span.Hours > 0) parts.Add($"{span.Hours} h");
        if (span.Minutes > 0) parts.Add($"{span.Minutes} min");
        if (parts.Count == 0) parts.Add("<1 min");
        return string.Join(", ", parts);
    }

    private string ComputeHealthBenefits(TimeSpan elapsed)
    {
        var milestones = new List<(TimeSpan Threshold, string Description)>
        {
            (TimeSpan.FromMinutes(20), "20 minutos: Pulso y presión arterial comienzan a normalizarse."),
            (TimeSpan.FromHours(8), "8 horas: Niveles de oxígeno aumentan y monóxido de carbono disminuye."),
            (TimeSpan.FromDays(1), "24 horas: El riesgo de ataque cardiaco comienza a disminuir."),
            (TimeSpan.FromDays(2), "48 horas: Mejora del sentido del gusto y olfato; comienzan a regenerarse las terminaciones nerviosas."),
            (TimeSpan.FromDays(14), "2 semanas: Mejora la circulación y la capacidad pulmonar."),
            (TimeSpan.FromDays(90), "2-3 meses: Mejor notable en la respiración y la capacidad física."),
            (TimeSpan.FromDays(365), "1 año: Riesgo de enfermedad coronaria reducido a la mitad comparado con un fumador."),
            (TimeSpan.FromDays(365*5), "5 años: Riesgo de accidente cerebrovascular disminuye significativamente.")
        };

        var sb = new StringBuilder();
        sb.AppendLine("Beneficios de salud logrados:");

        foreach (var m in milestones)
        {
            var achieved = elapsed >= m.Threshold;
            sb.AppendLine($"{(achieved ? "✓" : "○")} {m.Description}");
        }

        return sb.ToString().TrimEnd();
    }

    private Task LoadRandomTipAsync()
    {
        var tip = _notificationService.GetRandomTip();
        RandomTipText = tip.Message;
        RandomTipIcon = tip.Icon;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SmokeAsync()
    {
        // Confirmación con consejo
        var tip = _notificationService.GetRandomTip();
        var body = $"{tip.Icon} {tip.Title}\n\n{tip.Message}\n\n¿Deseas fumar ahora?";
        var confirm = await Shell.Current.DisplayAlert("Antes de fumar", body, "Fumar", "Cancelar");
        if (!confirm)
            return;

        if (SmokingData.RemainingCigarettes > 0)
        {
            // Registrar cigarro
            await _smokingDataService.AddSmokedCigaretteAsync();

            // Recalcular y refrescar
            SmokingData = await _smokingDataService.GetDataAsync();
            UpdateDisplay();
            await LoadRandomTipAsync();
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDataAsync();
        await LoadRandomTipAsync();
    }

    private async Task ShowSmokingTipAsync()
    {
        if (!ShowTipDialog && CanSmoke)
        {
            CurrentTip = _notificationService.GetRandomTip();
            ShowTipDialog = false; // no modal
            await _notificationService.SaveTipShownAsync(CurrentTip);
            await _notificationService.ShowSmokingAvailableNotificationAsync();
        }
    }

    [RelayCommand]
    private void CloseTipDialog()
    {
        ShowTipDialog = false;
        CurrentTip = null;
    }

    [RelayCommand]
    private async Task SmokeAnywayAsync()
    {
        ShowTipDialog = false;
        CurrentTip = null;
        await SmokeAsync();
    }

    [RelayCommand]
    private void DelaySmoking()
    {
        ShowTipDialog = false;
        CurrentTip = null;
    }
}