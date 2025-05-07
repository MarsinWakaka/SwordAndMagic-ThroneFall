using MyFramework.Utilities;

namespace Events.Battle.Skill
{
    public class SkillTargetSelectedUpdateUIEvent : IEventArgs
    {
        public bool HasTargetSelected { get; set; }
        
        public SkillTargetSelectedUpdateUIEvent(bool hasTargetSelected)
        {
            HasTargetSelected = hasTargetSelected;
        }
    }
}