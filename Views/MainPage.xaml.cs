using QuitSmoke.Services;
using QuitSmoke.Models;
using QuitSmoke.Helpers;

namespace QuitSmoke.Views;

public partial class MainPage : ContentPage
{
    private readonly ISmokingDataService _smokingDataService;
    private readonly INotificationService _notificationService;
    private SmokingData _smokingData = new();
    private readonly List<string> _tips = new()
    {
        "💪|Mantente fuerte|Cada cigarro que no fumas es una victoria",
        "🌟|Progreso gradual|Reducir poco a poco es mejor que no intentarlo",
        "💚|Tu salud mejora|Cada hora sin fumar beneficia tu cuerpo",
        "🎯|Enfócate en hoy|No pienses en mañana, solo en este momento",
        "🏆|Eres capaz|Ya has demostrado que puedes controlar tu consumo"
    };

    public MainPage()
    {
        InitializeComponent();
        _smokingDataService = ServiceHelper.GetService<ISmokingDataService>()!;
        _notificationService = ServiceHelper.GetService<INotificationService>()!;
        
        _ = LoadDataAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _smokingData = await _smokingDataService.GetDataAsync();
            UpdateUI();
            ShowRandomTip();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error cargando datos: {ex.Message}", "OK");
        }
    }

    private void UpdateUI()
    {
        // Actualizar progreso
        var progress = _smokingData.MaxCigarettesPerDay > 0 
            ? (double)_smokingData.SmokedToday / _smokingData.MaxCigarettesPerDay 
            : 0;
        DayProgressBar.Progress = Math.Min(progress, 1.0);
        
        ProgressLabel.Text = $"Fumados: {_smokingData.SmokedToday}/{_smokingData.MaxCigarettesPerDay}";
        RemainingLabel.Text = $"Restantes: {Math.Max(0, _smokingData.RemainingCigarettes)}";

        // Actualizar tiempos
        var lastSmoke = _smokingData.SmokingTimes.LastOrDefault();
        if (lastSmoke != default)
        {
            LastCigaretteLabel.Text = $"Último: {lastSmoke:HH:mm}";
            var timeSince = DateTime.Now - lastSmoke;
            TimeSinceLastLabel.Text = $"Sin fumar: {FormatTimeSpan(timeSince)}";
        }
        else
        {
            LastCigaretteLabel.Text = "Último: --";
            TimeSinceLastLabel.Text = "Sin fumar: --";
        }

        var nextTime = _smokingData.NextRecommendedTime;
        NextCigaretteLabel.Text = nextTime.HasValue 
            ? $"Próximo: {nextTime.Value:HH:mm}" 
            : "Próximo: --";

        // Actualizar estadísticas
        TimeBetweenLabel.Text = $"Entre cigarros: {_smokingData.TimeBetweenCigarettes:hh\\:mm}";
        AwakeHoursLabel.Text = $"Horas despierto: {_smokingData.AwakeHours:hh\\:mm}";

        // Actualizar botón
        SmokeBtn.IsEnabled = true;
        if (_smokingData.SmokedToday >= _smokingData.MaxCigarettesPerDay)
        {
            SmokeBtn.Text = "⚠️ Límite alcanzado";
            SmokeBtn.BackgroundColor = Colors.Orange;
        }
        else
        {
            SmokeBtn.Text = "🚬 Fumar Cigarro";
            SmokeBtn.BackgroundColor = Color.FromArgb("#512BD4"); // Primary color
        }
    }

    private void ShowRandomTip()
    {
        var random = new Random();
        var tip = _tips[random.Next(_tips.Count)];
        var parts = tip.Split('|');
        
        if (parts.Length == 3)
        {
            TipIcon.Text = parts[0];
            TipTitle.Text = parts[1];
            TipText.Text = parts[2];
        }
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
        try
        {
            if (_smokingData.SmokedToday >= _smokingData.MaxCigarettesPerDay)
            {
                var confirm = await DisplayAlert(
                    "Límite alcanzado", 
                    $"Ya has fumado {_smokingData.MaxCigarettesPerDay} cigarros hoy. ¿Quieres continuar?", 
                    "Sí, fumar", 
                    "No, esperar");
                
                if (!confirm) return;

                var doubleConfirm = await DisplayAlert(
                    "Confirmación final", 
                    "¿Estás seguro? Esto excederá tu límite diario.", 
                    "Sí, estoy seguro", 
                    "Cancelar");
                
                if (!doubleConfirm) return;
            }

            await _smokingDataService.AddSmokedCigaretteAsync();
            await LoadDataAsync();
            
            await DisplayAlert("Registrado", "Cigarro registrado correctamente", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error registrando cigarro: {ex.Message}", "OK");
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadDataAsync();
        ShowRandomTip();
    }
}