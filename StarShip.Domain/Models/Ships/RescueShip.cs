namespace StarShip.Domain.Models.Ships;
using StarShip. Domain.Models.Shared;
using StarShip.Domain.Enums;

public class RescueShip : BaseShip
{
     private static readonly Dictionary<CrewRole, int> RescueRoles = new()
     {
          { CrewRole.Captain, 1 },
          { CrewRole.Pilot, 1 },
          { CrewRole.Medic, 1 }
     };

     public RescueShip(string name, Coordinate position)
          : base(name, position, RescueRoles)
     {
          
     }

     
     
}