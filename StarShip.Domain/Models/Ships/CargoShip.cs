namespace StarShip.Domain.Models.Ships;

using StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;

public class CargoShip : BaseShip
{
    private static readonly Dictionary<CrewRole, int> CargoRoles = new()
    {
        { CrewRole.Captain, 1 },
        { CrewRole.Pilot, 1 }
    };

    public int MaxCargo { get; }
    public int Cargo { get; private set; }

    public bool IsFull => Cargo >= MaxCargo;
    public int CargoPercent => MaxCargo > 0 ? (Cargo * 100) / MaxCargo : 0;

    public CargoShip(string name, Coordinate position, int maxCargo) 
        : base(name, position, CargoRoles)
    {
        MaxCargo = maxCargo;
        Cargo = 0;
    }

    public void Load(int tons)
    {
        if (tons <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tons), "Menge muss größer als 0 sein.");
        }
        Rules.Require(IsDocked, "Schiff ist nicht angedockt.");
        Rules.Require(Cargo + tons <= MaxCargo, "Ladekapazität überschritten.");

        Cargo += tons;
        Logbook.Add(LogType.Cargo, $"{tons} t geladen, {Cargo}/{MaxCargo}");
    }

    public void Unload(int tons)
    {
        if (tons <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tons), "Menge muss größer als 0 sein.");
        }
        Rules.Require(IsDocked, "Schiff ist nicht angedockt.");
        Rules.Require(Cargo - tons >= 0, "Nicht genug Fracht zum Entladen vorhanden.");

        Cargo -= tons;
        Logbook.Add(LogType.Cargo, $"{tons} t entladen, {Cargo}/{MaxCargo}");
    }

    public override string Report()
    {
        return base.Report() + $"\n  Cargo       {Cargo}/{MaxCargo} ({CargoPercent}%)";
    }
}