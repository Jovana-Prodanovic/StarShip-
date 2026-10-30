namespace StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;

public record LogEntry(DateTime At, LogType Type, string Actor, string Message)
{
    public override string ToString() => $"[{At:yyyy-MM-dd HH:mm:ss}] [{Type}] {Actor}: {Message}";
}

