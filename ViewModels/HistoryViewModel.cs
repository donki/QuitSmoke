using QuitSmoke.Models;
using QuitSmoke.Services;

namespace QuitSmoke.ViewModels;

/// <summary>Historico de los ultimos 30 dias: cigarros, reduccion, gasto y ahorro.</summary>
public sealed class HistoryViewModel : ViewModelBase
{
    private readonly ISmokingDataService _data;
    private readonly Func<int, Task> _delay;

    public HistoryViewModel(ISmokingDataService data, ILocalizationService loc, Func<int, Task>? delay = null) : base(loc)
    {
        _data = data;
        _delay = delay ?? (ms => Task.Delay(ms));
        Summary = L("history_summary_loading");
    }

    // Rotulos
    public string Title => L("history_title");
    public string LoadingText => L("history_loading");
    public string GeneralStats => L("history_general_stats");
    public string TotalCigarettesCaption => L("history_total_cigarettes");
    public string DaysRecordedCaption => L("history_days_recorded");
    public string AvgPerDayCaption => L("history_avg_per_day");
    public string ReductionCaption => L("history_reduction");
    public string EconomicStats => L("history_economic_stats");
    public string TotalSpentCaption => L("history_total_spent");
    public string TotalSavedCaption => L("history_total_saved");
    public string SavingsPctCaption => L("history_savings_pct");
    public string RefreshText => L("history_refresh");

    // Valores
    public bool IsLoading { get; private set; } = true;   // la pantalla nace cargando
    public bool IsLoaded => !IsLoading;
    public string Summary { get; private set; }
    public string TotalCigarettes { get; private set; } = "";
    public string TotalDays { get; private set; } = "";
    public string AveragePerDay { get; private set; } = "";
    public string Reduction { get; private set; } = "";
    public string TotalSpent { get; private set; } = "";
    public string TotalSaved { get; private set; } = "";
    public string AverageSpent { get; private set; } = "";
    public string SavingsPercentage { get; private set; } = "";

    public async Task LoadAsync(IUserDialogs dialogs)
    {
        try
        {
            IsLoading = true;
            RaiseAllChanged();

            // Pequeña demora para que se vea el indicador de carga.
            await _delay(100);

            var history = await _data.GetHistoryAsync(30);
            var data = await _data.GetDataAsync();
            Show(HistoryStatistics.Calculate(history, data), data);
        }
        catch (Exception ex)
        {
            await dialogs.AlertAsync(L("error"), $"{L("history_error_loading")}: {ex.Message}", L("ok"));
        }
        finally
        {
            IsLoading = false;
            RaiseAllChanged();
        }
    }

    private void Show(HistoryStatistics stats, SmokingData data)
    {
        TotalCigarettes = stats.TotalCigarettes.ToString();
        TotalDays = stats.DaysWithCigarettes.ToString();
        AveragePerDay = stats.AveragePerDay.ToString("F1");
        Reduction = $"{Math.Max(0, stats.ReductionPercentage):F1}%";
        Summary = string.Format(L("history_summary"), stats.ReductionPercentage.ToString("F1"),
            stats.TotalCigarettes, stats.TotalPlanned, stats.DaysWithCigarettes);

        var currency = Currency.SymbolFor(data.Currency);
        TotalSpent = $"{currency}{stats.TotalSpent:F2}";
        TotalSaved = $"{currency}{stats.TotalSaved:F2}";
        AverageSpent = $"{currency}{stats.AverageSpentPerDay:F2}";
        SavingsPercentage = $"{stats.SavingsPercentage:F1}%";
    }
}
