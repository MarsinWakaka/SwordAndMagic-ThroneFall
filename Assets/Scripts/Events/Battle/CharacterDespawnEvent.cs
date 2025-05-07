using GameLogic.Unit.Controller;
using MyFramework.Utilities;

namespace Events.Battle
{
    /// <summary>
    /// 角色死亡事件(当某一动作造成角色死亡时触发)
    /// </summary>
    public class CharacterDespawnEvent : IEventArgs
    {
        public CharacterUnitController Unit { get; }
        
        public CharacterDespawnEvent(CharacterUnitController unit)
        {
            Unit = unit;
        }
    }
}