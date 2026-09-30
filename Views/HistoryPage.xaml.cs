using QuitSmoke.Services;
using QuitSmoke.Models;
using QuitSmoke.Helpers;

namespace QuitSmoke.Views;

public partial class HistoryPage : ContentPage
{
    private readonly ISmokingDataService _smokingDataService;
    private readonly ILocalizationService _loc;

    public HistoryPage()
    {
        InitializeComponent();
        _smokingDataService = ServiceHelper.GetService<ISmokingDataService>()!;
        _loc = ServiceHelper.GetService<ILocalizationService>();
        ApplyLocalization();
        SummaryLabel.Text = _loc.GetString("history_summary_loading");
        _ = LoadDataAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ApplyLocalization();
        await LoadDataAsync();
    }

    private void ApplyLocalization()
    {
        string L(string key) => _loc.GetString(key);
        Title = L("history_title");
        LoadingLabel.Text = L("history_loading");
        GeneralStatsLabel.Text = L("history_general_stats");
        TotalCigarettesCaption.Text = L("history_total_cigarettes");
        DaysRecordedCaption.Text = L("history_days_recorded");
        AvgPerDayCaption.Text = L("history_avg_per_day");
        ReductionCaption.Text = L("history_reduction");
        EconomicStatsLabel.Text = L("history_economic_stats");
        TotalSpentCaption.Text = L("history_total_spent");
        TotalSavedCaption.Text = L("history_total_saved");
        AvgSpentCaption.Text = L("history_avg_per_day");
        SavingsPctCaption.Text = L("history_savings_pct");
        RefreshButton.Text = L("history_refresh");
    }

    private async Task LoadDataAsync()
    {
        try
        {
            LoadingView.IsVisible = true;
            ContentView.IsVisible = false;
            LoadingIndicator.IsRunning = true;

            // Pequeña demora para mostrar el spinner
            await Task.Delay(100);

            var history = await _smokingDataService.GetHistoryAsync(30);
            var data = await _smokingDataService.GetDataAsync();

            CalculateStatistics(history, data);
        }
        catch (Exception ex)
        {
            await SocShared.ModernDialog.AlertAsync(this,_loc.GetString("error"), $"{_loc.GetString("history_error_loading")}: {ex.Message}", _loc.GetString("ok"));
        }
        finally
        {
            LoadingView.IsVisible = false;
            ContentView.IsVisible = true;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void CalculateStatistics(List<DailySmokingRecord> history, SmokingData data)
    {
        var stats = HistoryStatistics.Calculate(history, data);

        // Estadísticas generales
        TotalCigarettesLabel.Text = stats.TotalCigarettes.ToString();
        TotalDaysLabel.Text = stats.DaysWithCigarettes.ToString();
        AveragePerDayLabel.Text = stats.AveragePerDay.ToString("F1");
        ReductionLabel.Text = $"{Math.Max(0, stats.ReductionPercentage):F1}%";

        SummaryLabel.Text = string.Format(
            _loc.GetString("history_summary"),
            stats.ReductionPercentage.ToString("F1"),
            stats.TotalCigarettes,
            stats.TotalPlanned,
            stats.DaysWithCigarettes);

        // Estadísticas económicas
        var currency = Currency.SymbolFor(data.Currency);
        TotalSpentLabel.Text = $"{currency}{stats.TotalSpent:F2}";
        TotalSavedLabel.Text = $"{currency}{stats.TotalSaved:F2}";
        AverageSpentLabel.Text = $"{currency}{stats.AverageSpentPerDay:F2}";
        SavingsPercentageLabel.Text = $"{stats.SavingsPercentage:F1}%";
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadDataAsync();
    }
}
