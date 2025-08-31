namespace QuitSmoke.Models;

public class DailySmokingRecord
{
    public DateTime Date { get; set; }
    public List<DateTime> Times { get; set; } = new();
    public int Count { get; set; }
}
