using QuitSmoke.Helpers;
using QuitSmoke.Models;
using QuitSmoke.Services;

namespace QuitSmoke.Views;

public partial class SettingsPage : ContentPage
{
    private readonly ISmokingDataService _smokingDataService;
    private readonly IPowerSettingsService? _powerService;

    public SettingsPage()
    {
        InitializeComponent();
        _smokingDataService = ServiceHelper.GetService<ISmokingDataService>()!;
#if ANDROID
        _powerService = ServiceHelper.GetService<IPowerSettingsService>();
#endif
        CurrencyPicker.ItemDisplayBinding = new Binding(nameof(Currency.Name));
        CurrencyPicker.ItemsSource = Currency.GetAvailableCurrencies();

        // Actualizaciones en vivo de las etiquetas calculadas.
        MaxCigarettesEntry.TextChanged += (_, _) => UpdateTimeBetween();
        WakeUpTimePicker.PropertyChanged += (_, e) => { if (e.PropertyName == TimePicker.TimeProperty.PropertyName) UpdateTimeBetween(); };
        SleepTimePicker.PropertyChanged += (_, e) => { if (e.PropertyName == TimePicker.TimeProperty.PropertyName) UpdateTimeBetween(); };
        PackPriceEntry.TextChanged += (_, _) => UpdatePricePerCigarette();
        CigarettesPerPackEntry.TextChanged += (_, _) => UpdatePricePerCigarette();
        CurrencyPicker.SelectedIndexChanged += (_, _) => UpdatePricePerCigarette();

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
            var data = await _smokingDataService.GetDataAsync();

            MaxCigarettesEntry.Text = data.MaxCigarettesPerDay.ToString();
            WakeUpTimePicker.Time = data.WakeUpTime;
            SleepTimePicker.Time = data.SleepTime;
            PackPriceEntry.Text = data.PackPrice.ToString("F2");
            CigarettesPerPackEntry.Text = data.CigarettesPerPack.ToString();
            CurrencyPicker.SelectedItem = Currency.GetAvailableCurrencies()
                .FirstOrDefault(c => c.Code == data.Currency)
                ?? Currency.GetAvailableCurrencies().First();

            UpdateTimeBetween();
            UpdatePricePerCigarette();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error cargando configuración: {ex.Message}", "OK");
        }
    }

    private void UpdateTimeBetween()
    {
        var wakeUp = WakeUpTimePicker.Time;
        var sleep = SleepTimePicker.Time;
        var awake = sleep > wakeUp ? sleep - wakeUp : TimeSpan.FromHours(24) - (wakeUp - sleep);

        if (int.TryParse(MaxCigarettesEntry.Text, out var max) && max > 0)
        {
            var between = TimeSpan.FromMinutes(awake.TotalMinutes / max);
            TimeBetweenLabel.Text = $"Tiempo entre cigarros: {between:hh\\:mm}";
        }
        else
        {
            TimeBetweenLabel.Text = "Tiempo entre cigarros: --";
        }
    }

    private void UpdatePricePerCigarette()
    {
        if (decimal.TryParse(PackPriceEntry.Text, out var packPrice) &&
            int.TryParse(CigarettesPerPackEntry.Text, out var cigarettesPerPack) &&
            cigarettesPerPack > 0 &&
            CurrencyPicker.SelectedItem is Currency currency)
        {
            var pricePerCigarette = packPrice / cigarettesPerPack;
            PricePerCigaretteLabel.Text = $"Precio por cigarrillo: {currency.Symbol}{pricePerCigarette:F3}";
        }
        else
        {
            PricePerCigaretteLabel.Text = "Precio por cigarrillo: --";
        }
    }

    private async Task SaveSettingsAsync()
    {
        if (int.TryParse(MaxCigarettesEntry.Text, out var maxCigarettes) && maxCigarettes > 0)
        {
            await _smokingDataService.UpdateMaxCigarettesAsync(maxCigarettes);
        }

        await _smokingDataService.UpdateScheduleAsync(WakeUpTimePicker.Time, SleepTimePicker.Time);

        if (decimal.TryParse(PackPriceEntry.Text, out var packPrice) &&
            int.TryParse(CigarettesPerPackEntry.Text, out var cigarettesPerPack) &&
            CurrencyPicker.SelectedItem is Currency currency)
        {
            await _smokingDataService.UpdatePriceConfigurationAsync(packPrice, cigarettesPerPack, currency.Code);
        }
    }

    private async void OnSaveSettingsClicked(object sender, EventArgs e)
    {
        await SaveSettingsAsync();
        await DisplayAlert("Configuración", "Configuración guardada correctamente", "OK");
    }

    private async void OnReduceMaxClicked(object sender, EventArgs e)
    {
        if (int.TryParse(MaxCigarettesEntry.Text, out var max) && max > 1)
        {
            MaxCigarettesEntry.Text = (max - 1).ToString();
            UpdateTimeBetween();
            await SaveSettingsAsync();
        }
    }

    private async void OnDecreaseMaxCigarettes(object sender, EventArgs e)
    {
        if (int.TryParse(MaxCigarettesEntry.Text, out var max) && max > 1)
        {
            MaxCigarettesEntry.Text = (max - 1).ToString();
            UpdateTimeBetween();
            await SaveSettingsAsync();
        }
    }

    private async void OnIncreaseMaxCigarettes(object sender, EventArgs e)
    {
        var max = int.TryParse(MaxCigarettesEntry.Text, out var current) ? current : 0;
        MaxCigarettesEntry.Text = (max + 1).ToString();
        UpdateTimeBetween();
        await SaveSettingsAsync();
    }

#if ANDROID
    private void OnCheckPermissionsClicked(object sender, EventArgs e)
    {
        if (_powerService == null) return;
        var ignoring = _powerService.IsIgnoringBatteryOptimizations();
        BatteryOptStatus.Text = ignoring ? "Excluida del ahorro" : "Optimizada (recomendado excluir)";
        AutostartStatus.Text = "Revisar en ajustes del sistema";
    }

    private async void OnConfigureAllPermissionsClicked(object sender, EventArgs e)
    {
        if (_powerService == null) return;
        var ok = await _powerService.RequestIgnoreBatteryOptimizationsAsync();
        if (!ok)
        {
            _powerService.OpenBatteryOptimizationSettings();
        }
        _powerService.OpenAutostartSettings();
        _powerService.OpenBackgroundSettings();
        await DisplayAlert("Permisos", "Se abrieron los ajustes del sistema. Configura batería, autoinicio y segundo plano.", "OK");
    }

    private async void OnBatteryOptimizationClicked(object sender, EventArgs e)
    {
        if (_powerService == null) return;
        var ok = await _powerService.RequestIgnoreBatteryOptimizationsAsync();
        if (!ok)
        {
            _powerService.OpenBatteryOptimizationSettings();
        }
    }

    private void OnAutostartClicked(object sender, EventArgs e)
    {
        _powerService?.OpenAutostartSettings();
    }
#else
    private async void OnCheckPermissionsClicked(object sender, EventArgs e) => await DisplayAlert("Permisos", "Solo disponible en Android.", "OK");
    private async void OnConfigureAllPermissionsClicked(object sender, EventArgs e) => await DisplayAlert("Permisos", "Solo disponible en Android.", "OK");
    private async void OnBatteryOptimizationClicked(object sender, EventArgs e) => await DisplayAlert("Permisos", "Solo disponible en Android.", "OK");
    private async void OnAutostartClicked(object sender, EventArgs e) => await DisplayAlert("Permisos", "Solo disponible en Android.", "OK");
#endif
}
