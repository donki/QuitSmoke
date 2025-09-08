using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuitSmoke.Services;
using QuitSmoke.Views.Drawables;

namespace QuitSmoke.ViewModels;

public partial class HistoryPageViewModel : ObservableObject
{
    private readonly ISmokingDataService _smokingDataService;

    [ObservableProperty]
    private HistoryChartDrawable _chartDrawable = new();

    [ObservableProperty]
    private string _totalSummaryText = string.Empty;

    [ObservableProperty]
    private double _totalProgress = 0; // 0..1

    [ObservableProperty]
    private List<HistoryRow> _historyRows = new();

    [ObservableProperty]
    private int _totalCigarettes = 0;

    [ObservableProperty]
    private int _totalDays = 0;

    [ObservableProperty]
    private double _averagePerDay = 0;

    [ObservableProperty]
    private string _maxTimeBetween = string.Empty;

    [ObservableProperty]
    private string _minTimeBetween = string.Empty;

    [ObservableProperty]
    private string _averageTimeBetween = string.Empty;

    public int Days { get; } = 30;

    public HistoryPageViewModel(ISmokingDataService smokingDataService)
    {
        _smokingDataService = smokingDataService;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            var history = await _smokingDataService.GetHistoryAsync(Days);
            var data = await _smokingDataService.GetDataAsync();

            // Rango continuo de d�as para la gr�fica (rellenando con 0 si faltan)
            var fromDate = DateTime.Today.AddDays(-Days + 1).Date;
            var byDate = history.ToDictionary(h => h.Date.Date, h => h.Count);
            var counts = new List<int>();
            for (var d = fromDate; d <= DateTime.Today; d = d.AddDays(1))
            {
                counts.Add(byDate.TryGetValue(d, out var c) ? c : 0);
            }
            // Reemplazar drawable para forzar refresco del GraphicsView
            ChartDrawable = new HistoryChartDrawable { Counts = counts };

            // Calcular estadísticas detalladas primero
            TotalCigarettes = history.SelectMany(h => h.Times).Count();
            var daysWithCigarettes = history.Count(h => h.Count > 0);
            TotalDays = daysWithCigarettes;
            AveragePerDay = daysWithCigarettes > 0 ? (double)TotalCigarettes / daysWithCigarettes : 0;

            // Totales del periodo vs planificado (solo días con registros)
            var totalSmoked = TotalCigarettes;
            var totalPlanned = data.MaxCigarettesPerDay * TotalDays; // Solo días con registros
            
            // Porcentaje de reducción: (1 - (fumados/previstos)) * 100
            // 0% = fumó todo lo previsto, 100% = no fumó nada
            if (totalPlanned > 0)
            {
                var reductionPercentage = (1.0 - ((double)totalSmoked / totalPlanned)) * 100;
                TotalProgress = Math.Max(0, reductionPercentage / 100.0); // Convertir a 0-1 para ProgressBar
                TotalSummaryText = $"Reducción lograda: {reductionPercentage:F1}% ({totalSmoked}/{totalPlanned} cigarros en {TotalDays} días)";
            }
            else
            {
                TotalProgress = 0;
                TotalSummaryText = "Sin datos suficientes para calcular reducción";
            }

            // Tabla: fechas/horas y diferencia con el siguiente
            var allTimes = history
                .OrderBy(h => h.Date)
                .SelectMany(h => h.Times.OrderBy(t => t))
                .ToList();

            // Las estadísticas ya se calcularon arriba

            // Calcular tiempos entre cigarros
            var timeBetweenList = new List<TimeSpan>();
            for (int i = 0; i < allTimes.Count - 1; i++)
            {
                var delta = allTimes[i + 1] - allTimes[i];
                timeBetweenList.Add(delta);
            }

            if (timeBetweenList.Any())
            {
                var maxTime = timeBetweenList.Max();
                var minTime = timeBetweenList.Min();
                var avgTime = TimeSpan.FromTicks((long)timeBetweenList.Average(t => t.Ticks));

                MaxTimeBetween = FormatDelta(maxTime);
                MinTimeBetween = FormatDelta(minTime);
                AverageTimeBetween = FormatDelta(avgTime);
            }
            else
            {
                MaxTimeBetween = "--";
                MinTimeBetween = "--";
                AverageTimeBetween = "--";
            }

            // Crear filas para la tabla
            var rows = new List<HistoryRow>();
            for (int i = 0; i < allTimes.Count; i++)
            {
                var current = allTimes[i];
                string deltaText;
                if (i < allTimes.Count - 1)
                {
                    var delta = allTimes[i + 1] - current;
                    deltaText = FormatDelta(delta);
                }
                else
                {
                    deltaText = "--";
                }

                rows.Add(new HistoryRow
                {
                    DateTime = current,
                    DateTimeText = current.ToString("dd/MM/yyyy HH:mm"),
                    DeltaText = deltaText
                });
            }
            HistoryRows = rows.OrderByDescending(r => r.DateTime).ToList();
            
            // Para debug
            System.Diagnostics.Debug.WriteLine($"Historia cargada: {history.Count} d�as, {allTimes.Count} cigarros, {HistoryRows.Count} filas");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando historial: {ex.Message}");
            // Valores por defecto en caso de error
            TotalSummaryText = "Error cargando datos";
            HistoryRows = new List<HistoryRow>();
            ChartDrawable = new HistoryChartDrawable { Counts = new List<int>() };
        }
    }

    private static string FormatDelta(TimeSpan delta)
    {
        if (delta.TotalMinutes < 1)
            return "<1 min";

        var parts = new List<string>();
        if (delta.Days > 0) parts.Add($"{delta.Days} d");
        if (delta.Hours > 0) parts.Add($"{delta.Hours} h");
        if (delta.Minutes > 0) parts.Add($"{delta.Minutes} min");
        return string.Join(" ", parts);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }
}

public class HistoryRow
{
    public DateTime DateTime { get; set; }
    public string DateTimeText { get; set; } = string.Empty;
    public string DeltaText { get; set; } = string.Empty;
}
