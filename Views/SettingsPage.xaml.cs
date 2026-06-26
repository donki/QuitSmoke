using QuitSmoke.Helpers;
using QuitSmoke.Services;
using QuitSmoke.ViewModels;

namespace QuitSmoke.Views;

public partial class SettingsPage : ContentPage
{
    private readonly IPowerSettingsService? _powerService;
    private readonly SettingsPageViewModel _viewModel;

    public SettingsPage(SettingsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
#if ANDROID
        _powerService = ServiceHelper.Services.GetService(typeof(IPowerSettingsService)) as IPowerSettingsService;
#endif
    }

    private async void OnDecreaseMaxCigarettes(object sender, EventArgs e)
    {
        if (BindingContext is ViewModels.SettingsPageViewModel vm)
        {
            await vm.ReduceMaxCigarettesCommand.ExecuteAsync(null);
        }
    }

    private async void OnIncreaseMaxCigarettes(object sender, EventArgs e)
    {
        if (BindingContext is ViewModels.SettingsPageViewModel vm)
        {
            vm.MaxCigarettes++;
            MaxCigarettesEntry.Text = vm.MaxCigarettes.ToString();
            await vm.SaveSettingsCommand.ExecuteAsync(null);
        }
    }

    private async void OnSaveSettingsClicked(object sender, EventArgs e)
    {
        // Volcar los valores de los controles al ViewModel antes de guardar.
        if (int.TryParse(MaxCigarettesEntry.Text, out var maxCigarettes))
        {
            _viewModel.MaxCigarettes = maxCigarettes;
        }
        _viewModel.WakeUpTime = WakeUpTimePicker.Time;
        _viewModel.SleepTime = SleepTimePicker.Time;
        _viewModel.PackPrice = PackPriceEntry.Text;
        _viewModel.CigarettesPerPack = CigarettesPerPackEntry.Text;
        if (CurrencyPicker.SelectedItem is Models.Currency currency)
        {
            _viewModel.SelectedCurrency = currency;
        }

        await _viewModel.SaveSettingsCommand.ExecuteAsync(null);
    }

    private async void OnReduceMaxClicked(object sender, EventArgs e)
    {
        await _viewModel.ReduceMaxCigarettesCommand.ExecuteAsync(null);
        MaxCigarettesEntry.Text = _viewModel.MaxCigarettes.ToString();
    }

#if ANDROID
    private async void OnCheckPermissionsClicked(object sender, EventArgs e)
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
        await DisplayAlert("Permisos", "Se abrieron los ajustes del sistema. Configura bater�a, autoinicio y segundo plano.", "OK");
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

    private async void OnRequestIgnoreBatteryOptimization(object sender, EventArgs e)
    {
        if (_powerService == null) return;
        var ok = await _powerService.RequestIgnoreBatteryOptimizationsAsync();
        await DisplayAlert("Ajustes de energ�a", ok ? "Solicitud enviada. Comprueba los ajustes del sistema." : "No se pudo abrir la solicitud.", "OK");
    }

    private void OnOpenBatteryOptimizationSettings(object sender, EventArgs e)
    {
        _powerService?.OpenBatteryOptimizationSettings();
    }

    private void OnOpenAutostartSettings(object sender, EventArgs e)
    {
        _powerService?.OpenAutostartSettings();
    }

    private void OnOpenBackgroundSettings(object sender, EventArgs e)
    {
        _powerService?.OpenBackgroundSettings();
    }
#else
    private async void OnCheckPermissionsClicked(object sender, EventArgs e) => await DisplayAlert("Permisos", "Solo disponible en Android.", "OK");
    private async void OnConfigureAllPermissionsClicked(object sender, EventArgs e) => await DisplayAlert("Permisos", "Solo disponible en Android.", "OK");
    private async void OnBatteryOptimizationClicked(object sender, EventArgs e) => await DisplayAlert("Permisos", "Solo disponible en Android.", "OK");
    private async void OnAutostartClicked(object sender, EventArgs e) => await DisplayAlert("Permisos", "Solo disponible en Android.", "OK");
#endif
}