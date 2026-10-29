namespace StarShip.Domain.Models.Queries;

using StarShip.Domain.Enums;
using StarShip.Domain.Models.Ships;

public static class ShipQueries
{
    public static IEnumerable<BaseShip> GetShipsByStatus(IEnumerable<BaseShip> ships, ShipStatus status)
    {
        return ships.Where(s => s.Status == status);
    }

    public static IEnumerable<BaseShip> GetShipsByReadiness(IEnumerable<BaseShip> ships, Readiness readiness)
    {
        return ships.Where(s => s.Readiness == readiness);
    }

    public static IEnumerable<BaseShip> GetShipsInDistress(IEnumerable<BaseShip> ships)
    {
        return ships.Where(s => s.Status == ShipStatus.Distress);
    }
}