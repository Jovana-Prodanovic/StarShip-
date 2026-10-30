namespace StarShip.App;

using StarShip.Domain.Enums;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Models.Stations; 
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== STARSHIP SYSTEM START ===");

        
        var station = new SpaceStation("Alpha Station", new Coordinate(0, 0), 1000);
        Console.WriteLine($"Stanica uspešno kreirana: {station.Name}");

        var pilot = new CrewMember("Jovana", CrewRole.Captain);
        station.AddCrew(pilot); 
        Console.WriteLine($"Besatzungsmitglied hinzugefügt: {pilot.Name}");

        Console.WriteLine("=== SYSTEM IST BEREIT ===");
    }
}