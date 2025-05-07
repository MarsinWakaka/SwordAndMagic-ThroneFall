using Player;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    public class SettingsPanel : BaseUIPanel
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider bgmVolumeSlider;

        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        
        protected void Awake()
        {
            backButton.onClick.AddListener(OnCloseButtonClicked);
            sfxVolumeSlider.value = SoundManager.Instance.SFXVolume;
            bgmVolumeSlider.value = SoundManager.Instance.BGMVolume;
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            
            saveButton.onClick.AddListener(OnSaveButtonClicked);
            loadButton.onClick.AddListener(OnLoadButtonClicked);
        }

        private void OnLoadButtonClicked()
        {
            PlayerDataManager.Instance.LoadPlayerData();
        }

        private void OnSaveButtonClicked()
        {
            PlayerDataManager.Instance.SavePlayerData();
        }

        private void OnSFXVolumeChanged(float arg0)
        {
            SoundManager.Instance.SFXVolume = arg0;
        }

        private void OnBGMVolumeChanged(float arg0)
        {
            SoundManager.Instance.BGMVolume = arg0;
        }

        private void OnCloseButtonClicked()
        {
            // 关闭设置面板
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            UIManager.Instance.ClosePanel(PanelName.SettingsPanel);
        }
    }
}