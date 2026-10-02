using QuitSmoke.Helpers;
using QuitSmoke.Models;
using QuitSmoke.Services;
using QuitSmoke.ViewModels;

namespace QuitSmoke.Views;

/// <summary>
/// Configuracion. Rotulos enlazados a <see cref="SettingsViewModel"/>, que tambien hace las
/// cuentas y guarda; aqui solo se leen y se rellenan los campos.
/// </summary>
public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _vm;
    private readonly ILocalizationService _loc;
    private readonly ModernDialogs _dialogs;
    private string? _currencyLanguage;
    private bool _suppressSave;

    public SettingsPage()
    {
        InitializeComponent();
        _loc = ServiceHelper.GetService<ILocalizationService>();
        _dialogs = new ModernDialogs(this);
        IPowerSettingsService? power = null;
#if ANDROID
        power = ServiceHelper.GetService<IPowerSettingsService>();
#endif
        BindingContext = _vm = new SettingsViewModel(ServiceHelper.GetService<ISmokingDataService>(), _loc, power);
        CurrencyPicker.ItemDisplayBinding = new Binding(nameof(Currency.Name));

        // Etiquetas calculadas al vuelo y autoguardado al terminar de editar cada campo.
        MaxCigarettesEntry.TextChanged += (_, _) => UpdateTimeBetween();
        PackPriceEntry.TextChanged += (_, _) => UpdatePricePerCigarette();
        CigarettesPerPackEntry.TextChanged += (_, _) => UpdatePricePerCigarette();
        CurrencyPicker.SelectedIndexChanged += async (_, _) => { UpdatePricePerCigarette(); await SaveAsync(); };
        MaxCigarettesEntry.Unfocused += async (_, _) => await SaveAsync();
        PackPriceEntry.Unfocused += async (_, _) => await SaveAsync();
        CigarettesPerPackEntry.Unfocused += async (_, _) => await SaveAsync();
        WakeUpTimePicker.PropertyChanged += OnTimeChanged;
        SleepTimePicker.PropertyChanged += OnTimeChanged;

        ApplyLocalization();
        _ = LoadDataAsync();
    }

    private async void OnTimeChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != TimePicker.TimeProperty.PropertyName)
            return;
        UpdateTimeBetween();
        await SaveAsync();
    }

    // Atras vuelve a Inicio sin que el campo en edicion pierda el foco: se guarda al salir para no
    // perder lo escrito (General: no perder lo escrito).
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        try { await SaveAsync(); }
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
        _vm.Relocalize();

        // Nombres de las divisas en el idioma actual: se rehace la lista solo si cambio el idioma.
        var lang = _loc.GetCurrentLanguage();
        if (lang == _currencyLanguage)
            return;
        var selectedCode = (CurrencyPicker.SelectedItem as Currency)?.Code;
        _suppressSave = true;
        try
        {
            var currencies = _vm.Currencies();
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

    private async Task LoadDataAsync()
    {
        if (await _vm.LoadAsync(_dialogs) is not { } form)
            return;

        MaxCigarettesEntry.Text = form.MaxPerDay;
        WakeUpTimePicker.Time = form.WakeUp;
        SleepTimePicker.Time = form.Sleep;
        PackPriceEntry.Text = form.PackPrice;
        CigarettesPerPackEntry.Text = form.PerPack;
        // Se elige de la misma lista que muestra el selector (si no, el elemento no se encuentra).
        if (CurrencyPicker.ItemsSource is IList<Currency> { Count: > 0 } currencies)
            CurrencyPicker.SelectedItem = currencies.FirstOrDefault(c => c.Code == form.CurrencyCode) ?? currencies[0];

        UpdateTimeBetween();
        UpdatePricePerCigarette();
    }

    private void UpdateTimeBetween() =>
        TimeBetweenLabel.Text = _vm.TimeBetweenText(WakeUpTimePicker.Time, SleepTimePicker.Time, MaxCigarettesEntry.Text);

    private void UpdatePricePerCigarette() =>
        PricePerCigaretteLabel.Text = _vm.PricePerCigaretteText(PackPriceEntry.Text, CigarettesPerPackEntry.Text,
            CurrencyPicker.SelectedItem as Currency);

    private SettingsForm Form() => new(MaxCigarettesEntry.Text, WakeUpTimePicker.Time, SleepTimePicker.Time,
        PackPriceEntry.Text, CigarettesPerPackEntry.Text, (CurrencyPicker.SelectedItem as Currency)?.Code);

    private Task SaveAsync() => _suppressSave ? Task.CompletedTask : _vm.SaveAsync(Form());

    private async void OnSaveSettingsClicked(object sender, EventArgs e)
    {
        await SaveAsync();
        await _vm.SavedAsync(_dialogs);
    }

    private async void OnReduceMaxClicked(object sender, EventArgs e) => await SetMaxAsync(SettingsViewModel.Decrease(MaxCigarettesEntry.Text));
    private async void OnDecreaseMaxCigarettes(object sender, EventArgs e) => await SetMaxAsync(SettingsViewModel.Decrease(MaxCigarettesEntry.Text));
    private async void OnIncreaseMaxCigarettes(object sender, EventArgs e) => await SetMaxAsync(SettingsViewModel.Increase(MaxCigarettesEntry.Text));

    private async Task SetMaxAsync(string? max)
    {
        if (max is null)
            return;
        MaxCigarettesEntry.Text = max;
        UpdateTimeBetween();
        await SaveAsync();
    }

    private async void OnCheckPermissionsClicked(object sender, EventArgs e) => await _vm.CheckPermissionsAsync(_dialogs);
    private async void OnConfigureAllPermissionsClicked(object sender, EventArgs e) => await _vm.ConfigureAllAsync(_dialogs);
    private async void OnBatteryOptimizationClicked(object sender, EventArgs e) => await _vm.BatteryOptimizationAsync(_dialogs);
    private async void OnAutostartClicked(object sender, EventArgs e) => await _vm.AutostartAsync(_dialogs);
}
