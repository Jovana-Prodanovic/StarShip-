
namespace StarShip.Domain.Models.Queries;

using StarShip.Domain.Enums; 

public record RoleCount(CrewRole Role, int Count)
{
    public override string ToString() => $"{Role}: {Count}";
}