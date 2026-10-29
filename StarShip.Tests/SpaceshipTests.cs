using StarShip.Domain.Enums;
using StarShip.Domain.Models;
using Xunit;

namespace StarShip.Tests;

public class SpaceshipTests
{
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var ship = new Spaceship("USS-1", "Enterprise");

        // Assert
        Assert.Equal("USS-1", ship.Id);
        Assert.Equal("Enterprise", ship.Name);
        Assert.Equal(ShipStatus.Ready, ship.Status);
        Assert.Equal(100, ship.FuelLevel);
        Assert.Empty(ship.Crew);
    }
}
