namespace StarShip.Domain.Models.Stations;

using StarShip.Domain.Enums;
using StarShip.Domain.Models.Entities;
using StarShip.Domain.Models.Queries; 
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Models.Ships;

public class SpaceStation : BaseEntity
{
    public const int MaxDocks = 10;
    public int FuelCapacity { get; }
    public int Fuel { get; private set; }
    public int FuelPool { get; private set; }
    
    private readonly List<BaseShip> _dockedShips = [];
    public IReadOnlyList<BaseShip> DockedShips => _dockedShips.AsReadOnly();

    private readonly List<CrewMember> _crewPool = [];
    public IReadOnlyList<CrewMember> CrewPool => _crewPool.AsReadOnly();
    public int CrewCount => _crewPool.Count;

    public SpaceStation(string name, Coordinate position, int fuelCapacity) 
        : base(name, position)
    {
        FuelCapacity = fuelCapacity;
        Fuel = fuelCapacity;
    }

    public void AddCrew(CrewMember member)
    {
        ArgumentNullException.ThrowIfNull(member);
        Rules.Require(!_crewPool.Contains(member), $"{member.Name} ist schon im Pool von {Name}.");
        _crewPool.Add(member);
        Logbook.Add(LogType.Crew, $"{member} im Pool");
    }

    public void Dock(BaseShip ship)
    {
        ArgumentNullException.ThrowIfNull(ship);

        Rules.Require(_dockedShips.Count < MaxDocks, "Kein Dock frei.");
        Rules.Require(!_dockedShips.Contains(ship), "Schiff ist bereits angedockt.");
        Rules.Require(ship.Position.Equals(Position), "Schiff ist nicht an der Position der Station.");
        Rules.Require(!ship.IsDocked, "Schiff ist bereits woanders angedockt.");

        _dockedShips.Add(ship);
        ship.AttachTo(this);

        Logbook.Add(LogType.Docking, $"{ship.Name} angedockt");
    }

    public void Undock(BaseShip ship)
    {
        ArgumentNullException.ThrowIfNull(ship);
        Rules.Require(_dockedShips.Contains(ship), "Schiff ist hier nicht angedockt.");

        _dockedShips.Remove(ship);
        ship.Detach();

        Logbook.Add(LogType.Docking, $"{ship.Name} abgedockt");
    }
    
    public void Supply(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Menge muss größer als 0 sein.");
        }

        FuelPool += amount;

        Logbook.Add(LogType.Fuel, $"{amount} Einheiten Antimaterie geliefert");
    }

    public void Refuel(BaseShip ship)
    {
        ArgumentNullException.ThrowIfNull(ship);

        Rules.Require(_dockedShips.Contains(ship), "Schiff ist hier nicht angedockt.");
        Rules.Require(ship.Fuel < BaseShip.MaxFuel, "Warpkern ist bereits voll.");
        
        int needed = BaseShip.MaxFuel - ship.Fuel;
        Rules.Require(FuelPool >= needed, "FuelPool ist zu klein.");

        FuelPool -= needed;
        ship.AddFuel(needed);

        Logbook.Add(LogType.Fuel, $"{ship.Name} geladen mit {needed}, Pool {FuelPool}");
    }

    // --- Queries ---
    public int ShipCount => _dockedShips.Count;

    public int CrewCountByRole(CrewRole role) => 
        _crewPool.Count(c => c.Role == role);

    public IReadOnlyList<string> CrewNames() => 
        _crewPool.Select(c => c.Name).ToList();

    public IReadOnlyList<RoleCount> CrewCountPerRole() => 
        _crewPool.GroupBy(c => c.Role)
                 .Select(g => new RoleCount(g.Key, g.Count()))
                 .OrderBy(r => r.Role)
                 .ToList();
}