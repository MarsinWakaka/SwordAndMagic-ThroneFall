using GameLogic.Character.BattleRuntimeData;
using GameLogic.Unit.BattleRuntimeData;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class ActionEndEvent : IEventArgs
    {
        public readonly CharacterBattleRuntimeData CharacterRTData;

        public ActionEndEvent(CharacterBattleRuntimeData characterRTData)
        {
            CharacterRTData = characterRTData;
        }
    }
}