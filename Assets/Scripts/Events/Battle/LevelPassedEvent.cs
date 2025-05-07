using MyFramework.Utilities;

namespace Events.Battle
{
    public class LevelPassedEvent : IEventArgs
    {
        public readonly string LevelID;
        public readonly short Stars;
        
        public LevelPassedEvent(string levelID, short stars)
        {
            LevelID = levelID;
            Stars = stars;
        }
    }
}