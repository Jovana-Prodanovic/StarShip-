namespace StarShip.Domain.Models.Ships;

using StarShip.Domain.Models.Entities;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;

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
    public object? Station { get; set; } // Biće zamenjeno sa SpaceStation kasnije

    private readonly List<CrewMember> _crew = new();
    public IReadOnlyList<CrewMember> Crew => _crew.AsReadOnly();

    public int CrewCount => _crew.Count;
    public bool IsDocked => Station != null;
    
    public ShipStatus Status => IsDocked ? ShipStatus.Docked : (Distress != null ? ShipStatus.Distress : ShipStatus.InSpace);
    
    public Readiness Readiness => Fuel < FuelPerWarp ? Readiness.NeedsFuel : (!HasRequiredRoles ? Readiness.NeedsCrew : Readiness.Ready);

    public bool HasRequiredRoles
    {
        get
        {
            foreach (var kvp in RequiredRoles)
            {
                int count = _crew.Count(c => c.Role == kvp.Key);
                if (count < kvp.Value) return false;
            }
            return true;
        }
    }

    protected BaseShip(string name, Coordinate position, IReadOnlyDictionary<CrewRole, int> requiredRoles) 
        : base(name, position)
    {
        RequiredRoles = requiredRoles ?? throw new ArgumentNullException(nameof(requiredRoles));
    }
}