namespace StarShip.App;

using System;
using System.Collections.Generic;
using System.Linq;
using StarShip.Domain.Models.Ships;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;
using StarShip.Domain.Models.Queries; 
using StarShip.Domain.Models.Stations;
using StarShip.Domain.Models.Entities;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== AKT 1: Die Docks öffnen ===");

        var home = new Coordinate(100, 200);
        var ds9 = new SpaceStation("Deep Space 9", home, 100) { BuiltAt = new DateOnly(2351, 1, 1) };

        ds9.AddCrew(CrewMember.Captain("Kira"));
        ds9.AddCrew(CrewMember.Captain("Worf"));
        ds9.AddCrew(CrewMember.Pilot("Nog"));
        ds9.AddCrew(CrewMember.Pilot("Dax"));
        ds9.AddCrew(CrewMember.Engineer("O'Brien"));
        ds9.AddCrew(CrewMember.Engineer("Rom"));
        ds9.AddCrew(CrewMember.Medic("Bashir"));
        ds9.AddCrew(CrewMember.Medic("Jabara"));
        
        ds9.Supply(40);

        var xhosa = new CargoShip("Xhosa", home, 100) { BuiltAt = new DateOnly(2360, 5, 12) };
        var bokNor = new CargoShip("Bok'Nor", home, 60) { BuiltAt = new DateOnly(2355, 7, 1) };
        var rioGrande = new RescueShip("Rio Grande", home) { BuiltAt = new DateOnly(2369, 11, 20) };
        List<BaseShip> fleet = [xhosa, bokNor, rioGrande];

        ds9.Dock(xhosa);
        ds9.Dock(bokNor);
        ds9.Dock(rioGrande);
        
        ds9.Refuel(xhosa);
        ds9.Refuel(bokNor);
        
        Console.WriteLine($"Piloten im Pool: {ds9.CrewCountByRole(CrewRole.Pilot)}");
        Console.WriteLine($"Crew je Rolle: {string.Join(", ", ds9.CrewCountPerRole())}");

        Console.WriteLine();
        Console.WriteLine(ds9.Report());
        Console.WriteLine();
        Console.WriteLine(xhosa.Report());
        
        Console.WriteLine("\n=== AKT 2: Fleet Queries ===");
        Console.WriteLine($"Gesamtfracht der Flott: {fleet.TotalCargo()} t");
        
        foreach (var fuelInfo in fleet.TotalFuelByType())
        {
            Console.WriteLine($"Schiffstyp: {fuelInfo.Type}, Treibstoff: {fuelInfo.Fuel}");
        }

        // ------ AKT 3 ------
        Console.WriteLine();
        Console.WriteLine("=== AKT 3: Notruf ===");

        xhosa.Board(CrewMember.Captain("Sisko"));
        xhosa.Board(CrewMember.Pilot("Paris"));

        var target = new Coordinate(145, 200);
        xhosa.Launch();
        xhosa.WarpTo(target);
        xhosa.SendDistressSignal("Warpkern überhitzt");

        Console.WriteLine($"Schiff in Not: {fleet.AnyInDistress()}");
        Console.WriteLine($"In Not: {string.Join(", ", fleet.InDistress().Select(s => s.Name))}");
        Console.WriteLine($"Nächstes Schiff zur Station: {fleet.Nearest(home)?.Name}");
        Console.WriteLine($"Notrufe: {string.Join(", ", fleet.DistressReports())}");

        Console.WriteLine("Lagebericht:");
        foreach (var ship in fleet)
            Console.WriteLine($"  {Describe(ship)}");

        
        Console.WriteLine();
        Console.WriteLine("=== AKT 4: Rio Grande fliegt ===");

        var rioGrandeRescue = new RescueShip("Rio Grande Rescue", new Coordinate(0, 0));

        
        var distressedShip1 = new CargoShip("Titan", new Coordinate(10, 5), 100);
        distressedShip1.SendDistressSignal("Engine failure");

        var distressedShip2 = new CargoShip("Nomad", new Coordinate(15, -5), 80);
        distressedShip2.SendDistressSignal("Hull breach");

        var allShips = new List<BaseShip> { rioGrandeRescue, distressedShip1, distressedShip2 };

        Console.WriteLine("--- STARTING SCAN ---");
        var foundShips = rioGrandeRescue.Scan(allShips);

        foreach (var ship in foundShips)
        {
            Console.WriteLine($"Found ship in distress: {ship.Name}");
        
            
            if (rioGrandeRescue.CanRescue(ship, ds9))
            {
                rioGrandeRescue.Rescue(ship, ds9);
                Console.WriteLine($"Successfully rescued: {ship.Name}");
            }
        }

        Console.WriteLine("\n--- LOGBOOK ANALYSIS ---");
        foreach (var summary in rioGrandeRescue.Logbook.CountByType())
        {
            Console.WriteLine(summary);
        }
    }

    static string Describe(BaseShip ship) => ship switch
    {
        CargoShip { Status: ShipStatus.Distress } c => $"{c.Name}: Frachter 80 % beladen, NOTRUF",
        CargoShip { IsFull: true } c => $"{c.Name}: Frachter voll beladen, Docked, NeedsCrew",
        CargoShip c => $"{c.Name}: Frachter 80 % beladen, Docked, NeedsCrew",
        RescueShip { Readiness: Readiness.Ready } r => $"{r.Name}: Rettungsschiff startklar, Docked",
        RescueShip r => $"{r.Name}: Rettungsschiff, Docked, NeedsCrew",
        _ => ship.ToString()
    };
}