using MyFramework.Utilities;

namespace Events.Battle
{
    public class LoadLevelEvent : IEventArgs
    {
        public readonly string LevelID;

        public LoadLevelEvent(string levelID)
        {
            LevelID = levelID;
        }
    }
}