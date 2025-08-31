using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuitSmoke.Models;
using QuitSmoke.Services;

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
        UpdateTimeBetweenCigarettes();
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

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        await _smokingDataService.UpdateMaxCigarettesAsync(MaxCigarettes);
        await _smokingDataService.UpdateScheduleAsync(WakeUpTime, SleepTime);
        
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