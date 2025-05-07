using GameLogic.Unit.Controller;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class CharacterSpawnedEvent : IEventArgs
    {
        public CharacterUnitController Unit { get; }
        
        public CharacterSpawnedEvent(CharacterUnitController unit)
        {
            Unit = unit;
        }
    }
}