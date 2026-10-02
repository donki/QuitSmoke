using System.ComponentModel;
using QuitSmoke.Models;
using QuitSmoke.Services;
using QuitSmoke.ViewModels;

namespace QuitSmoke.Tests;

/// <summary>Las pantallas sin pantalla: Inicio, Historico, Configuracion, Acerca de, version y notificacion.</summary>
public class ViewModelTests : IDisposable
{
    private readonly LocalizationService _loc = new();
    private readonly FakeDialogs _dialogs = new();
    private readonly FakeNotifications _notifications = new();

    public ViewModelTests()
    {
        FileSystem.AppDataDirectory = Path.Combine(Path.GetTempPath(), $"quitsmoke-vm-{Guid.NewGuid():N}");
        Directory.CreateDirectory(FileSystem.AppDataDirectory);
        Preferences.Values.Clear();
        _loc.SetLanguage("es");
    }

    public void Dispose()
    {
        Preferences.Values.Clear();
        try { Directory.Delete(FileSystem.AppDataDirectory, true); } catch (IOException) { }
    }

    // -----------------------------------------------------------------------
    // Dobles
    // -----------------------------------------------------------------------

    private sealed class FakeDialogs : IUserDialogs
    {
        public List<string> Shown { get; } = [];
        public Queue<bool> Answers { get; } = new();

        public Task AlertAsync(string title, string message, string ok)
        {
            Shown.Add($"{title}|{message}");
            return Task.CompletedTask;
        }

        public Task<bool> ConfirmAsync(string title, string message, string accept, string cancel)
        {
            Shown.Add($"{title}|{message}");
            return Task.FromResult(Answers.Count > 0 && Answers.Dequeue());
        }
    }

    private sealed class FakeNotifications : INotificationService
    {
        public List<SmokingData> Updates { get; } = [];
        public int PermissionRequests;
        public bool Throws;

        public Task<bool> RequestPermissionAsync()
        {
            PermissionRequests++;
            return Throws ? throw new InvalidOperationException("sin avisos") : Task.FromResult(true);
        }

        public Task ShowSmokingAvailableNotificationAsync() => Task.CompletedTask;
        public Task ScheduleNextNotificationAsync(DateTime nextTime) => Task.CompletedTask;

        public Task UpdatePersistentStatusAsync(SmokingData data)
        {
            Updates.Add(data);
            return Task.CompletedTask;
        }

        public SmokingTip GetRandomTip() => new();
        public Task<List<SmokingTip>> GetTipsHistoryAsync() => Task.FromResult(new List<SmokingTip>());
        public Task SaveTipShownAsync(SmokingTip tip) => Task.CompletedTask;
    }

    /// <summary>Datos en memoria; puede fallar a proposito.</summary>
    private sealed class FakeData : ISmokingDataService
    {
        public SmokingData Data { get; } = new();
        public List<DailySmokingRecord> History { get; } = [];
        public bool Throws;
        public List<string> Calls { get; } = [];

        public Task<SmokingData> GetDataAsync() => Throws ? throw new IOException("disco roto") : Task.FromResult(Data);
        public Task SaveDataAsync(SmokingData data) => Task.CompletedTask;

        public Task AddSmokedCigaretteAsync()
        {
            if (Throws) throw new IOException("disco roto");
            Data.SmokedToday++;
            Data.SmokingTimes.Add(DateTime.Now);
            return Task.CompletedTask;
        }

        public Task UpdateMaxCigarettesAsync(int maxCigarettes)
        {
            Calls.Add($"max {maxCigarettes}");
            return Task.CompletedTask;
        }

        public Task UpdateScheduleAsync(TimeSpan wakeUpTime, TimeSpan sleepTime)
        {
            Calls.Add($"horario {wakeUpTime:hh\\:mm}-{sleepTime:hh\\:mm}");
            return Task.CompletedTask;
        }

        public Task UpdatePriceConfigurationAsync(decimal packPrice, int cigarettesPerPack, string currency)
        {
            Calls.Add($"precio {packPrice} {cigarettesPerPack} {currency}");
            return Task.CompletedTask;
        }

        public Task AddSmokedCigaretteWithPriceAsync(decimal price, string currency) => Task.CompletedTask;
        public Task<List<DailySmokingRecord>> GetHistoryAsync(int days = 30) => Task.FromResult(History);
    }

    private sealed class FakePower : IPowerSettingsService
    {
        public bool Ignoring;
        public bool RequestOk;
        public List<string> Opened { get; } = [];
        public bool IsIgnoringBatteryOptimizations() => Ignoring;
        public Task<bool> RequestIgnoreBatteryOptimizationsAsync() => Task.FromResult(RequestOk);
        public void OpenBatteryOptimizationSettings() => Opened.Add("bateria");
        public void OpenAutostartSettings() => Opened.Add("inicio");
        public void OpenBackgroundSettings() => Opened.Add("segundo plano");
    }

    private sealed class FakeEmail(Exception? error = null) : IEmailService
    {
        public List<string> Sent { get; } = [];
        public Task SendEmailAsync(string email, string subject, string body)
        {
            if (error is not null) throw error;
            Sent.Add($"{email}|{subject}|{body}");
            return Task.CompletedTask;
        }
    }

    private sealed class FakeLinks : ILinkOpener
    {
        public List<string> Opened { get; } = [];
        public Task OpenAsync(string url)
        {
            Opened.Add(url);
            return Task.CompletedTask;
        }
    }

    // -----------------------------------------------------------------------
    // Inicio
    // -----------------------------------------------------------------------

    [Fact]
    public async Task InicioSinCigarrosTodaviaEnseñaGuiones()
    {
        var data = new FakeData();
        var vm = new MainViewModel(data, _notifications, _loc, random: new Random(1));
        var changes = 0;
        ((INotifyPropertyChanged)vm).PropertyChanged += (_, _) => changes++;

        await vm.LoadAsync(_dialogs);

        Assert.True(changes > 0);
        Assert.Same(data.Data, Assert.Single(_notifications.Updates));
        Assert.Equal(0, vm.Progress);
        Assert.Equal($"{_loc.GetString("main_smoked")}: 0/20", vm.ProgressText);
        Assert.Equal($"{_loc.GetString("main_remaining")}: 20", vm.RemainingText);
        Assert.Equal($"{_loc.GetString("main_last")}: {_loc.GetString("main_placeholder")}", vm.LastText);
        Assert.Equal($"{_loc.GetString("main_time_since")}: {_loc.GetString("main_placeholder")}", vm.SinceText);
        Assert.Equal($"{_loc.GetString("main_time_between")}: 00:48", vm.TimeBetweenText);
        Assert.Equal($"{_loc.GetString("main_awake_hours")}: 16:00", vm.AwakeText);
        Assert.False(vm.LimitReached);
        Assert.Equal(_loc.GetString("main_smoke_button"), vm.SmokeButtonText);
        Assert.NotEmpty(vm.TipTitle);
        Assert.NotEmpty(vm.TipText);
        Assert.EndsWith(".png", vm.TipIcon);
        Assert.Equal(_loc.GetString("main_tip_smart"), vm.TipSmart);
        Assert.Equal(_loc.GetString("main_daily_stats"), vm.DailyStats);
        Assert.Equal(_loc.GetString("main_refresh"), vm.RefreshText);
    }

    [Fact]
    public async Task FumarApuntaYCuentaElTiempoDesdeElUltimo()
    {
        var data = new FakeData();
        var vm = new MainViewModel(data, _notifications, _loc, now: () => DateTime.Now.AddMinutes(90));
        await vm.LoadAsync(_dialogs);

        await vm.SmokeAsync(_dialogs);

        Assert.Equal(1, data.Data.SmokedToday);
        Assert.Equal(0.05, vm.Progress, 3);
        Assert.Contains(":", vm.LastText);
        Assert.Contains("1", vm.SinceText);
        Assert.Equal($"{_loc.GetString("main_registered_title")}|{_loc.GetString("main_registered_message")}", _dialogs.Shown[^1]);
        Assert.Same(data.Data, vm.Data);
    }

    [Fact]
    public async Task PasadoElMaximoSePreguntaDosVeces()
    {
        var data = new FakeData();
        data.Data.MaxCigarettesPerDay = 2;
        data.Data.SmokedToday = 2;
        var vm = new MainViewModel(data, _notifications, _loc);
        await vm.LoadAsync(_dialogs);
        Assert.True(vm.LimitReached);
        Assert.Equal(_loc.GetString("main_limit_reached_button"), vm.SmokeButtonText);
        Assert.Equal(1, vm.Progress);

        await vm.SmokeAsync(_dialogs);                 // no a la primera
        _dialogs.Answers.Enqueue(true);
        await vm.SmokeAsync(_dialogs);                 // si y luego no
        Assert.Equal(2, data.Data.SmokedToday);

        _dialogs.Answers.Enqueue(true);
        _dialogs.Answers.Enqueue(true);
        await vm.SmokeAsync(_dialogs);
        Assert.Equal(3, data.Data.SmokedToday);
        Assert.Equal($"{_loc.GetString("main_remaining")}: 0", vm.RemainingText);   // nunca negativo
    }

    [Fact]
    public async Task UnFalloAlLeerOAlApuntarSeAvisa()
    {
        var data = new FakeData { Throws = true };
        var vm = new MainViewModel(data, _notifications, _loc);

        await vm.LoadAsync(_dialogs);
        await vm.SmokeAsync(_dialogs);

        Assert.Equal($"{_loc.GetString("error")}|{_loc.GetString("main_error_loading")}: disco roto", _dialogs.Shown[0]);
        Assert.Equal($"{_loc.GetString("error")}|{_loc.GetString("main_error_register")}: disco roto", _dialogs.Shown[1]);
    }

    [Fact]
    public async Task ElSiguienteRecomendadoYRefrescar()
    {
        var data = new FakeData();
        data.Data.SmokingTimes.Add(DateTime.Now.AddMinutes(-10));
        data.Data.SmokedToday = 1;
        var vm = new MainViewModel(data, _notifications, _loc);

        await vm.RefreshAsync(_dialogs);

        Assert.Single(_notifications.Updates);
        Assert.StartsWith(_loc.GetString("main_next"), vm.NextText);
    }

    [Fact]
    public async Task ConMaximoCeroNoSeDivide()
    {
        var data = new FakeData();
        data.Data.MaxCigarettesPerDay = 0;
        var vm = new MainViewModel(data, _notifications, _loc);
        await vm.LoadAsync(_dialogs);
        Assert.Equal(0, vm.Progress);
    }

    // -----------------------------------------------------------------------
    // Historico
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ElHistoricoPasaDeCargandoAMostrarLasCuentas()
    {
        var data = new FakeData();
        data.Data.Currency = "USD";
        data.History.Add(new DailySmokingRecord { Date = DateTime.Today.AddDays(-1), Count = 10, Times = [.. Enumerable.Repeat(DateTime.Today.AddDays(-1), 10)] });
        var states = new List<bool>();
        var vm = new HistoryViewModel(data, _loc, _ => { return Task.CompletedTask; });
        ((INotifyPropertyChanged)vm).PropertyChanged += (_, _) => states.Add(vm.IsLoading);
        Assert.True(vm.IsLoading);
        Assert.Equal(_loc.GetString("history_summary_loading"), vm.Summary);

        await vm.LoadAsync(_dialogs);

        Assert.Equal([true, false], states);
        Assert.True(vm.IsLoaded);
        Assert.Equal(("10", "1", "10.0"), (vm.TotalCigarettes, vm.TotalDays, vm.AveragePerDay.Replace(',', '.')));
        Assert.StartsWith("$", vm.TotalSpent);
        Assert.StartsWith("$", vm.TotalSaved);
        Assert.StartsWith("$", vm.AverageSpent);
        Assert.EndsWith("%", vm.SavingsPercentage);
        Assert.EndsWith("%", vm.Reduction);
        Assert.NotEqual(_loc.GetString("history_summary_loading"), vm.Summary);
        string[] captions = [vm.Title, vm.LoadingText, vm.GeneralStats, vm.TotalCigarettesCaption, vm.DaysRecordedCaption,
            vm.AvgPerDayCaption, vm.ReductionCaption, vm.EconomicStats, vm.TotalSpentCaption, vm.TotalSavedCaption,
            vm.SavingsPctCaption, vm.RefreshText];
        Assert.All(captions, c => Assert.False(string.IsNullOrWhiteSpace(c)));
    }

    [Fact]
    public async Task UnFalloEnElHistoricoSeAvisaYDejaDeCargar()
    {
        var vm = new HistoryViewModel(new FakeData { Throws = true }, _loc, _ => Task.CompletedTask);
        await vm.LoadAsync(_dialogs);
        Assert.False(vm.IsLoading);
        Assert.StartsWith($"{_loc.GetString("error")}|{_loc.GetString("history_error_loading")}: ", _dialogs.Shown[0]);
        Assert.NotNull(new HistoryViewModel(new FakeData(), _loc));   // con la espera de verdad por defecto
    }

    // -----------------------------------------------------------------------
    // Configuracion
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(7, 23, "16", "01:00")]
    [InlineData(22, 6, "8", "01:00")]     // pasa la medianoche
    [InlineData(7, 23, "0", "--")]
    [InlineData(7, 23, "veinte", "--")]
    public void TiempoEntreCigarros(int wake, int sleep, string max, string expected)
    {
        var vm = new SettingsViewModel(new FakeData(), _loc, null);
        Assert.Equal($"{_loc.GetString("settings_time_between")}: {expected}", vm.TimeBetweenText(TimeSpan.FromHours(wake), TimeSpan.FromHours(sleep), max));
    }

    [Fact]
    public void PrecioPorCigarro()
    {
        var vm = new SettingsViewModel(new FakeData(), _loc, null);
        var euro = vm.Currencies().First(c => c.Code == "EUR");
        Assert.Matches(@"€0[.,]250$", vm.PricePerCigaretteText("5", "20", euro));
        Assert.EndsWith(": --", vm.PricePerCigaretteText("5", "0", euro));
        Assert.EndsWith(": --", vm.PricePerCigaretteText("cinco", "20", euro));
        Assert.EndsWith(": --", vm.PricePerCigaretteText("5", "20", null));
    }

    [Fact]
    public async Task CargarYGuardarLoQueSePueda()
    {
        var data = new FakeData();
        var vm = new SettingsViewModel(data, _loc, null);

        var form = await vm.LoadAsync(_dialogs);
        Assert.Equal(new SettingsForm("20", new TimeSpan(7, 0, 0), new TimeSpan(23, 0, 0), 5m.ToString("F2"), "20", "EUR"), form);

        await vm.SaveAsync(form! with { MaxPerDay = "15", PackPrice = "6" });
        Assert.Equal(["max 15", "horario 07:00-23:00", "precio 6 20 EUR"], data.Calls);

        data.Calls.Clear();
        await vm.SaveAsync(form! with { MaxPerDay = "0", PerPack = "x", CurrencyCode = null });
        Assert.Equal(["horario 07:00-23:00"], data.Calls);
    }

    [Fact]
    public async Task UnFalloAlCargarLaConfiguracionSeAvisa()
    {
        var vm = new SettingsViewModel(new FakeData { Throws = true }, _loc, null);
        Assert.Null(await vm.LoadAsync(_dialogs));
        Assert.StartsWith($"{_loc.GetString("error")}|{_loc.GetString("settings_error_loading")}", _dialogs.Shown[0]);
    }

    [Theory]
    [InlineData("5", "4")]
    [InlineData("1", null)]
    [InlineData("", null)]
    public void BajarElMaximo(string current, string? expected) => Assert.Equal(expected, SettingsViewModel.Decrease(current));

    [Theory]
    [InlineData("5", "6")]
    [InlineData("", "1")]
    public void SubirElMaximo(string current, string expected) => Assert.Equal(expected, SettingsViewModel.Increase(current));

    [Fact]
    public async Task PermisosSinAndroidLoExplican()
    {
        var vm = new SettingsViewModel(new FakeData(), _loc, null);
        await vm.CheckPermissionsAsync(_dialogs);
        await vm.ConfigureAllAsync(_dialogs);
        await vm.BatteryOptimizationAsync(_dialogs);
        await vm.AutostartAsync(_dialogs);
        Assert.Equal(4, _dialogs.Shown.Count(s => s.EndsWith(_loc.GetString("settings_android_only"))));
        Assert.Equal(_loc.GetString("settings_unverified"), vm.BatteryStatus);
    }

    [Fact]
    public async Task PermisosEnAndroid()
    {
        var power = new FakePower();
        var vm = new SettingsViewModel(new FakeData(), _loc, power);
        Assert.Equal(_loc.GetString("settings_manual"), vm.AutostartStatus);

        await vm.CheckPermissionsAsync(_dialogs);
        Assert.Equal(_loc.GetString("settings_battery_optimized"), vm.BatteryStatus);
        Assert.Equal(_loc.GetString("settings_check_system"), vm.AutostartStatus);
        power.Ignoring = true;
        await vm.CheckPermissionsAsync(_dialogs);
        Assert.Equal(_loc.GetString("settings_battery_excluded"), vm.BatteryStatus);
        vm.Relocalize();   // ya comprobados: no vuelven a «sin comprobar»
        Assert.Equal(_loc.GetString("settings_battery_excluded"), vm.BatteryStatus);

        await vm.ConfigureAllAsync(_dialogs);
        Assert.Equal(["bateria", "inicio", "segundo plano"], power.Opened);
        Assert.Equal($"{_loc.GetString("settings_permissions_result_title")}|{_loc.GetString("settings_permissions_result_message")}", _dialogs.Shown[^1]);

        power.Opened.Clear();
        power.RequestOk = true;
        await vm.ConfigureAllAsync(_dialogs);
        await vm.BatteryOptimizationAsync(_dialogs);
        await vm.AutostartAsync(_dialogs);
        Assert.Equal(["inicio", "segundo plano", "inicio"], power.Opened);
        power.RequestOk = false;
        await vm.BatteryOptimizationAsync(_dialogs);
        Assert.Equal("bateria", power.Opened[^1]);

        await vm.SavedAsync(_dialogs);
        Assert.Equal($"{_loc.GetString("settings_saved_title")}|{_loc.GetString("settings_saved_message")}", _dialogs.Shown[^1]);
    }

    [Fact]
    public void RotulosDeConfiguracionEnLosDosIdiomas()
    {
        var vm = new SettingsViewModel(new FakeData(), _loc, null);
        string[] Texts() => [vm.Title, vm.Subtitle, vm.BatterySave, vm.Autostart, vm.PermissionsTitle, vm.CheckPermissions,
            vm.ConfigureAll, vm.Battery, vm.PermissionsHint, vm.MaxPerDay, vm.Schedule, vm.WakeTime, vm.SleepTime,
            vm.PriceConfig, vm.PackPrice, vm.PerPack, vm.CurrencyText, vm.Save, vm.ReduceMax];
        var es = Texts();
        _loc.SetLanguage("en");
        vm.Relocalize();
        var en = Texts();
        Assert.All(es.Concat(en), t => Assert.False(string.IsNullOrWhiteSpace(t)));
        Assert.True(es.Zip(en).Count(p => p.First != p.Second) >= 15);
        Assert.Equal(_loc.GetString("settings_unverified"), vm.BatteryStatus);
    }

    [Fact]
    public void HorasDespierto()
    {
        Assert.Equal(TimeSpan.FromHours(16), SettingsViewModel.AwakeTime(TimeSpan.FromHours(7), TimeSpan.FromHours(23)));
        Assert.Equal(TimeSpan.FromHours(8), SettingsViewModel.AwakeTime(TimeSpan.FromHours(22), TimeSpan.FromHours(6)));
        Assert.Equal(TimeSpan.FromHours(24), SettingsViewModel.AwakeTime(TimeSpan.FromHours(8), TimeSpan.FromHours(8)));
    }

    // -----------------------------------------------------------------------
    // Acerca de
    // -----------------------------------------------------------------------

    [Fact]
    public async Task AcercaDeEnLosDosIdiomas()
    {
        var vm = new AboutViewModel(_loc, "2026.10.01.0", new FakeEmail());
        string[] Texts() => [vm.Title, vm.Description, vm.ContactTitle, vm.ContactHint, vm.LanguageTitle, vm.LanguageHint,
            vm.PrivacyTitle, vm.PrivacyText, vm.LicenseTitle, vm.LicenseText, vm.LegalTitle, vm.LegalText1, vm.LegalText2, vm.LegalWarning];
        var es = Texts();
        Assert.True(vm.SpanishActive);
        Assert.EndsWith("2026.10.01.0", vm.Version);
        Assert.NotEqual(vm.SpanishText, vm.EnglishText);
        Assert.False(string.IsNullOrWhiteSpace(vm.LicenseLine));

        await vm.SetLanguageAsync("en", _dialogs);

        Assert.True(vm.EnglishActive);
        Assert.False(vm.SpanishActive);
        Assert.True(es.Zip(Texts()).Count(p => p.First != p.Second) >= 12);
        Assert.Equal($"{_loc.GetString("language")}|{_loc.GetString("language_selected")}", _dialogs.Shown[0]);
    }

    [Fact]
    public async Task ContactarPorCorreo()
    {
        var email = new FakeEmail();
        await new AboutViewModel(_loc, "1", email).ContactAsync(_dialogs);
        var sent = Assert.Single(email.Sent).Split('|');
        Assert.Equal(AboutViewModel.ContactEmail, sent[0]);
        Assert.Equal(_loc.GetString("email_subject"), sent[1]);
        Assert.Contains("QuitSmoke", sent[2]);

        await new AboutViewModel(_loc, "1", new FakeEmail(new FeatureNotSupportedException())).ContactAsync(_dialogs);
        await new AboutViewModel(_loc, "1", new FakeEmail(new InvalidOperationException("sin red"))).ContactAsync(_dialogs);
        Assert.Equal($"{_loc.GetString("error")}|{_loc.GetString("email_error")}", _dialogs.Shown[0]);
        Assert.Equal($"{_loc.GetString("error")}|{_loc.GetString("email_error_message")}: sin red", _dialogs.Shown[1]);
    }

    [Fact]
    public async Task FueraDelMovilElCorreoYElNavegadorDeVerdadNoExisten()
    {
        await Assert.ThrowsAnyAsync<Exception>(() => new EssentialsEmailService().SendEmailAsync("a@b.c", "s", "b"));
        await Assert.ThrowsAnyAsync<Exception>(() => new BrowserLinkOpener().OpenAsync("https://example.org"));
    }

    // -----------------------------------------------------------------------
    // Version y notificacion
    // -----------------------------------------------------------------------

    private UpdateService Updates(FakeLinks links, string json) =>
        new(_loc, links, () => Task.FromResult(json), () => "2026.10.01.0");

    [Fact]
    public async Task VersionNuevaSeOfreceUnaVez()
    {
        var links = new FakeLinks();
        var updates = Updates(links, """{"version":"2026.10.2.0","url":"https://example.org/qs"}""");
        _dialogs.Answers.Enqueue(true);

        await updates.CheckAndPromptAsync(_dialogs);
        await updates.CheckAndPromptAsync(_dialogs);

        Assert.Single(_dialogs.Shown);
        Assert.Equal(["https://example.org/qs"], links.Opened);
    }

    [Theory]
    [InlineData("""{"version":"2026.10.01.0","url":"x"}""", false)]
    [InlineData("""{"url":"x"}""", false)]
    [InlineData("roto", false)]
    [InlineData("""{"version":"2099.1"}""", true)]   // pregunta, dice que no
    public async Task SinVersionNuevaOSinRespuestaNoSeAbreNada(string json, bool asks)
    {
        var links = new FakeLinks();
        await Updates(links, json).CheckAndPromptAsync(_dialogs);
        Assert.Equal(asks, _dialogs.Shown.Count == 1);
        Assert.Empty(links.Opened);
    }

    [Theory]
    [InlineData("1.10.0", "1.9.9", 1)]
    [InlineData("1.0", "1.0.0", 0)]
    [InlineData("1.a", "1.1", -1)]
    public void CompararVersiones(string a, string b, int sign) =>
        Assert.Equal(sign, Math.Sign(UpdateService.CompareVersions(a, b)));

    [Fact]
    public async Task LaNotificacionSePoneAlDiaAlArrancarYElBotonFumarApunta()
    {
        var data = new FakeData();
        await NotificationActions.InitAsync(_notifications, data);
        Assert.Equal(1, _notifications.PermissionRequests);
        Assert.Single(_notifications.Updates);

        Assert.True(await NotificationActions.HandleAsync(NotificationService.SmokeActionId, data, _notifications));
        Assert.Equal(1, data.Data.SmokedToday);
        Assert.False(await NotificationActions.HandleAsync(999, data, _notifications));
        Assert.False(await NotificationActions.HandleAsync(NotificationService.SmokeActionId, null, _notifications));
    }

    [Fact]
    public async Task UnFalloDeLaNotificacionNoParaNada()
    {
        await NotificationActions.InitAsync(null, new FakeData());
        await NotificationActions.InitAsync(new FakeNotifications { Throws = true }, new FakeData());
        Assert.False(await NotificationActions.HandleAsync(NotificationService.SmokeActionId, new FakeData { Throws = true }, _notifications));
    }
}
