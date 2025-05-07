using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    public class NavigationPanel : BaseUIPanel
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button mainLobbyButton;
        [SerializeField] private Button characterButton;
        [SerializeField] private Button battleButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button settingsButton;
        
        private void Awake()
        {
            RegisterButtonEvents();
        }

        private void RegisterButtonEvents()
        {
            backButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ClosePanel(PanelName.NavigationPanel);
            });
            mainLobbyButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.MainLobbyPanel, OpenStrategy.CloseAllFirst);
            });
            characterButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.CharacterRosterPanel, OpenStrategy.ReplaceCurrent);
            });
            battleButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.BattlePanel, OpenStrategy.ReplaceCurrent);
            });
            mainMenuButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.MainMenuPanel, OpenStrategy.CloseAllFirst);
            });
            settingsButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.SettingsPanel, OpenStrategy.Additive);
            });
        }
    }
}