using MyFramework.Utilities;

namespace Events.Battle
{
    public struct DeployCharacterCountChangeEvent : IEventArgs
    {
        public int CurrentCount;

        public DeployCharacterCountChangeEvent(int currentCount)
        {
            CurrentCount = currentCount;
        }
    }
}