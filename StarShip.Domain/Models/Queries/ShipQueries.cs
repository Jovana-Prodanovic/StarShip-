namespace StarShip.Domain.Queries;

using System.Collections.Generic;
using System.Linq;
using StarShip.Domain.Models.Ships;
using StarShip.Domain.Enums;

public record FuelByType(string Type, int Fuel);

public static class ShipQueries
{
    public static IEnumerable<BaseShip> GetShipsByStatus(this IEnumerable<BaseShip> ships, ShipStatus status) =>
        ships.Where(s => s.Status == status);

    public static IEnumerable<BaseShip> GetShipsByReadiness(this IEnumerable<BaseShip> ships, Readiness readiness) =>
        ships.Where(s => s.Readiness == readiness);

    public static IEnumerable<BaseShip> GetShipsInDistress(this IEnumerable<BaseShip> ships) =>
        ships.Where(s => s.Status == ShipStatus.Distress);

    public static IReadOnlyList<BaseShip> InSpace(this IEnumerable<BaseShip> ships) =>
        ships.Where(s => s.Status == ShipStatus.InSpace).ToList();

    public static bool AnyInSpace(this IEnumerable<BaseShip> ships) =>
        ships.Any(s => s.Status == ShipStatus.InSpace);

    public static bool AllDocked(this IEnumerable<BaseShip> ships) =>
        ships.All(s => s.Status == ShipStatus.Docked);

    public static IReadOnlyList<string> ReadyToLaunch(this IEnumerable<BaseShip> ships) =>
        ships.Where(s => s.Readiness == Readiness.Ready).Select(s => s.Name).ToList();

    public static int TotalCargo(this IEnumerable<BaseShip> ships) =>
        ships.OfType<CargoShip>().Sum(c => c.Cargo);

    public static IReadOnlyList<FuelByType> TotalFuelByType(this IEnumerable<BaseShip> ships) =>
        ships.GroupBy(s => s.GetType().Name)
            .Select(g => new FuelByType(g.Key, g.Sum(s => s.Fuel)))
            .OrderBy(g => g.Type)
            .ToList();
}