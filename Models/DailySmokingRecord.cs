namespace QuitSmoke.Models;

public class DailySmokingRecord
{
    public DateTime Date { get; set; }
    public List<DateTime> Times { get; set; } = new();
    public int Count { get; set; }
    public List<SmokedCigarette> SmokedCigarettes { get; set; } = new();
    
    // Propiedades calculadas
    public decimal TotalSmokedValue => SmokedCigarettes.Sum(c => c.Price);
    public decimal AveragePricePerCigarette => Count > 0 ? TotalSmokedValue / Count : 0;
}
