using QuitSmoke.Services;
using QuitSmoke.Models;
using QuitSmoke.Helpers;

namespace QuitSmoke.Views;

public partial class HistoryPage : ContentPage
{
    private readonly ISmokingDataService _smokingDataService;

    public HistoryPage()
    {
        InitializeComponent();
        _smokingDataService = ServiceHelper.GetService<ISmokingDataService>()!;
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
            await DisplayAlert("Error", $"Error cargando historial: {ex.Message}", "OK");
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
        // Estadísticas generales
        var totalCigarettes = history.SelectMany(h => h.Times).Count();
        var daysWithCigarettes = history.Count(h => h.Count > 0);
        var averagePerDay = daysWithCigarettes > 0 ? (double)totalCigarettes / daysWithCigarettes : 0;

        TotalCigarettesLabel.Text = totalCigarettes.ToString();
        TotalDaysLabel.Text = daysWithCigarettes.ToString();
        AveragePerDayLabel.Text = averagePerDay.ToString("F1");

        // Cálculo de reducción
        var totalPlanned = data.MaxCigarettesPerDay * daysWithCigarettes;
        var reductionPercentage = totalPlanned > 0 ? (1.0 - ((double)totalCigarettes / totalPlanned)) * 100 : 0;
        ReductionLabel.Text = $"{Math.Max(0, reductionPercentage):F1}%";

        SummaryLabel.Text = $"Reducción lograda: {reductionPercentage:F1}% ({totalCigarettes}/{totalPlanned} cigarros en {daysWithCigarettes} días)";

        // Estadísticas económicas
        CalculateEconomicStats(history, data);
    }

    private void CalculateEconomicStats(List<DailySmokingRecord> history, SmokingData data)
    {
        var currency = GetCurrencySymbol(data.Currency);
        
        // Total gastado
        var totalSpent = 0m;
        var totalCigarettesSmoked = 0;
        
        foreach (var day in history)
        {
            if (day.SmokedCigarettes?.Any() == true)
            {
                totalSpent += day.SmokedCigarettes.Sum(c => c.Price);
                totalCigarettesSmoked += day.SmokedCigarettes.Count;
            }
            else if (day.Count > 0)
            {
                totalSpent += day.Count * data.PricePerCigarette;
                totalCigarettesSmoked += day.Count;
            }
        }
        
        TotalSpentLabel.Text = $"{currency}{totalSpent:F2}";
        
        // Total ahorrado: (Teórico fumado - Real fumado) * precio cigarrillo
        var daysWithRecords = history.Count(h => h.Count > 0);
        var theoreticalCigarettes = daysWithRecords * data.MaxCigarettesPerDay;
        var cigarettesSaved = Math.Max(0, theoreticalCigarettes - totalCigarettesSmoked);
        var totalSaved = cigarettesSaved * data.PricePerCigarette;
        
        TotalSavedLabel.Text = $"{currency}{totalSaved:F2}";
        
        // Promedio gasto por día
        var averageCigarettesPerDay = daysWithRecords > 0 ? (double)totalCigarettesSmoked / daysWithRecords : 0;
        var averageSpentPerDay = averageCigarettesPerDay * (double)data.PricePerCigarette;
        AverageSpentLabel.Text = $"{currency}{averageSpentPerDay:F2}";
        
        // Porcentaje de ahorro
        var savingsPercentage = totalSpent > 0 ? ((double)totalSaved / (double)totalSpent) * 100 : 0;
        SavingsPercentageLabel.Text = $"{savingsPercentage:F1}%";
    }

    private string GetCurrencySymbol(string currencyCode)
    {
        var currency = Currency.GetAvailableCurrencies().FirstOrDefault(c => c.Code == currencyCode);
        return currency?.Symbol ?? currencyCode;
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadDataAsync();
    }
}
