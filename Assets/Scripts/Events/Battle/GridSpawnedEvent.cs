using GameLogic.Grid;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class GridSpawnedEvent : IEventArgs
    {
        public GridController Grid { get; }
        
        public GridSpawnedEvent(GridController grid)
        {
            Grid = grid;
        }
    }
}