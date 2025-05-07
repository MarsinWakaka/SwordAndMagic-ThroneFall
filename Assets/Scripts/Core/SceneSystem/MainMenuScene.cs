using Scene;
using SoundSystem;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.SceneSystem
{
    public class MainMenuScene : BaseScene
    {
        private static string SceneName => "MainMenuScene";

        
        public MainMenuScene()
        {
            
        }
        
        // public override string SceneName => 
        public override void OnEnter()
        {
            var asyncHandler = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            if (asyncHandler != null)
            {
                asyncHandler.completed += operation =>
                {
                    // SoundManager.Instance.PlayBGM(BGM.MainLobbyBGM);
                    UIManager.Instance.ShowPanel(PanelName.MainLobbyPanel, OpenStrategy.CloseAllFirst);
                };
            }
        }

        public override void OnExit()
        {
            
        }
    }
}