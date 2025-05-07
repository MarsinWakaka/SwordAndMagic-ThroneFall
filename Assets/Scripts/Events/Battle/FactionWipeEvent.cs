using GameLogic.Unit;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class FactionWipeEvent : IEventArgs
    {
        public readonly Faction Faction;
        public FactionWipeEvent(Faction faction)
        {
            Faction = faction;
        }
    }
}