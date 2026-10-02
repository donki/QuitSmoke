using QuitSmoke.Models;
using QuitSmoke.Services;

namespace QuitSmoke.ViewModels;

/// <summary>Lo que hay escrito en Configuracion (tal cual: los numeros pueden venir mal escritos).</summary>
public sealed record SettingsForm(string MaxPerDay, TimeSpan WakeUp, TimeSpan Sleep, string PackPrice,
    string PerPack, string? CurrencyCode);

/// <summary>
/// Configuracion: maximo diario, horario, precio y permisos de bateria. Los campos se guardan solos
/// al terminar de editarlos (el boton «Guardar» queda oculto).
/// </summary>
public sealed class SettingsViewModel : ViewModelBase
{
    private readonly ISmokingDataService _data;
    private readonly IPowerSettingsService? _power;

    /// <param name="power">Solo en Android; sin el, los botones de permisos lo explican.</param>
    public SettingsViewModel(ISmokingDataService data, ILocalizationService loc, IPowerSettingsService? power) : base(loc)
    {
        _data = data;
        _power = power;
        BatteryStatus = L("settings_unverified");
        AutostartStatus = L("settings_manual");
    }

    // Rotulos
    public string Title => L("settings_title");
    public string Subtitle => L("settings_subtitle");
    public string BatterySave => L("settings_battery_save");
    public string Autostart => L("settings_autostart");
    public string PermissionsTitle => L("settings_permissions_title");
    public string CheckPermissions => L("settings_check_permissions");
    public string ConfigureAll => L("settings_configure_all");
    public string Battery => L("settings_battery");
    public string PermissionsHint => L("settings_permissions_hint");
    public string MaxPerDay => L("settings_max_per_day");
    public string Schedule => L("settings_schedule");
    public string WakeTime => L("settings_wake_time");
    public string SleepTime => L("settings_sleep_time");
    public string PriceConfig => L("settings_price_config");
    public string PackPrice => L("settings_pack_price");
    public string PerPack => L("settings_per_pack");
    public string CurrencyText => L("settings_currency");
    public string Save => L("settings_save");
    public string ReduceMax => L("settings_reduce_max");

    // Estado de los permisos (sin comprobar hasta que se pide)
    public string BatteryStatus { get; private set; }
    public string AutostartStatus { get; private set; }
    private bool _permissionsChecked;

    /// <summary>Al cambiar de idioma: los rotulos y, si no se han comprobado, los estados.</summary>
    public void Relocalize()
    {
        if (!_permissionsChecked)
        {
            BatteryStatus = L("settings_unverified");
            AutostartStatus = L("settings_manual");
        }
        RaiseAllChanged();
    }

    /// <summary>Las divisas con su nombre en el idioma activo.</summary>
    public List<Currency> Currencies() => Currency.GetAvailableCurrencies(Loc);

    /// <summary>Lo guardado, listo para los campos; null si no se pudo leer (y ya se aviso).</summary>
    public async Task<SettingsForm?> LoadAsync(IUserDialogs dialogs)
    {
        try
        {
            var d = await _data.GetDataAsync();
            return new SettingsForm(d.MaxCigarettesPerDay.ToString(), d.WakeUpTime, d.SleepTime,
                d.PackPrice.ToString("F2"), d.CigarettesPerPack.ToString(), d.Currency);
        }
        catch (Exception ex)
        {
            await dialogs.AlertAsync(L("error"), $"{L("settings_error_loading")}: {ex.Message}", L("ok"));
            return null;
        }
    }

    /// <summary>Horas despierto (pasando la medianoche si hace falta).</summary>
    public static TimeSpan AwakeTime(TimeSpan wakeUp, TimeSpan sleep) =>
        sleep > wakeUp ? sleep - wakeUp : TimeSpan.FromHours(24) - (wakeUp - sleep);

    /// <summary>«Tiempo entre cigarros: 00:48», o «--» si el maximo no es un numero positivo.</summary>
    public string TimeBetweenText(TimeSpan wakeUp, TimeSpan sleep, string? maxPerDay) =>
        int.TryParse(maxPerDay, out var max) && max > 0
            ? $"{L("settings_time_between")}: {TimeSpan.FromMinutes(AwakeTime(wakeUp, sleep).TotalMinutes / max):hh\\:mm}"
            : $"{L("settings_time_between")}: --";

    /// <summary>«Precio por cigarro: €0.250», o «--» si falta algo.</summary>
    public string PricePerCigaretteText(string? packPrice, string? perPack, Currency? currency) =>
        decimal.TryParse(packPrice, out var price) && int.TryParse(perPack, out var count) && count > 0 && currency is not null
            ? $"{L("settings_price_per_cig")}: {currency.Symbol}{price / count:F3}"
            : $"{L("settings_price_per_cig")}: --";

    /// <summary>Guarda lo que se pueda: un campo mal escrito no impide guardar los demas.</summary>
    public async Task SaveAsync(SettingsForm form)
    {
        if (int.TryParse(form.MaxPerDay, out var max) && max > 0)
            await _data.UpdateMaxCigarettesAsync(max);

        await _data.UpdateScheduleAsync(form.WakeUp, form.Sleep);

        if (decimal.TryParse(form.PackPrice, out var price) && int.TryParse(form.PerPack, out var perPack)
            && form.CurrencyCode is { } currency)
            await _data.UpdatePriceConfigurationAsync(price, perPack, currency);
    }

    /// <summary>Un cigarro menos al dia (nunca por debajo de uno); null si no hay nada que bajar.</summary>
    public static string? Decrease(string? maxPerDay) =>
        int.TryParse(maxPerDay, out var max) && max > 1 ? (max - 1).ToString() : null;

    /// <summary>Un cigarro mas al dia; si no hay un numero, empieza en uno.</summary>
    public static string Increase(string? maxPerDay) =>
        ((int.TryParse(maxPerDay, out var max) ? max : 0) + 1).ToString();

    // ---- Permisos de bateria (Android) ----

    private Task AndroidOnly(IUserDialogs dialogs) =>
        dialogs.AlertAsync(L("settings_permissions_result_title"), L("settings_android_only"), L("ok"));

    public async Task CheckPermissionsAsync(IUserDialogs dialogs)
    {
        if (_power is null)
        {
            await AndroidOnly(dialogs);
            return;
        }
        _permissionsChecked = true;
        BatteryStatus = _power.IsIgnoringBatteryOptimizations() ? L("settings_battery_excluded") : L("settings_battery_optimized");
        AutostartStatus = L("settings_check_system");
        RaiseAllChanged();
    }

    public async Task ConfigureAllAsync(IUserDialogs dialogs)
    {
        if (_power is null)
        {
            await AndroidOnly(dialogs);
            return;
        }
        if (!await _power.RequestIgnoreBatteryOptimizationsAsync())
            _power.OpenBatteryOptimizationSettings();
        _power.OpenAutostartSettings();
        _power.OpenBackgroundSettings();
        await dialogs.AlertAsync(L("settings_permissions_result_title"), L("settings_permissions_result_message"), L("ok"));
    }

    public async Task BatteryOptimizationAsync(IUserDialogs dialogs)
    {
        if (_power is null)
        {
            await AndroidOnly(dialogs);
            return;
        }
        if (!await _power.RequestIgnoreBatteryOptimizationsAsync())
            _power.OpenBatteryOptimizationSettings();
    }

    public async Task AutostartAsync(IUserDialogs dialogs)
    {
        if (_power is null)
        {
            await AndroidOnly(dialogs);
            return;
        }
        _power.OpenAutostartSettings();
    }

    public Task SavedAsync(IUserDialogs dialogs) =>
        dialogs.AlertAsync(L("settings_saved_title"), L("settings_saved_message"), L("ok"));
}
