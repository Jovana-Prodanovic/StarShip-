namespace StarShip.Domain.Models.Ships;

using System;
using System.Collections.Generic;
using System.Linq;
using StarShip.Domain.Models.Shared;
using StarShip.Domain.Enums;
using StarShip.Domain.Models.Stations;

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

     private string? RescueBlocker(BaseShip target, SpaceStation home)
     {
          ArgumentNullException.ThrowIfNull(target);
          ArgumentNullException.ThrowIfNull(home);

          if (target.Status != ShipStatus.Distress)
               return "Ziel hat keinen Notruf.";
          if (!Position.Equals(target.Position))
               return "Schiffe sind nicht an derselben Position.";
          if (!HasRequiredRoles)
               return "Rettungsschiff hat nicht die erforderliche Besatzung.";
          if (CrewCount + target.CrewCount > MaxCrew)
               return "Nicht genügend Platz für die gesamte Crew.";
          if (!HasEnoughFuelTo(home.Position))
               return "Antimaterie reicht nicht bis zur Heimatstation.";

          return null;
     }

     public bool CanRescue(BaseShip target, SpaceStation home)
     {
          return RescueBlocker(target, home) is null;
     }

     public void Rescue(BaseShip target, SpaceStation home)
     {
          var blocker = RescueBlocker(target, home);
          Rules.Require(blocker is null, blocker ?? "Rettung nicht möglich.");

          int crewCountBefore = target.CrewCount;
          var targetCrew = target.Crew.ToList();

          foreach (var member in targetCrew)
          {
               target.Unboard(member);
               Board(member);
          }

          Logbook.Append(target.Logbook);

          string distressTime = target.Distress?.SentAt.ToString("HH:mm:ss") ?? "unbekannt";
          
          
          target.Logbook.Add(LogType.Distress, $"Notruf aufgehoben (beantwortet von {Name})");
          
          
          
          Logbook.Add(LogType.Rescue, $"Crew von {target.Name} übernommen ({crewCountBefore} Personen), Notruf von {distressTime} beantwortet");
     }
    
     public override IEnumerable<BaseShip> Scan(IEnumerable<BaseShip> ships)
     {
          ArgumentNullException.ThrowIfNull(ships);
          return base.Scan(ships).Where(s => s.Status == ShipStatus.Distress);
     }
}