using GameLogic.Skill.Active;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class SkillSlotSelectedEvent : IEventArgs
    {
        public ActiveSkillInstance ActiveSkillInstance { get; }

        public SkillSlotSelectedEvent(ActiveSkillInstance activeSkillInstance)
        {
            ActiveSkillInstance = activeSkillInstance;
        }
    }
}