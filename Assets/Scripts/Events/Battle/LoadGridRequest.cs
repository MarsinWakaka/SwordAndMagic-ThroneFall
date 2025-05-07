using System.Collections.Generic;
using GameLogic.Map;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class LoadGridRequest : IEventArgs
    {
        public List<SpawnGridData> Records { get; }
        
        public LoadGridRequest(List<SpawnGridData> records)
        {
            Records = records;
        }
    }
}