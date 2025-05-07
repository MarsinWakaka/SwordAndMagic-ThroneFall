using System;
using Player;
using SoundSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    public class MainLobbyPanel : BaseUIPanel
    {
        [SerializeField] private Button characterButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button navigationButton;
        
        // 显示资源
        [SerializeField] private Text goldText;
        
        private void Awake()
        {
            characterButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.CharacterRosterPanel, OpenStrategy.HideCurrent);
            });
            
            settingsButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.SettingsPanel, OpenStrategy.HideCurrent);
            });
            
            navigationButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.NavigationPanel, OpenStrategy.HideCurrent);
            });
            SoundManager.Instance.PlayBGM(BGM.MainLobbyBGM);
        }

        public void OnDestroy()
        {
            characterButton.onClick.RemoveAllListeners();
            navigationButton.onClick.RemoveAllListeners();
        }
        
        public override void OnCreate(object data)
        {
            base.OnCreate(data);
            PlayerDataManager.Instance.AddListerOnResourcesChanged(UpdateGoldText);
            UpdateGoldText(PlayerDataManager.Instance.Resources);
        }
        
        public override void OnRelease()
        {
            PlayerDataManager.Instance.RemoveListerOnResourcesChanged(UpdateGoldText);
            base.OnRelease();
        }

        private void UpdateGoldText(int newGold)
        {
            goldText.text = newGold.ToString();
        }
    }
}