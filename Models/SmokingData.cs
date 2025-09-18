namespace QuitSmoke.Models;

public class SmokingData
{
    public int MaxCigarettesPerDay { get; set; } = 20;
    public int SmokedToday { get; set; } = 0;
    public TimeSpan WakeUpTime { get; set; } = new TimeSpan(7, 0, 0);
    public TimeSpan SleepTime { get; set; } = new TimeSpan(23, 0, 0);
    public DateTime LastResetDate { get; set; } = DateTime.Today;
    public List<DateTime> SmokingTimes { get; set; } = new();
    
    // Configuración de precios
    public decimal PackPrice { get; set; } = 5.0m; // Precio de la cajetilla
    public int CigarettesPerPack { get; set; } = 20; // Cigarros por cajetilla
    public string Currency { get; set; } = "EUR"; // Divisa
    
    // Lista de cigarros fumados con precio
    public List<SmokedCigarette> SmokedCigarettes { get; set; } = new();
    
    public int RemainingCigarettes => MaxCigarettesPerDay - SmokedToday;
    
    public TimeSpan AwakeHours => SleepTime > WakeUpTime 
        ? SleepTime - WakeUpTime 
        : TimeSpan.FromHours(24) - (WakeUpTime - SleepTime);
    
    public TimeSpan TimeBetweenCigarettes => MaxCigarettesPerDay > 0 
        ? TimeSpan.FromMinutes(AwakeHours.TotalMinutes / MaxCigarettesPerDay)
        : TimeSpan.Zero;
    
    public DateTime? NextRecommendedTime
    {
        get
        {
            if (RemainingCigarettes <= 0) return null;
            
            var lastSmoke = SmokingTimes.LastOrDefault();
            if (lastSmoke == default) 
            {
                var today = DateTime.Today;
                return today.Add(WakeUpTime);
            }
            
            return lastSmoke.Add(TimeBetweenCigarettes);
        }
    }
    
    // Propiedades calculadas para precios
    public decimal PricePerCigarette => CigarettesPerPack > 0 ? PackPrice / CigarettesPerPack : 0;
    
    public decimal TotalSmokedValue => SmokedCigarettes.Sum(c => c.Price);
    
    public decimal TodaySmokedValue => SmokedCigarettes
        .Where(c => c.SmokedAt.Date == DateTime.Today)
        .Sum(c => c.Price);
    
    public decimal TodaySavedValue => (MaxCigarettesPerDay - SmokedToday) * PricePerCigarette;
}