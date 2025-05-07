using GameLogic.Battle;
using MyFramework.Utilities;

namespace Events.Battle
{
    public struct SelectDeployUnitEvent : IEventArgs
    {
        public DeployCharacterData SelectedCharacterData;

        public SelectDeployUnitEvent(DeployCharacterData selectedCharacterData)
        {
            SelectedCharacterData = selectedCharacterData;
        }
    }
}