namespace StarShip.Domain.Models.Shared;

public record DistressReport(string Name, string Reason, DateTime SentAt)
{
    public override string ToString() => $"{Name}: {Reason} ({SentAt:HH:mm:ss})";
}