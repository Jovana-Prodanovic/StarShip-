namespace StarShip.Domain.Models.Stations;

using StarShip.Domain.Models.Entities;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Models.Ships;

public class SpaceStation : BaseEntity
{
    public int FuelCapacity { get; }
    public int Fuel { get; private set; }
    
    private readonly List<BaseShip> _dockedShips = new();
    public IReadOnlyList<BaseShip> DockedShips => _dockedShips.AsReadOnly();

    public SpaceStation(string name, Coordinate position, int fuelCapacity) 
        : base(name, position)
    {
        FuelCapacity = fuelCapacity;
        Fuel = fuelCapacity;
    }

    public void DockShip(BaseShip ship)
    {
        ArgumentNullException.ThrowIfNull(ship);
        if (!_dockedShips.Contains(ship))
        {
            _dockedShips.Add(ship);
            ship.Station = this;
        }
    }

    public void UndockShip(BaseShip ship)
    {
        ArgumentNullException.ThrowIfNull(ship);
        if (_dockedShips.Contains(ship))
        {
            _dockedShips.Remove(ship);
            ship.Station = null;
        }
    }
}