namespace QuitSmoke.Models;

public class BenefitItem
{
    public string Description { get; set; } = string.Empty;
    public bool Achieved { get; set; }
    public string Icon => Achieved ? "✅" : "⏳";
}