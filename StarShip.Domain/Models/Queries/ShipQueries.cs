using System;
using System.Collections.Generic;
using System.Linq;
using StarShip.Domain.Models.Ships;
using StarShip.Domain.Enums;
using StarShip.Domain.Models.Shared;

namespace StarShip.Domain.Models.Queries;
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


    public static bool AnyInDistress(this IEnumerable<BaseShip> ships)
    {
        ArgumentNullException.ThrowIfNull(ships);
        return ships.Any(s => s.Status == ShipStatus.Distress);
    }

    public static IReadOnlyList<BaseShip> InDistress(this IEnumerable<BaseShip> ships)
    {
        ArgumentNullException.ThrowIfNull(ships);
        return ships.Where(s => s.Status == ShipStatus.Distress).ToList();
    }

    public static BaseShip? Nearest(this IEnumerable<BaseShip> ships, Coordinate origin)
    {
        ArgumentNullException.ThrowIfNull(ships);
        ArgumentNullException.ThrowIfNull(origin);

        return ships.Any() ? ships.MinBy(s => s.Position.DistanceTo(origin)) : null;
    }

    public static BaseShip? Farthest(this IEnumerable<BaseShip> ships, Coordinate origin)
    {
        ArgumentNullException.ThrowIfNull(ships);
        ArgumentNullException.ThrowIfNull(origin);

        return ships.Any() ? ships.MaxBy(s => s.Position.DistanceTo(origin)) : null;
    }

    public static IReadOnlyList<DistressReport> DistressReports(this IEnumerable<BaseShip> ships)
    {
        ArgumentNullException.ThrowIfNull(ships);
        
        return ships
            .Where(s => s.Distress != null)
            .OrderBy(s => s.Distress!.SentAt)
            .Select(s => new DistressReport(s.Name, s.Distress!.Reason, s.Distress!.SentAt))
            .ToList();
    }

    public static IReadOnlyList<BaseShip> WithinRange(this IEnumerable<BaseShip> ships, Coordinate origin, int range)
    {
        ArgumentNullException.ThrowIfNull(ships);
        ArgumentNullException.ThrowIfNull(origin);

        return ships.Where(s => s.Position.DistanceTo(origin) <= range).OrderBy(s => s.Position.DistanceTo(origin))
            .ToList();
    }

    public record YearCount(int Year, int Count)
    {
        public override string ToString() => $"{Year}: {Count}";
        
    }
    
        public static BaseShip? Oldest(this IEnumerable<BaseShip> ships) => ships.MinBy(s => s.BuiltAt);
        public static BaseShip? Newest(this IEnumerable<BaseShip> ships) => ships.MaxBy(s => s.BuiltAt);

        public static BaseShip? WithLargestCrew(this IEnumerable<BaseShip> ships) => ships.MaxBy(s => s.CrewCount);

        public static IReadOnlyList<YearCount> PerBuildYear(this IEnumerable<BaseShip> ships) => ships
            .GroupBy(s => s.BuiltAt.Year)
            .Select(g => new YearCount(g.Key, g.Count()))
            .OrderBy(yc => yc.Year)
            .ToList();
    }
