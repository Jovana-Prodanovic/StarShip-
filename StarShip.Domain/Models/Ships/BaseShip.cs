using StarShip.Domain.Models.Queries;

namespace StarShip.Domain.Models.Ships;

using System.Linq;
using System.Collections.Generic;
using StarShip.Domain.Models.Entities;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;
using StarShip.Domain.Models.Stations;

public abstract class BaseShip : BaseEntity
{
    public const int MaxFuel = 10;
    public const int MaxCrew = 6;
    public const int WarpRange = 15;
    public const int SensorRange = 20;
    public const int FuelPerWarp = 1;
    
    private readonly List<CrewMember> _crew = [];

    public IReadOnlyDictionary<CrewRole, int> RequiredRoles { get; }
    public int Fuel { get; protected set; } = 0;
    public DistressSignal? Distress { get; protected set; }
    public SpaceStation? Station { get; set; }
    
    public IReadOnlyList<CrewMember> Crew => _crew.AsReadOnly();

    public int CrewCount => _crew.Count;
    public bool IsDocked => Station != null;

    protected BaseShip(string name, Coordinate position, IReadOnlyDictionary<CrewRole, int> requiredRoles) 
        : base(name, position)
    {
        RequiredRoles = requiredRoles ?? throw new ArgumentNullException(nameof(requiredRoles));
    }

    public void AttachTo(SpaceStation station)
    {
        ArgumentNullException.ThrowIfNull(station);
        
        Rules.Require(!IsDocked, "Schiff ist bereits angedockt.");
        Rules.Require(station.DockedShips.Contains(this), "Station listet das Schiff nicht.");

        Station = station;
        Logbook.Add(LogType.Docking, $"An {station.Name} angedockt");
    }

    public void Detach()
    {
        Rules.Require(IsDocked, "Schiff ist nicht angedockt.");
        
        var stationName = Station?.Name ?? "Station";
        Rules.Require(!Station!.DockedShips.Contains(this), "Station listet das Schiff immer noch.");

        Station = null;
        Logbook.Add(LogType.Docking, $"Von {stationName} abgedockt");
    }

    public void AddFuel(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Menge muss größer als 0 sein.");
        }

        Rules.Require(IsDocked, "Schiff ist nicht angedockt.");
        Rules.Require(Fuel + amount <= MaxFuel, "Warpkern würde überlaufen.");

        Fuel += amount;
        Logbook.Add(LogType.Fuel, $"{amount} Antimaterie geladen, Antimaterie {Fuel}/{MaxFuel}");
    }

    public void Board(CrewMember member)
    {
        ArgumentNullException.ThrowIfNull(member);
        Rules.Require(_crew.Count < MaxCrew, "Schiff ist voll.");
        Rules.Require(!_crew.Contains(member), "Person ist bereits an Bord.");

        _crew.Add(member);
        Logbook.Add(LogType.Crew, $"{member} an Bord");
    }

    public void Unboard(CrewMember member)
    {
        ArgumentNullException.ThrowIfNull(member);
        Rules.Require(_crew.Contains(member), "Person ist nicht an Bord.");

        _crew.Remove(member);
        Logbook.Add(LogType.Crew, $"{member} von Bord");
    }

    public void Launch()
    {
        Rules.Require(IsDocked, "Schiff ist nicht angedockt.");
        Rules.Require(HasRequiredRoles, $"Besatzung unvollständig: {string.Join(", ", MissingRoles())}");
        Rules.Require(Fuel == MaxFuel, "Warpkern ist nicht voll.");

        var station = Station ?? throw new InvalidOperationException("Schiff ist an keiner Station.");
        station.Undock(this);
    }

    public bool HasEnoughFuelTo(Coordinate target)
    {
        ArgumentNullException.ThrowIfNull(target);
        double distance = Position.DistanceTo(target);
        int jumpsNeeded = (int)Math.Ceiling(distance / WarpRange);
        return Fuel >= jumpsNeeded * FuelPerWarp;
    }

    public bool CanWarpTo(Coordinate target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return Status == ShipStatus.InSpace && HasRequiredRoles && HasEnoughFuelTo(target);
    }

    public bool CanWarpTo(SpaceStation station)
    {
        ArgumentNullException.ThrowIfNull(station);
        return CanWarpTo(station.Position);
    }

    public void WarpTo(Coordinate target)
    {
        ArgumentNullException.ThrowIfNull(target);
        Rules.Require(CanWarpTo(target), "Warp-Bedingungen nicht erfüllt.");

        double distance = Position.DistanceTo(target);
        Coordinate newPosition;

        if (distance <= WarpRange)
        {
            newPosition = target;
        }
        else
        {
            double ratio = WarpRange / distance;
            int newX = (int)Math.Round(Position.X + (target.X - Position.X) * ratio);
            int newY = (int)Math.Round(Position.Y + (target.Y - Position.Y) * ratio);
            newPosition = new Coordinate(newX, newY);
        }

        Position = newPosition;
        Fuel -= FuelPerWarp;

        Logbook.Add(LogType.Warp, $"Warp-Sprung nach {Position}, Antimaterie {Fuel}/{MaxFuel}");
    }

    public void WarpTo(SpaceStation station)
    {
        ArgumentNullException.ThrowIfNull(station);
        WarpTo(station.Position);
        if (Position.Equals(station.Position))
        {
            station.Dock(this);
        }
    }

    // --- Queries ---
    public bool HasRequiredRoles => RequiredRoles.All(r => _crew.Count(c => c.Role == r.Key) >= r.Value);

    public IReadOnlySet<CrewRole> MissingRoles() =>
        RequiredRoles
            .Where(r => _crew.Count(c => c.Role == r.Key) < r.Value)
            .Select(r => r.Key)
            .ToHashSet();

    public ShipStatus Status => this switch
    {
        { Distress: not null } => ShipStatus.Distress,
        { Station: null } => ShipStatus.InSpace,
        _ => ShipStatus.Docked
    };

    public Readiness Readiness => this switch
    {
        { HasRequiredRoles: false } => Readiness.NeedsCrew,
        { Fuel: < MaxFuel } => Readiness.NeedsFuel,
        _ => Readiness.Ready
    };
    
    public virtual string Report()
    {
        string crewNames = Crew.Any() ? string.Join(", ", Crew) : "-";
        return $"{Name} ({GetType().Name})\n" +
               $"  Status      {Status}\n" +
               $"  Readiness   {Readiness}\n" +
               $"  Position    {Position} Lichtjahre\n" +
               $"  Fuel        {Fuel}/{MaxFuel}\n" +
               $"  Crew        {Crew.Count}/{MaxCrew}: {crewNames}";
    }

    public void SendDistressSignal(string reason)
    {
        Distress = new DistressSignal(reason, DateTime.Now);
        
    }

    public bool IsInSensorRange(BaseShip other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return Position.DistanceTo(other.Position) <= SensorRange;
    }

    public virtual IEnumerable<BaseShip> Scan(IEnumerable<BaseShip> ships)
    {
        ArgumentNullException.ThrowIfNull(ships);
    
        return ships
            .Where(s => s != this && IsInSensorRange(s))
            .WithinRange(Position, SensorRange);
    }
}