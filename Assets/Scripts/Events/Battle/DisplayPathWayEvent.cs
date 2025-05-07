using GameLogic.Grid.Path;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class DisplayPathWayEvent : IEventArgs
    {
        public readonly PathTreeNode PathTreeNode;

        public DisplayPathWayEvent(PathTreeNode pathTreeNode)
        {
            PathTreeNode = pathTreeNode;
        }
    }
}
