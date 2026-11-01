using System;
using System.Collections.Generic;
using System.Linq;
using StarShip.Domain.Enums;
using StarShip.Domain.Models.Ships;
using StarShip.Domain.Models.Shared;

namespace StarShip.Domain.Models.Stations;

// Ergebnis der Gruppierung nach Crew-Rolle
public record RoleCount(CrewRole Role, int Count)
{
    public override string ToString() => $"{Role}: {Count}";
}

// Ergebnis der Gruppierung nach Schiffstyp
public record TypeCount(string Type, int Count)
{
    public override string ToString() => $"{Type}: {Count}";
}

public class SpaceStation
{
    private readonly List<BaseShip> _dockedShips = new();
    private readonly List<CrewMember> _crewPool = new();

    public string Name { get; }
    public Coordinate Position { get; }
    public int FuelPool { get; private set; }

    // Datum der Errichtung
    public DateOnly BuiltAt { get; init; }

    public IReadOnlyList<BaseShip> DockedShips =>
        _dockedShips.AsReadOnly();

    public IReadOnlyList<CrewMember> CrewPool =>
        _crewPool.AsReadOnly();

    public int ShipCount => _dockedShips.Count;

    public SpaceStation(
        string name,
        Coordinate position,
        int fuelPool)
    {
        Name = name;
        Position = position;
        FuelPool = fuelPool;
    }

    // --------------------------------------------------
    // Schiffe andocken und abdocken
    // --------------------------------------------------

    public void Dock(BaseShip ship)
    {
        ArgumentNullException.ThrowIfNull(ship);

        _dockedShips.Add(ship);
        ship.AttachTo(this);
    }

    public void Undock(BaseShip ship)
    {
        ArgumentNullException.ThrowIfNull(ship);

        _dockedShips.Remove(ship);
        ship.Detach();
    }

    // --------------------------------------------------
    // Treibstoffversorgung
    // --------------------------------------------------

    public void Supply(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Die Liefermenge muss größer als 0 sein.");
        }

        FuelPool = checked(FuelPool + amount);
    }

    public void Refuel(BaseShip ship)
    {
        ArgumentNullException.ThrowIfNull(ship);

        int needed = BaseShip.MaxFuel - ship.Fuel;
        int amountToLoad = Math.Min(needed, FuelPool);

        // Kein Betanken erforderlich oder kein Vorrat vorhanden
        if (amountToLoad <= 0)
        {
            return;
        }

        // Vorrat erst nach erfolgreichem Betanken reduzieren
        ship.AddFuel(amountToLoad);
        FuelPool -= amountToLoad;
    }

    // --------------------------------------------------
    // Crew-Verwaltung
    // --------------------------------------------------

    public void AddCrew(CrewMember crew)
    {
        ArgumentNullException.ThrowIfNull(crew);

        _crewPool.Add(crew);
    }

    public void RemoveCrew(CrewMember crew)
    {
        ArgumentNullException.ThrowIfNull(crew);

        _crewPool.Remove(crew);
    }

    // --------------------------------------------------
    // Crew-Abfragen
    // --------------------------------------------------

    public int CrewCountByRole(CrewRole role) =>
        _crewPool.Count(crew => crew.Role == role);

    public IReadOnlyList<string> CrewNames() =>
        _crewPool
            .Select(crew => crew.Name)
            .ToList();

    public IReadOnlyList<RoleCount> CrewCountPerRole() =>
        _crewPool
            .GroupBy(crew => crew.Role)
            .Select(group => new RoleCount(
                group.Key,
                group.Count()))
            .OrderBy(result => result.Role)
            .ToList();

    // --------------------------------------------------
    // Schiffs-Abfragen
    // --------------------------------------------------

    public bool AllShipsReady() =>
        _dockedShips.All(
            ship => ship.Readiness == Readiness.Ready);

    public bool AnyShipReady() =>
        _dockedShips.Any(
            ship => ship.Readiness == Readiness.Ready);

    public IReadOnlyList<TypeCount> ShipCountByType() =>
        _dockedShips
            .GroupBy(ship => ship.GetType().Name)
            .Select(group => new TypeCount(
                group.Key,
                group.Count()))
            .ToList();

    // --------------------------------------------------
    // Stationsbericht
    // --------------------------------------------------

    public string Report()
    {
        string builtAtText = BuiltAt == default
            ? "Nicht angegeben"
            : BuiltAt.ToString("dd.MM.yyyy");

        var lines = new List<string>
        {
            $"=== Stationsbericht: {Name} ===",
            $"Errichtet am: {builtAtText}",
            $"Position: {Position}",
            $"Treibstoffvorrat: {FuelPool}",
            $"Angedockte Schiffe: {ShipCount}",
            $"Crew im Pool: {_crewPool.Count}",
            $"Alle angedockten Schiffe bereit: {AllShipsReady()}",
            "",
            "Angedockte Schiffe:"
        };

        if (_dockedShips.Count == 0)
        {
            lines.Add("  Keine Schiffe angedockt.");
        }
        else
        {
            foreach (var ship in _dockedShips)
            {
                lines.Add(
                    $"  {ship.Name} " +
                    $"({ship.GetType().Name}) - " +
                    $"Treibstoff: {ship.Fuel}, " +
                    $"Bereitschaft: {ship.Readiness}");
            }
        }

        lines.Add("");
        lines.Add("Crew je Rolle:");

        if (_crewPool.Count == 0)
        {
            lines.Add("  Keine Crew im Pool.");
        }
        else
        {
            foreach (var roleCount in CrewCountPerRole())
            {
                lines.Add($"  {roleCount}");
            }
        }

        lines.Add("");
        lines.Add("Schiffe je Typ:");

        foreach (var typeCount in ShipCountByType())
        {
            lines.Add($"  {typeCount}");
        }

        return string.Join(Environment.NewLine, lines);
    }
}