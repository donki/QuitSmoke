using QuitSmoke.Helpers;
using QuitSmoke.Models;
using QuitSmoke.Services;

namespace QuitSmoke.Views;

public partial class SettingsPage : ContentPage
{
    private readonly ISmokingDataService _smokingDataService;
    private readonly ILocalizationService _loc;
    private readonly IPowerSettingsService? _powerService;
    private string? _currencyLanguage;
    private bool _suppressSave;
    private bool _permissionsChecked;

    public SettingsPage()
    {
        InitializeComponent();
        _smokingDataService = ServiceHelper.GetService<ISmokingDataService>()!;
        _loc = ServiceHelper.GetService<ILocalizationService>();
#if ANDROID
        _powerService = ServiceHelper.GetService<IPowerSettingsService>();
#endif
        CurrencyPicker.ItemDisplayBinding = new Binding(nameof(Currency.Name));

        // Actualizaciones en vivo de las etiquetas calculadas.
        MaxCigarettesEntry.TextChanged += (_, _) => UpdateTimeBetween();
        WakeUpTimePicker.PropertyChanged += (_, e) => { if (e.PropertyName == TimePicker.TimeProperty.PropertyName) UpdateTimeBetween(); };
        SleepTimePicker.PropertyChanged += (_, e) => { if (e.PropertyName == TimePicker.TimeProperty.PropertyName) UpdateTimeBetween(); };
        PackPriceEntry.TextChanged += (_, _) => UpdatePricePerCigarette();
        CigarettesPerPackEntry.TextChanged += (_, _) => UpdatePricePerCigarette();
        CurrencyPicker.SelectedIndexChanged += (_, _) => UpdatePricePerCigarette();

        // Autoguardado: los ajustes se guardan solos al terminar de editar cada campo o cambiar
        // un selector, sin necesidad del botón "Guardar" (que queda oculto).
        MaxCigarettesEntry.Unfocused += async (_, _) => await SaveSettingsAsync();
        PackPriceEntry.Unfocused += async (_, _) => await SaveSettingsAsync();
        CigarettesPerPackEntry.Unfocused += async (_, _) => await SaveSettingsAsync();
        WakeUpTimePicker.PropertyChanged += async (_, e) => { if (e.PropertyName == TimePicker.TimeProperty.PropertyName) await SaveSettingsAsync(); };
        SleepTimePicker.PropertyChanged += async (_, e) => { if (e.PropertyName == TimePicker.TimeProperty.PropertyName) await SaveSettingsAsync(); };
        CurrencyPicker.SelectedIndexChanged += async (_, _) => await SaveSettingsAsync();

        ApplyLocalization();
        _ = LoadDataAsync();
    }

    // Atrás vuelve a Inicio sin que el campo en edición pierda el foco: se guarda al salir para no
    // perder lo escrito (General: no perder lo escrito).
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        try { await SaveSettingsAsync(); }
        catch (Exception ex) { SocShared.CrashGuard.Log(ex, "SettingsPage.OnDisappearing"); }
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
        Title = L("settings_title");
        HeaderTitleLabel.Text = L("settings_title");
        HeaderSubtitleLabel.Text = L("settings_subtitle");
        BatterySaveLabel.Text = L("settings_battery_save");
        AutostartLabel.Text = L("settings_autostart");
        PermissionsTitleLabel.Text = L("settings_permissions_title");
        CheckPermissionsButton.Text = L("settings_check_permissions");
        ConfigureAllButton.Text = L("settings_configure_all");
        BatteryButton.Text = L("settings_battery");
        AutostartButton.Text = L("settings_autostart");
        PermissionsHintLabel.Text = L("settings_permissions_hint");
        MaxPerDayLabel.Text = L("settings_max_per_day");
        ScheduleLabel.Text = L("settings_schedule");
        WakeTimeLabel.Text = L("settings_wake_time");
        SleepTimeLabel.Text = L("settings_sleep_time");
        PriceConfigLabel.Text = L("settings_price_config");
        PackPriceLabel.Text = L("settings_pack_price");
        PerPackLabel.Text = L("settings_per_pack");
        CurrencyLabel.Text = L("settings_currency");
        SaveButton.Text = L("settings_save");
        ReduceMaxButton.Text = L("settings_reduce_max");

        if (!_permissionsChecked)
        {
            BatteryOptStatus.Text = L("settings_unverified");
            AutostartStatus.Text = L("settings_manual");
        }

        // Nombres de las divisas en el idioma actual: se rehace la lista solo si cambió el idioma.
        var lang = _loc.GetCurrentLanguage();
        if (lang != _currencyLanguage)
        {
            var selectedCode = (CurrencyPicker.SelectedItem as Currency)?.Code;
            _suppressSave = true;
            try
            {
                var currencies = Currency.GetAvailableCurrencies(_loc);
                CurrencyPicker.ItemsSource = currencies;
                if (selectedCode != null)
                    CurrencyPicker.SelectedItem = currencies.FirstOrDefault(c => c.Code == selectedCode);
            }
            finally
            {
                _suppressSave = false;
            }
            _currencyLanguage = lang;
        }
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
            // Se elige de la misma lista que muestra el selector (si no, el elemento no se encuentra).
            if (CurrencyPicker.ItemsSource is IList<Currency> currencies && currencies.Count > 0)
                CurrencyPicker.SelectedItem = currencies.FirstOrDefault(c => c.Code == data.Currency)
                    ?? currencies[0];

            UpdateTimeBetween();
            UpdatePricePerCigarette();
        }
        catch (Exception ex)
        {
            await SocShared.ModernDialog.AlertAsync(this,_loc.GetString("error"), $"{_loc.GetString("settings_error_loading")}: {ex.Message}", _loc.GetString("ok"));
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
            TimeBetweenLabel.Text = $"{_loc.GetString("settings_time_between")}: {between:hh\\:mm}";
        }
        else
        {
            TimeBetweenLabel.Text = $"{_loc.GetString("settings_time_between")}: --";
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
            PricePerCigaretteLabel.Text = $"{_loc.GetString("settings_price_per_cig")}: {currency.Symbol}{pricePerCigarette:F3}";
        }
        else
        {
            PricePerCigaretteLabel.Text = $"{_loc.GetString("settings_price_per_cig")}: --";
        }
    }

    private async Task SaveSettingsAsync()
    {
        if (_suppressSave)
            return;

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
        await SocShared.ModernDialog.AlertAsync(this,_loc.GetString("settings_saved_title"), _loc.GetString("settings_saved_message"), _loc.GetString("ok"));
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
        _permissionsChecked = true;
        var ignoring = _powerService.IsIgnoringBatteryOptimizations();
        BatteryOptStatus.Text = ignoring ? _loc.GetString("settings_battery_excluded") : _loc.GetString("settings_battery_optimized");
        AutostartStatus.Text = _loc.GetString("settings_check_system");
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
        await SocShared.ModernDialog.AlertAsync(this,_loc.GetString("settings_permissions_result_title"), _loc.GetString("settings_permissions_result_message"), _loc.GetString("ok"));
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
    private async void OnCheckPermissionsClicked(object sender, EventArgs e) => await SocShared.ModernDialog.AlertAsync(this,_loc.GetString("settings_permissions_result_title"), _loc.GetString("settings_android_only"), _loc.GetString("ok"));
    private async void OnConfigureAllPermissionsClicked(object sender, EventArgs e) => await SocShared.ModernDialog.AlertAsync(this,_loc.GetString("settings_permissions_result_title"), _loc.GetString("settings_android_only"), _loc.GetString("ok"));
    private async void OnBatteryOptimizationClicked(object sender, EventArgs e) => await SocShared.ModernDialog.AlertAsync(this,_loc.GetString("settings_permissions_result_title"), _loc.GetString("settings_android_only"), _loc.GetString("ok"));
    private async void OnAutostartClicked(object sender, EventArgs e) => await SocShared.ModernDialog.AlertAsync(this,_loc.GetString("settings_permissions_result_title"), _loc.GetString("settings_android_only"), _loc.GetString("ok"));
#endif
}
