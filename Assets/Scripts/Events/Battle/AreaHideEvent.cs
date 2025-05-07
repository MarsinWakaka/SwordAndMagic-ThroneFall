using GameLogic.Grid.Area;
using MyFramework.Utilities;

namespace Events.Battle
{
    public struct AreaHideEvent : IEventArgs
    {
        public AreaType AreaType { get; }
        
        public AreaHideEvent(AreaType areaType)
        {
            AreaType = areaType;
        }
    }
}