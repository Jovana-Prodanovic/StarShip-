using Xunit;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Models.Ships;
using StarShip.Domain.Models.Stations;

namespace StarShip.Tests;

public class SpaceStationTests
{
    [Fact]
    public void DockShip_ShouldAddShipToDockedList()
    {
        
        var station = new SpaceStation("Alpha Station", new Coordinate(0, 0), 1000);
        var ship = new CargoShip("USS-Enterprise", new Coordinate(0, 0), 100);

       
        station.DockShip(ship);

       
        Assert.Contains(ship, station.DockedShips);
        Assert.Equal(station, ship.Station);
    }
}