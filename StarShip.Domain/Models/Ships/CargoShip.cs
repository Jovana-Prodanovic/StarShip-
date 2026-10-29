namespace StarShip.Domain.Models.Ships;

using StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;

public class CargoShip : BaseShip
{
    private static readonly Dictionary<CrewRole, int> CargoRoles = new()
    {
        { CrewRole.Captain, 1 },
        { CrewRole.Pilot, 1 }
    };

    public int MaxCargo { get; }
    public int Cargo { get; set; }

    public bool IsFull => Cargo >= MaxCargo;
    public int CargoPercent => MaxCargo > 0 ? (Cargo * 100) / MaxCargo : 0;

    public CargoShip(string name, Coordinate position, int maxCargo) 
        : base(name, position, CargoRoles)
    {
        MaxCargo = maxCargo;
        Cargo = 0;
    }
}