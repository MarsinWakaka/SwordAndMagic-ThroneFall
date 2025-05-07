using GameLogic.Unit;
using MyFramework.Utilities;

namespace Events.Global
{
    public class CharacterUpgradeSuccessEvent : IEventArgs
    {
        public CharacterUpgradeSuccessEvent(CharacterData characterData)
        {
            CharacterData = characterData;
        }

        public CharacterData CharacterData { get; } 
    }
}