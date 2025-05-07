using Events.Battle;
using MyFramework.Utilities;
using Scene;
using SoundSystem;
using UI;
using UnityEngine.SceneManagement;

namespace Core.SceneSystem
{
    public class BattleScene : BaseScene
    {
        private static string SceneName => "BattleScene";

        private readonly string _levelID;
        
        public BattleScene(string levelID)
        {
            _levelID = levelID;
        }
        
        // public override string SceneName => 
        public override void OnEnter()
        {
            var asyncHandler = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            if (asyncHandler != null)
            {
                asyncHandler.completed += operation =>
                {
                    // SoundManager.Instance.PlayBGM(BGM.BattleBGM);
                    UIManager.Instance.ShowPanel(PanelName.BattlePanel, OpenStrategy.CloseAllFirst);
                    EventBus.Channel(Channel.Gameplay).Publish(new LoadLevelEvent(_levelID));
                };
            }
        }

        public override void OnExit()
        {
            
        }
    }
}