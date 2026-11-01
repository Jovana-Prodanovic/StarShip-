using StarShip.Domain.Models;

namespace StarShip.Domain.Logbooks;

using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public void Add(LogType type, string message)
    {
        _entries.Add(new LogEntry(DateTime.Now, type, Owner.Name, message));
    }

    public IReadOnlyList<LogEntry> ByType(LogType type)
    {
        return _entries.Where(e => e.Type == type).ToList().AsReadOnly();
    }

    public LogEntry? LastOf(LogType type)
    {
        return _entries.LastOrDefault(e => e.Type == type);
    }

    public override string ToString()
    {
        return $"Logbook for {Owner.Name} with {Count} entries.";
    }

    public void Append(Logbook other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Rules.Require(other != this, "Ein Logbuck kann nicht an sich selbest angehängt werden");

        foreach (var entry in other.Entries)
        {
            _entries.Add(entry);
        }
    }

    public record LogTypeCount(LogType Type, int Count)
    {
        public override string ToString() => $"{Type}: {Count}";
    }

    public IReadOnlyList<LogTypeCount> CountByType()
    {
        return _entries
            .GroupBy(e => e.Type)
            .Select(g => new LogTypeCount(g.Key, g.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();
    }

    public string Format(LogEntry entry)
    {
        string label = entry.Type == LogType.Distress ? "[!!]" : $"[{entry.Type}]";
        return $"{label,-10}{entry.Actor,-12}{entry.Message}";
    }

    public string Report()
    {
        return string.Join(Environment.NewLine, Entries.Select(Format));
    }
}