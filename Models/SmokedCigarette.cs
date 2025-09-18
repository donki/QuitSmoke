namespace QuitSmoke.Models;

public class SmokedCigarette
{
    public DateTime SmokedAt { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";
}