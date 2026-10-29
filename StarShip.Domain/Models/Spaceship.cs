namespace StarShip.Domain.Models;

using StarShip.Domain.Enums;
using StarShip.Domain.Models.Shared;

public class Spaceship
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ShipStatus Status { get; set; }
    public Readiness Readiness { get; set; }
    public int FuelLevel { get; private set; }
    public List<CrewMember> Crew { get; } = new();

    public Spaceship(string id, string name)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Status = ShipStatus.InSpace;
        Readiness = Readiness.Ready;
        FuelLevel = 100;
    }

    public void AddCrewMember(CrewMember member)
    {
        ArgumentNullException.ThrowIfNull(member);
        Crew.Add(member);
    }
}

    

