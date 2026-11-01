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
        // Akt 1 //
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
        // AKT 2 //
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
        // ------ AKT 4 ------
Console.WriteLine();
Console.WriteLine("=== AKT 4: Rio Grande fliegt ===");


var rescueBase = new SpaceStation(
    "Rettungsbasis",
    new Coordinate(0, 0),
    20);


var rioGrandeRescue = new RescueShip(
    "Rio Grande Rescue",
    rescueBase.Position);

rescueBase.Dock(rioGrandeRescue);
rescueBase.Refuel(rioGrandeRescue);

rioGrandeRescue.Board(CrewMember.Captain("Janeway"));
rioGrandeRescue.Board(CrewMember.Pilot("Tom"));
rioGrandeRescue.Board(CrewMember.Medic("Doctor"));

rioGrandeRescue.Launch();


var distressedShip1 = new CargoShip(
    "Titan",
    new Coordinate(10, 5),
    100);

distressedShip1.Board(CrewMember.Engineer("Torres"));
distressedShip1.SendDistressSignal("Engine failure");

var distressedShip2 = new CargoShip(
    "Nomad",
    new Coordinate(15, -5),
    80);

distressedShip2.Board(CrewMember.Engineer("Scotty"));
distressedShip2.SendDistressSignal("Hull breach");

var allShips = new List<BaseShip>
{
    rioGrandeRescue,
    distressedShip1,
    distressedShip2
};


Console.WriteLine("--- STARTING SCAN ---");

var foundShips = rioGrandeRescue.Scan(allShips).ToList();

Console.WriteLine(
    $"Gefundene Schiffe: {foundShips.Count}");

foreach (var ship in foundShips)
{
    Console.WriteLine(
        $"Gefunden: {ship.Name}, Position: {ship.Position}");

   
    while (!rioGrandeRescue.Position.Equals(ship.Position)
           && rioGrandeRescue.CanWarpTo(ship.Position))
    {
        rioGrandeRescue.WarpTo(ship.Position);

        Console.WriteLine(
            $"Rettungsschiff erreicht: {rioGrandeRescue.Position}");
    }

   
    if (rioGrandeRescue.CanRescue(ship, rescueBase))
    {
        rioGrandeRescue.Rescue(ship, rescueBase);

        Console.WriteLine(
            $"Erfolgreich gerettet: {ship.Name}");

        Console.WriteLine(
            $"Crew an Bord: {rioGrandeRescue.CrewCount}");
    }
    else
    {
        Console.WriteLine(
            $"Rettung von {ship.Name} nicht möglich.");

        Console.WriteLine(
            $"Position Rettungsschiff: {rioGrandeRescue.Position}");

        Console.WriteLine(
            $"Position Zielschiff: {ship.Position}");

        Console.WriteLine(
            $"Treibstoff: {rioGrandeRescue.Fuel}/{BaseShip.MaxFuel}");
    }
}


while (!rioGrandeRescue.IsDocked
       && rioGrandeRescue.CanWarpTo(rescueBase))
{
    rioGrandeRescue.WarpTo(rescueBase);

    Console.WriteLine(
        $"Rückflug: {rioGrandeRescue.Position}");
}

if (rioGrandeRescue.IsDocked)
{
    rioGrandeRescue.UnassignCrew();
    rescueBase.Refuel(rioGrandeRescue);

    Console.WriteLine(
        $"Rettungsmission beendet: {rescueBase.Name}");
}
else
{
    Console.WriteLine(
        "Rückkehr zur Rettungsbasis nicht abgeschlossen.");
}


Console.WriteLine("\n--- LOGBOOK ANALYSIS ---");

foreach (var summary in rioGrandeRescue.Logbook.CountByType())
{
    Console.WriteLine(summary);
}
        // ------ AKT 5 ------//
        Console.WriteLine();
        Console.WriteLine("=== AKT 5: Heimkehr und Bilanz ===");

        
        Console.WriteLine(
            $"{rioGrande.Name} ist bereits an {ds9.Name} angedockt.");

       
        Console.WriteLine($"Ältestes Schiff: {fleet.Oldest()?.Name}");
        Console.WriteLine($"Größte Crew: {fleet.WithLargestCrew()?.Name}");
        Console.WriteLine($"Schiffe je jahr: {string.Join(", ", fleet.PerBuildYear())}");

      
        rioGrande.UnassignCrew();
        ds9.Refuel(rioGrande);

       
        Console.WriteLine($"Alle Schiffe bereit: {ds9.AllShipsReady()}");
        Console.WriteLine($"Schiffe je Typ: {string.Join(", ", ds9.ShipCountByType())}");
        Console.WriteLine($"Crew je Rolle: {string.Join(", ", ds9.CrewCountPerRole())}");

        
        var firstEntry = rioGrandeRescue.Logbook.Entries.FirstOrDefault();

        Console.WriteLine(
            $"Erster Eintrag: {firstEntry?.Message ?? "Keine Einträge"}");

        var rescueEntries = rioGrandeRescue.Logbook.Entries
            .Where(entry => entry.Type == LogType.Rescue)
            .ToList();

        if (rescueEntries.Count == 0)
        {
            Console.WriteLine("Rettung: Keine Rettungen protokolliert.");
        }
        else
        {
            foreach (var entry in rescueEntries)
            {
                Console.WriteLine($"Rettung: {entry.Message}");
            }
        }


        Console.WriteLine();
        Console.WriteLine(rioGrandeRescue.Logbook.Report());

        Console.WriteLine();
        Console.WriteLine(ds9.Report());
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
