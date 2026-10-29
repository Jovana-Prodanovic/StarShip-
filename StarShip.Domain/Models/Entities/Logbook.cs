namespace StarShip.Domain.Logbooks;

using StarShip.Domain.Models.Entities;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;

public class Logbook
{
    private readonly List<LogEntry> _entries = new();

    public BaseEntity Owner { get; }
    public IReadOnlyList<LogEntry> Entries => _entries.AsReadOnly();

    public int Count => _entries.Count;
    public LogEntry? First => _entries.FirstOrDefault();
    public LogEntry? Last => _entries.LastOrDefault();

    public Logbook(BaseEntity owner)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    public void AddEntry(LogType type, string actor, string message)
    {
        _entries.Add(new LogEntry(DateTime.Now, type, actor, message));
    }

    public IReadOnlyList<LogEntry> ByType(LogType type)
    {
        return _entries.Where(e => e.Type == type).ToList().AsReadOnly();
    }

    public LogEntry? LastOf(LogType type)
    {
        return _entries.LastOrDefault(e => e.Type == type);
    }
}