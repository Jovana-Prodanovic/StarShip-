using StarShip.Domain.Models;

namespace StarShip.App;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== STARSHIP SYSTEM INITIALIZATION ===");

        var myShip = new Spaceship("USS-2026", "Enterprise-Prodanovic");
        Console.WriteLine($"Schiff erfolgreich erstellt: {myShip.Name} (ID: {myShip.Id})");
        Console.WriteLine($"Aktueller Status: {myShip.Status}, Treibstoff: {myShip.FuelLevel}%");

        var pilot = new CrewMember("Jovana", "Captain");
        myShip.AddCrewMember(pilot);
        Console.WriteLine($"Besatzungsmitglied hinzugefügt: {pilot.Name} als {pilot.Role}");

        myShip.Refuel(20);
        Console.WriteLine($"Treibstoffstand nach der Aktion: {myShip.FuelLevel}%");

        Console.WriteLine("=== SYSTEM IST BEREIT FÜR DIE PRÄSENTATION ===");
    }
}