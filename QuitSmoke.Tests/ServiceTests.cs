using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using QuitSmoke.Models;
using QuitSmoke.Services;

namespace QuitSmoke.Tests;

/// <summary>Datos del día e historial en disco, estadísticas del Histórico, consejos vistos y textos es/en.</summary>
public partial class ServiceTests : IDisposable
{
    public ServiceTests()
    {
        FileSystem.AppDataDirectory = Path.Combine(Path.GetTempPath(), $"quitsmoke-{Guid.NewGuid():N}");
        Directory.CreateDirectory(FileSystem.AppDataDirectory);
        Preferences.Values.Clear();
    }

    public void Dispose()
    {
        Preferences.Values.Clear();
        try { Directory.Delete(FileSystem.AppDataDirectory, true); } catch (IOException) { }
    }

    private static string DataFile => Path.Combine(FileSystem.AppDataDirectory, "smoking_data.json");
    private static string HistoryFile => Path.Combine(FileSystem.AppDataDirectory, "smoking_history.json");

    // -----------------------------------------------------------------------
    // Datos del día
    // -----------------------------------------------------------------------

    [Fact]
    public async Task PrimeraVezDaElPlanPorDefecto()
    {
        var data = await new SmokingDataService().GetDataAsync();
        Assert.Equal(20, data.MaxCigarettesPerDay);
        Assert.Empty(await new SmokingDataService().GetHistoryAsync());
    }

    [Fact]
    public async Task FumarGuardaElCigarroConSuPrecioYLoApuntaEnElHistorial()
    {
        var service = new SmokingDataService();
        await service.UpdatePriceConfigurationAsync(6m, 20, "USD");

        await service.AddSmokedCigaretteAsync();
        await service.AddSmokedCigaretteWithPriceAsync(0.5m, "EUR");

        var data = await service.GetDataAsync();
        Assert.Equal(2, data.SmokedToday);
        Assert.Equal([0.3m, 0.5m], data.SmokedCigarettes.Select(c => c.Price));
        Assert.Equal(["USD", "EUR"], data.SmokedCigarettes.Select(c => c.Currency));

        var day = Assert.Single(await service.GetHistoryAsync());
        Assert.Equal(DateTime.Today, day.Date);
        Assert.Equal(2, day.Count);
        Assert.Equal(0.8m, day.TotalSmokedValue);

        // Otra instancia (la app reiniciada) lo lee del disco.
        var again = await new SmokingDataService().GetDataAsync();
        Assert.Equal((2, 6m, "USD"), (again.SmokedToday, again.PackPrice, again.Currency));
    }

    [Fact]
    public async Task CambiarPlanYHorario()
    {
        var service = new SmokingDataService();
        await service.UpdateMaxCigarettesAsync(12);
        await service.UpdateScheduleAsync(new TimeSpan(8, 0, 0), new TimeSpan(0, 30, 0));

        var data = await new SmokingDataService().GetDataAsync();
        Assert.Equal(12, data.MaxCigarettesPerDay);
        Assert.Equal((new TimeSpan(8, 0, 0), new TimeSpan(0, 30, 0)), (data.WakeUpTime, data.SleepTime));

        await service.UpdateMaxCigarettesAsync(0);
        Assert.Equal(1, (await service.GetDataAsync()).MaxCigarettesPerDay);   // nunca menos de uno
        await service.UpdateMaxCigarettesAsync(-5);
        Assert.Equal(1, (await service.GetDataAsync()).MaxCigarettesPerDay);
    }

    [Fact]
    public async Task AlCambiarDeDiaLoDeAyerPasaAlHistorialYHoyEmpiezaDeCero()
    {
        var yesterday = DateTime.Today.AddDays(-1);
        var old = new SmokingData
        {
            MaxCigarettesPerDay = 10,
            SmokedToday = 2,
            LastResetDate = yesterday,
            SmokingTimes = [yesterday.AddHours(9), yesterday.AddHours(12)],
            SmokedCigarettes =
            [
                new SmokedCigarette { SmokedAt = yesterday.AddHours(9), Price = 0.25m },
                new SmokedCigarette { SmokedAt = yesterday.AddHours(12), Price = 0.25m },
            ],
        };
        await File.WriteAllTextAsync(DataFile, JsonSerializer.Serialize(old));

        var service = new SmokingDataService();
        var data = await service.GetDataAsync();

        Assert.Equal((0, DateTime.Today), (data.SmokedToday, data.LastResetDate));
        Assert.Empty(data.SmokingTimes);
        Assert.Empty(data.SmokedCigarettes);
        Assert.Equal(10, data.MaxCigarettesPerDay);   // el plan se conserva
        var day = Assert.Single(await service.GetHistoryAsync());
        Assert.Equal((yesterday, 2, 0.5m), (day.Date, day.Count, day.TotalSmokedValue));
    }

    [Fact]
    public async Task ConLaAppAbiertaTambienSeCambiaDeDia()
    {
        var service = new SmokingDataService();
        var data = await service.GetDataAsync();   // queda en cache
        var yesterday = DateTime.Today.AddDays(-1);
        data.LastResetDate = yesterday;
        data.SmokingTimes.Add(yesterday.AddHours(20));
        data.SmokedToday = 1;

        data = await service.GetDataAsync();

        Assert.Equal(0, data.SmokedToday);
        Assert.Equal(yesterday, Assert.Single(await service.GetHistoryAsync()).Date);
    }

    [Fact]
    public async Task DatosIncoherentesSeArreglanAlLeer()
    {
        var today = DateTime.Today;
        var bad = new SmokingData
        {
            SmokedToday = 7,   // no cuadra con los tiempos
            LastResetDate = today.AddDays(1),   // reloj adelantado
            SmokingTimes = [today.AddHours(8), today.AddDays(1).AddHours(1)],
            SmokedCigarettes = [new SmokedCigarette { SmokedAt = today.AddDays(2), Price = 1 }],
        };
        await File.WriteAllTextAsync(DataFile, JsonSerializer.Serialize(bad));

        var data = await new SmokingDataService().GetDataAsync();

        Assert.Equal(1, data.SmokedToday);
        Assert.Equal([today.AddHours(8)], data.SmokingTimes);
        Assert.Empty(data.SmokedCigarettes);
        Assert.Equal(today, data.LastResetDate);
        Assert.Equal(1, JsonSerializer.Deserialize<SmokingData>(await File.ReadAllTextAsync(DataFile))!.SmokedToday);   // y se guarda
    }

    [Fact]
    public async Task FicherosRotosNoRompenLaApp()
    {
        await File.WriteAllTextAsync(DataFile, "{roto");
        await File.WriteAllTextAsync(HistoryFile, "{roto");
        var service = new SmokingDataService();

        Assert.Equal(20, (await service.GetDataAsync()).MaxCigarettesPerDay);
        Assert.Empty(await service.GetHistoryAsync());
    }

    [Fact]
    public async Task HistorialSoloDeLosUltimosDiasYOrdenado()
    {
        var days = Enumerable.Range(0, 40).Select(i => new DailySmokingRecord { Date = DateTime.Today.AddDays(-i), Count = 1 }).Reverse().ToList();
        await File.WriteAllTextAsync(HistoryFile, JsonSerializer.Serialize(days.OrderByDescending(d => d.Date)));
        var service = new SmokingDataService();

        var last30 = await service.GetHistoryAsync();
        Assert.Equal(30, last30.Count);
        Assert.Equal(DateTime.Today.AddDays(-29), last30[0].Date);
        Assert.Equal(DateTime.Today, last30[^1].Date);
        Assert.Equal(7, (await service.GetHistoryAsync(7)).Count);
    }

    [Fact]
    public async Task SinPoderEscribirSeSigueEnMemoria()
    {
        FileSystem.AppDataDirectory = Path.Combine(FileSystem.AppDataDirectory, "no", "existe");
        var service = new SmokingDataService();

        await service.AddSmokedCigaretteAsync();

        Assert.Equal(1, (await service.GetDataAsync()).SmokedToday);
        Assert.Empty(await service.GetHistoryAsync());
    }

    // -----------------------------------------------------------------------
    // Estadísticas del Histórico
    // -----------------------------------------------------------------------

    [Fact]
    public void EstadisticasConPrecioGuardadoYSinel()
    {
        var data = new SmokingData { MaxCigarettesPerDay = 10, PackPrice = 5m, CigarettesPerPack = 20 };   // 0,25 el cigarro
        var history = new List<DailySmokingRecord>
        {
            // Dia con precio de cada cigarro.
            new() { Date = DateTime.Today.AddDays(-2), Count = 4, Times = [.. Enumerable.Repeat(DateTime.Today, 4)],
                    SmokedCigarettes = [.. Enumerable.Repeat(new SmokedCigarette { Price = 0.3m }, 4)] },
            // Dia antiguo sin precios: se usa el actual.
            new() { Date = DateTime.Today.AddDays(-1), Count = 6, Times = [.. Enumerable.Repeat(DateTime.Today, 6)] },
            // Dia sin cigarros: no cuenta.
            new() { Date = DateTime.Today, Count = 0 },
        };

        var s = HistoryStatistics.Calculate(history, data);

        Assert.Equal(10, s.TotalCigarettes);
        Assert.Equal(2, s.DaysWithCigarettes);
        Assert.Equal(5, s.AveragePerDay);
        Assert.Equal(20, s.TotalPlanned);
        Assert.Equal(50, s.ReductionPercentage, 9);
        Assert.Equal(1.2m + 1.5m, s.TotalSpent);
        Assert.Equal(10 * 0.25m, s.TotalSaved);   // 20 previstos - 10 fumados
        Assert.Equal(5 * 0.25, s.AverageSpentPerDay, 9);
        Assert.Equal(2.5 / 2.7 * 100, s.SavingsPercentage, 9);
    }

    [Fact]
    public void EstadisticasSinHistorialOPorEncimaDelPlan()
    {
        var data = new SmokingData { MaxCigarettesPerDay = 2 };
        Assert.Equal(new HistoryStatistics(0, 0, 0, 0, 0, 0, 0, 0, 0), HistoryStatistics.Calculate([], data));

        var over = HistoryStatistics.Calculate(
            [new DailySmokingRecord { Date = DateTime.Today, Count = 3, Times = [DateTime.Today, DateTime.Today, DateTime.Today] }], data);
        Assert.Equal(-50, over.ReductionPercentage, 9);   // se fumo mas de lo previsto
        Assert.Equal(0, over.TotalSaved);                  // el ahorro no baja de cero
    }

    // -----------------------------------------------------------------------
    // Consejos vistos
    // -----------------------------------------------------------------------

    [Fact]
    public async Task ConsejosNoSeRepitenHastaAgotarlos()
    {
        Preferences.Values["app_language"] = "en";
        var notifications = new NotificationService(new LocalizationService());
        var all = SmokingTips.GetAllTips("en");

        foreach (var tip in all.Take(all.Count - 1))
            await notifications.SaveTipShownAsync(tip);

        // Solo queda uno sin ver: tiene que salir ese.
        Assert.Equal(all[^1].Message, notifications.GetRandomTip().Message);
        Assert.Equal(all[^1].Message, new NotificationService(new LocalizationService()).GetRandomTip().Message);   // tambien leyendo del disco

        await notifications.SaveTipShownAsync(all[^1]);
        Assert.Contains(notifications.GetRandomTip(), all);   // vistos todos: vuelve a cualquiera
        Assert.Equal(all.Count, (await notifications.GetTipsHistoryAsync()).Count);
    }

    [Fact]
    public async Task HistorialDeConsejosVacioOIlegible()
    {
        var notifications = new NotificationService(new LocalizationService());
        Assert.Empty(await notifications.GetTipsHistoryAsync());

        await File.WriteAllTextAsync(Path.Combine(FileSystem.AppDataDirectory, "tips_history.json"), "{roto");
        var broken = new NotificationService(new LocalizationService());
        Assert.Empty(await broken.GetTipsHistoryAsync());
        Assert.NotNull(broken.GetRandomTip());
    }

    [Fact]
    public async Task HistorialDeConsejosSeLeeDelDisco()
    {
        var first = new NotificationService(new LocalizationService());
        var tip = SmokingTips.GetAllTips("es")[3];
        await first.SaveTipShownAsync(tip);

        var history = await new NotificationService(new LocalizationService()).GetTipsHistoryAsync();
        var seen = Assert.Single(history);
        Assert.Equal((tip.Title, tip.Message, tip.Icon), (seen.Title, seen.Message, seen.Icon));
    }

    [Fact]
    public async Task LosAvisosSinSistemaDeNotificacionesNoLanzan()
    {
        var notifications = new NotificationService(new LocalizationService());
        Assert.False(await notifications.RequestPermissionAsync());
        await notifications.ShowSmokingAvailableNotificationAsync();
        await notifications.ScheduleNextNotificationAsync(DateTime.Now.AddHours(1));
        await notifications.UpdatePersistentStatusAsync(new SmokingData());
        await notifications.UpdatePersistentStatusAsync(new SmokingData { SmokedToday = 20 });
        await notifications.UpdatePersistentStatusAsync(new SmokingData { SmokingTimes = [DateTime.Now.AddHours(-5)] });
        await notifications.UpdatePersistentStatusAsync(new SmokingData { SmokingTimes = [DateTime.Now] });
    }

    // -----------------------------------------------------------------------
    // Textos
    // -----------------------------------------------------------------------

    private static Dictionary<string, Dictionary<string, string>> Tables() =>
        (Dictionary<string, Dictionary<string, string>>)typeof(LocalizationService)
            .GetField("_translations", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(new LocalizationService())!;

    [GeneratedRegex(@"\{(\d+)")]
    private static partial Regex Hole();

    [Fact]
    public void LasDosTablasTienenLasMismasClavesYHuecos()
    {
        var t = Tables();
        Assert.Equal(["es", "en"], t.Keys);
        Assert.Empty(t["es"].Keys.Except(t["en"].Keys));
        Assert.Empty(t["en"].Keys.Except(t["es"].Keys));
        foreach (var (key, es) in t["es"])
        {
            var en = t["en"][key];
            Assert.False(string.IsNullOrWhiteSpace(es), key);
            Assert.False(string.IsNullOrWhiteSpace(en), key);
            Assert.True(
                Hole().Matches(es).Select(m => m.Value).Distinct().Order().SequenceEqual(Hole().Matches(en).Select(m => m.Value).Distinct().Order()),
                $"{key}: huecos distintos");
        }
    }

    private static IEnumerable<string> AppSources(DirectoryInfo dir)
    {
        foreach (var f in dir.EnumerateFiles("*.cs"))
            yield return f.FullName;
        foreach (var sub in dir.EnumerateDirectories())
        {
            if (sub.Name is "bin" or "obj" or "QuitSmoke.Tests" or "constitution" or "releases" or ".git")
                continue;
            foreach (var f in AppSources(sub))
                yield return f;
        }
    }

    [Fact]
    public void TodoTextoQuePideLaAppExiste()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(dir!.FullName, "QuitSmoke.csproj")))
            dir = dir.Parent;

        var used = new HashSet<string>();
        foreach (var file in AppSources(dir))
        {
            foreach (Match m in Regex.Matches(File.ReadAllText(file), @"(?:GetString|\bL)\(""(\w+)""\)"))
                used.Add(m.Groups[1].Value);
        }

        Assert.True(used.Count > 40, $"solo {used.Count} claves");
        var es = Tables()["es"];
        var missing = used.Where(k => !es.ContainsKey(k)).ToList();
        Assert.True(missing.Count == 0, "Faltan: " + string.Join(", ", missing));
    }

    [Fact]
    public void IdiomaElegidoFallbackAlInglesYClaveSinTexto()
    {
        var loc = new LocalizationService();
        loc.SetLanguage("es");
        Assert.Equal(("es", "es"), (loc.GetCurrentLanguage(), Preferences.Values["app_language"]));
        Assert.Equal("Cancelar", loc.GetString("cancel"));
        Assert.Equal("Cancelar", new LocalizationService().GetString("cancel"));   // otra instancia coge lo guardado

        loc.SetLanguage("fr");   // no soportado: no cambia
        Assert.Equal("es", loc.GetCurrentLanguage());

        loc.SetLanguage("en");
        Assert.Equal("Cancel", loc.GetString("cancel"));

        // Una clave que faltara en castellano saldria en ingles; una que no existe, como clave.
        loc.SetLanguage("es");
        var tables = (Dictionary<string, Dictionary<string, string>>)typeof(LocalizationService)
            .GetField("_translations", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(loc)!;
        tables["es"].Remove("cancel");
        Assert.Equal("Cancel", loc.GetString("cancel"));
        Assert.Equal("no_existe", loc.GetString("no_existe"));
    }

    /// <summary>
    /// Fallo encontrado: el idioma guardado solo se leia al pedir un texto, asi que los consejos de
    /// un aviso lanzado con la app cerrada salian en el idioma del sistema.
    /// </summary>
    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    public void ElIdiomaGuardadoMandaDesdeElPrincipio(string lang)
    {
        Preferences.Values["app_language"] = lang;
        var loc = new LocalizationService();
        Assert.Equal(lang, loc.GetCurrentLanguage());   // sin haber pedido ningun texto antes
        Assert.Contains(new NotificationService(loc).GetRandomTip(), SmokingTips.GetAllTips(lang));
    }

    [Fact]
    public void SinIdiomaGuardadoSeUsaElDelSistema()
    {
        var system = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "es" ? "es" : "en";
        Assert.Equal(system, new LocalizationService().GetCurrentLanguage());

        Preferences.Values["app_language"] = "de";   // guardado raro: se ignora
        var loc = new LocalizationService();
        Assert.Equal(system == "es" ? "Cancelar" : "Cancel", loc.GetString("cancel"));
    }
}
