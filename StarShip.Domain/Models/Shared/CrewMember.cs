namespace StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;

public record CrewMember(string Name, CrewRole Role)
{
    public static CrewMember Captian(string name) => new(name, CrewRole.Captain);
    public static CrewMember Pilot(string name) => new (name, CrewRole.Pilot);
    public static CrewMember Engineer(string name) => new(name, CrewRole.Engineer);
    public static CrewMember Medic(string name) => new(name, CrewRole.Medic);
}