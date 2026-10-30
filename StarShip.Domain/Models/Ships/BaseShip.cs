namespace StarShip.Domain.Models.Ships;

using System.Linq; // LINQ metode (.All, .Where, .Select, .ToHashSet)
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

    public IReadOnlyDictionary<CrewRole, int> RequiredRoles { get; }
    public int Fuel { get; protected set; } = MaxFuel;
    public DistressSignal? Distress { get; set; }
    public SpaceStation? Station { get; set; }

    private readonly List<CrewMember> _crew = new();
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
}