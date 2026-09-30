using QuitSmoke.Helpers;
using QuitSmoke.Models;
using QuitSmoke.Services;

namespace QuitSmoke.Tests;

/// <summary>El plan de reducción (horarios, intervalo, próximo cigarro), precios, divisas y consejos.</summary>
public class ModelTests
{
    [Fact]
    public void HorasDespiertoYIntervaloEntreCigarros()
    {
        var data = new SmokingData { MaxCigarettesPerDay = 16, WakeUpTime = new(7, 0, 0), SleepTime = new(23, 0, 0) };
        Assert.Equal(TimeSpan.FromHours(16), data.AwakeHours);
        Assert.Equal(TimeSpan.FromHours(1), data.TimeBetweenCigarettes);

        // Quien se acuesta pasada la medianoche.
        var noche = new SmokingData { MaxCigarettesPerDay = 10, WakeUpTime = new(10, 0, 0), SleepTime = new(2, 0, 0) };
        Assert.Equal(TimeSpan.FromHours(16), noche.AwakeHours);
        Assert.Equal(TimeSpan.FromMinutes(96), noche.TimeBetweenCigarettes);

        Assert.Equal(TimeSpan.FromHours(24), new SmokingData { WakeUpTime = new(8, 0, 0), SleepTime = new(8, 0, 0) }.AwakeHours);
        Assert.Equal(TimeSpan.Zero, new SmokingData { MaxCigarettesPerDay = 0 }.TimeBetweenCigarettes);
    }

    [Fact]
    public void ProximoCigarroRecomendado()
    {
        var data = new SmokingData { MaxCigarettesPerDay = 16, WakeUpTime = new(7, 0, 0), SleepTime = new(23, 0, 0) };
        Assert.Equal(DateTime.Today.AddHours(7), data.NextRecommendedTime);   // aun ninguno: al levantarse

        var last = DateTime.Today.AddHours(9).AddMinutes(15);
        data.SmokingTimes.Add(DateTime.Today.AddHours(8));
        data.SmokingTimes.Add(last);
        data.SmokedToday = 2;
        Assert.Equal(last.AddHours(1), data.NextRecommendedTime);
        Assert.Equal(14, data.RemainingCigarettes);

        data.SmokedToday = 16;
        Assert.Null(data.NextRecommendedTime);   // limite alcanzado
        data.SmokedToday = 18;
        Assert.Equal(-2, data.RemainingCigarettes);
    }

    [Fact]
    public void PreciosGastoYAhorroDelDia()
    {
        var data = new SmokingData { PackPrice = 6m, CigarettesPerPack = 20, MaxCigarettesPerDay = 10, SmokedToday = 4 };
        Assert.Equal(0.3m, data.PricePerCigarette);
        Assert.Equal(1.8m, data.TodaySavedValue);   // 6 que no se fumaron

        data.SmokedCigarettes.Add(new SmokedCigarette { SmokedAt = DateTime.Now, Price = 0.3m });
        data.SmokedCigarettes.Add(new SmokedCigarette { SmokedAt = DateTime.Now, Price = 0.25m });
        data.SmokedCigarettes.Add(new SmokedCigarette { SmokedAt = DateTime.Today.AddDays(-1), Price = 1m });
        Assert.Equal(1.55m, data.TotalSmokedValue);
        Assert.Equal(0.55m, data.TodaySmokedValue);

        Assert.Equal(0m, new SmokingData { CigarettesPerPack = 0 }.PricePerCigarette);
    }

    [Fact]
    public void ValoresPorDefectoDelPlan()
    {
        var data = new SmokingData();
        Assert.Equal((20, 0, "EUR", 5.0m, 20), (data.MaxCigarettesPerDay, data.SmokedToday, data.Currency, data.PackPrice, data.CigarettesPerPack));
        Assert.Equal(DateTime.Today, data.LastResetDate);
        Assert.Equal("EUR", new SmokedCigarette().Currency);
    }

    [Fact]
    public void RegistroDiario()
    {
        var day = new DailySmokingRecord { Count = 2 };
        day.SmokedCigarettes.Add(new SmokedCigarette { Price = 0.2m });
        day.SmokedCigarettes.Add(new SmokedCigarette { Price = 0.3m });
        Assert.Equal(0.5m, day.TotalSmokedValue);
        Assert.Equal(0.25m, day.AveragePricePerCigarette);
        Assert.Equal(0m, new DailySmokingRecord().AveragePricePerCigarette);
    }

    [Fact]
    public void BeneficioConSuIcono()
    {
        Assert.Equal("ic_tip_motivation.png", new BenefitItem { Achieved = true }.Icon);
        Assert.Equal("ic_alarm.png", new BenefitItem { Achieved = false }.Icon);
    }

    [Fact]
    public void DivisasConNombreTraducidoYSimbolo()
    {
        Preferences.Values["app_language"] = "es";
        try
        {
            var loc = new LocalizationService();
            var list = Currency.GetAvailableCurrencies(loc);
            Assert.Equal(14, list.Count);
            Assert.Equal(list.Count, list.Select(c => c.Code).Distinct().Count());
            Assert.All(list, c => Assert.NotEqual(c.NameKey, c.Name));   // todas tienen texto
            Assert.Equal("Euro", list.Single(c => c.Code == "EUR").Name);

            Assert.All(Currency.GetAvailableCurrencies(), c => Assert.Equal(c.Code, c.Name));   // sin idioma: el codigo
            Assert.Equal("€", Currency.SymbolFor("EUR"));
            Assert.Equal("S/", Currency.SymbolFor("PEN"));
            Assert.Equal("XYZ", Currency.SymbolFor("XYZ"));
        }
        finally
        {
            Preferences.Values.Clear();
        }
    }

    [Theory]
    [InlineData(0, "< 1 min")]
    [InlineData(59, "< 1 min")]
    [InlineData(60, "1 min")]
    [InlineData(59 * 60 + 59, "59 min")]
    [InlineData(3600 + 5 * 60, "1h 5min")]
    [InlineData(23 * 3600 + 59 * 60, "23h 59min")]
    [InlineData(2 * 86400 + 4 * 3600 + 30 * 60, "2d 4h")]
    public void TiempoDesdeElUltimo(int seconds, string expected) =>
        Assert.Equal(expected, TimeFormat.Elapsed(TimeSpan.FromSeconds(seconds)));

    // -----------------------------------------------------------------------
    // Consejos
    // -----------------------------------------------------------------------

    private static string ImagesDir()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(dir!.FullName, "QuitSmoke.csproj")))
            dir = dir.Parent;
        return Path.Combine(dir.FullName, "Resources", "Images");
    }

    [Fact]
    public void ConsejosEnLosDosIdiomasConIconoQueExiste()
    {
        var es = SmokingTips.GetAllTips("es");
        var en = SmokingTips.GetAllTips("en");

        Assert.True(es.Count >= 50);
        Assert.Equal(es.Count, en.Count);
        for (var i = 0; i < es.Count; i++)
        {
            Assert.Equal(es[i].Icon, en[i].Icon);   // el mismo consejo en el mismo orden
            foreach (var tip in new[] { es[i], en[i] })
            {
                Assert.False(string.IsNullOrWhiteSpace(tip.Title));
                Assert.False(string.IsNullOrWhiteSpace(tip.Message));
                Assert.False(string.IsNullOrWhiteSpace(tip.Category));
                var svg = Path.ChangeExtension(tip.Icon, ".svg");
                Assert.True(File.Exists(Path.Combine(ImagesDir(), svg)), $"falta {svg}");
            }
        }

        Assert.Equal(es.Count, es.Select(t => t.Message).Distinct().Count());
        Assert.Equal(en.Count, en.Select(t => t.Message).Distinct().Count());
        Assert.Same(es, SmokingTips.GetAllTips("fr"));   // otro idioma: castellano
        Assert.Same(es, SmokingTips.GetAllTips());
        Assert.Equal("ic_tip_bulb.png", new SmokingTip().Icon);
    }
}
