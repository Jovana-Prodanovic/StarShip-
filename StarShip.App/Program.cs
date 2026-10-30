namespace StarShip.App;

using System;
using System.Collections.Generic;
using StarShip.Domain.Models.Ships;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;
using StarShip.Domain.Queries; 
using StarShip.Domain.Models.Stations;

using System;
using System.Collections.Generic;
using StarShip.Domain.Models.Ships;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;
using StarShip.Domain.Queries; 

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
    }
}

