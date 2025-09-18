using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuitSmoke.Models;
using QuitSmoke.Services;
using System.Collections.ObjectModel;

namespace QuitSmoke.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
    private readonly ISmokingDataService _smokingDataService;

    [ObservableProperty]
    private SmokingData _smokingData = new();

    [ObservableProperty]
    private int _maxCigarettes;

    [ObservableProperty]
    private TimeSpan _wakeUpTime;

    [ObservableProperty]
    private TimeSpan _sleepTime;

    [ObservableProperty]
    private string _timeBetweenCigarettesText = "";

    // Propiedades para precios
    [ObservableProperty]
    private string _packPrice = "5.00";

    [ObservableProperty]
    private string _cigarettesPerPack = "20";

    [ObservableProperty]
    private Currency? _selectedCurrency;

    [ObservableProperty]
    private string _pricePerCigaretteText = "";

    public ObservableCollection<Currency> AvailableCurrencies { get; } = new(Currency.GetAvailableCurrencies());

    public SettingsPageViewModel(ISmokingDataService smokingDataService)
    {
        _smokingDataService = smokingDataService;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        SmokingData = await _smokingDataService.GetDataAsync();
        MaxCigarettes = SmokingData.MaxCigarettesPerDay;
        WakeUpTime = SmokingData.WakeUpTime;
        SleepTime = SmokingData.SleepTime;
        
        // Cargar configuración de precios
        PackPrice = SmokingData.PackPrice.ToString("F2");
        CigarettesPerPack = SmokingData.CigarettesPerPack.ToString();
        SelectedCurrency = AvailableCurrencies.FirstOrDefault(c => c.Code == SmokingData.Currency) 
                          ?? AvailableCurrencies.First();
        
        UpdateTimeBetweenCigarettes();
        UpdatePricePerCigarette();
    }

    partial void OnMaxCigarettesChanged(int value)
    {
        UpdateTimeBetweenCigarettes();
    }

    partial void OnWakeUpTimeChanged(TimeSpan value)
    {
        UpdateTimeBetweenCigarettes();
    }

    partial void OnSleepTimeChanged(TimeSpan value)
    {
        UpdateTimeBetweenCigarettes();
    }

    partial void OnPackPriceChanged(string value)
    {
        UpdatePricePerCigarette();
    }

    partial void OnCigarettesPerPackChanged(string value)
    {
        UpdatePricePerCigarette();
    }

    partial void OnSelectedCurrencyChanged(Currency? value)
    {
        UpdatePricePerCigarette();
    }

    private void UpdateTimeBetweenCigarettes()
    {
        var awakeHours = SleepTime > WakeUpTime 
            ? SleepTime - WakeUpTime 
            : TimeSpan.FromHours(24) - (WakeUpTime - SleepTime);

        var timeBetween = MaxCigarettes > 0 
            ? TimeSpan.FromMinutes(awakeHours.TotalMinutes / MaxCigarettes)
            : TimeSpan.Zero;

        TimeBetweenCigarettesText = $"Tiempo entre cigarros: {timeBetween:hh\\:mm}";
    }

    private void UpdatePricePerCigarette()
    {
        if (decimal.TryParse(PackPrice, out var packPrice) && 
            int.TryParse(CigarettesPerPack, out var cigarettesPerPack) && 
            cigarettesPerPack > 0 && 
            SelectedCurrency != null)
        {
            var pricePerCigarette = packPrice / cigarettesPerPack;
            PricePerCigaretteText = $"Precio por cigarro: {SelectedCurrency.Symbol}{pricePerCigarette:F3}";
        }
        else
        {
            PricePerCigaretteText = "Precio por cigarro: --";
        }
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await _smokingDataService.UpdateMaxCigarettesAsync(MaxCigarettes);
        await _smokingDataService.UpdateScheduleAsync(WakeUpTime, SleepTime);
        
        // Guardar configuración de precios
        if (decimal.TryParse(PackPrice, out var packPrice) && 
            int.TryParse(CigarettesPerPack, out var cigarettesPerPack) && 
            SelectedCurrency != null)
        {
            await _smokingDataService.UpdatePriceConfigurationAsync(packPrice, cigarettesPerPack, SelectedCurrency.Code);
        }
        
        await Shell.Current.DisplayAlert("Configuración", "Configuración guardada correctamente", "OK");
    }

    [RelayCommand]
    private async Task ReduceMaxCigarettesAsync()
    {
        if (MaxCigarettes > 1)
        {
            MaxCigarettes--;
            await SaveSettingsAsync();
        }
    }
}