using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    public class MainMenuPanel : BaseUIPanel
    {
        [SerializeField] private Button startButton;
        
        private void Awake()
        {
            startButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.MainLobbyPanel, OpenStrategy.CloseAllFirst);
            });
            SoundManager.Instance.PlayBGM(BGM.MainMenuBGM);
        }
    }
}