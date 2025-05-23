using GameLogic.BUFF;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class CharacterBuffAppliedEvent : IEventArgs
    {
        public CharacterUnitController Target { get; }
        public BuffConfig Buff { get; }
        
        public CharacterBuffAppliedEvent(CharacterUnitController target, BuffConfig buff)
        {
            Target = target;
            Buff = buff;
        }
    }
}