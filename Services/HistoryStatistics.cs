using QuitSmoke.Models;

namespace QuitSmoke.Services;

/// <summary>
/// Las cuentas del Histórico: consumo, reducción frente al plan y dinero gastado y ahorrado.
/// </summary>
/// <remarks>
/// Estaban dentro de <c>HistoryPage</c>, mezcladas con las etiquetas; aquí se pueden probar. La
/// página solo las pinta.
/// </remarks>
public sealed record HistoryStatistics(
    int TotalCigarettes,
    int DaysWithCigarettes,
    double AveragePerDay,
    int TotalPlanned,
    double ReductionPercentage,
    decimal TotalSpent,
    decimal TotalSaved,
    double AverageSpentPerDay,
    double SavingsPercentage)
{
    public static HistoryStatistics Calculate(IReadOnlyList<DailySmokingRecord> history, SmokingData data)
    {
        // Consumo
        var totalCigarettes = history.SelectMany(h => h.Times).Count();
        var daysWithCigarettes = history.Count(h => h.Count > 0);
        var averagePerDay = daysWithCigarettes > 0 ? (double)totalCigarettes / daysWithCigarettes : 0;

        // Reducción: lo fumado frente al máximo del plan en esos días.
        var totalPlanned = data.MaxCigarettesPerDay * daysWithCigarettes;
        var reductionPercentage = totalPlanned > 0 ? (1.0 - ((double)totalCigarettes / totalPlanned)) * 100 : 0;

        // Gastado: con el precio de cada cigarro si se guardó, o con el precio actual si no.
        var totalSpent = 0m;
        var totalCigarettesSmoked = 0;
        foreach (var day in history)
        {
            if (day.SmokedCigarettes?.Any() == true)
            {
                totalSpent += day.SmokedCigarettes.Sum(c => c.Price);
                totalCigarettesSmoked += day.SmokedCigarettes.Count;
            }
            else if (day.Count > 0)
            {
                totalSpent += day.Count * data.PricePerCigarette;
                totalCigarettesSmoked += day.Count;
            }
        }

        // Ahorrado: (teórico fumado - real fumado) * precio del cigarro.
        var theoreticalCigarettes = daysWithCigarettes * data.MaxCigarettesPerDay;
        var cigarettesSaved = Math.Max(0, theoreticalCigarettes - totalCigarettesSmoked);
        var totalSaved = cigarettesSaved * data.PricePerCigarette;

        var averageCigarettesPerDay = daysWithCigarettes > 0 ? (double)totalCigarettesSmoked / daysWithCigarettes : 0;
        var averageSpentPerDay = averageCigarettesPerDay * (double)data.PricePerCigarette;
        var savingsPercentage = totalSpent > 0 ? ((double)totalSaved / (double)totalSpent) * 100 : 0;

        return new HistoryStatistics(
            totalCigarettes,
            daysWithCigarettes,
            averagePerDay,
            totalPlanned,
            reductionPercentage,
            totalSpent,
            totalSaved,
            averageSpentPerDay,
            savingsPercentage);
    }
}
