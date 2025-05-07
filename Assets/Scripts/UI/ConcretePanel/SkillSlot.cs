using System;
using Config;
using GameLogic.Unit;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    public class SkillSlot : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private Button skillButton;
        
        private SkillData _skillData;
        public event Action<SkillData> OnSkillButtonClick;

        private void Awake()
        {
            skillButton.interactable = false;
        }

        private void OnDestroy()
        {
            UnRegisterEvent();
        }

        public void Initialize(SkillData skillData)
        {
            skillButton.interactable = true;
            UnRegisterEvent();
            _skillData = skillData;
            RegisterEvent();
            SetVisual();
        }

        private void SetVisual()
        {
            skillIcon.sprite = BaseSkillConfigManager.Instance.GetSkillConfig(_skillData.skillID).skillIcon;
        }

        private void RegisterEvent()
        {
            skillButton.onClick.AddListener(OnSKillButtonClick);
        }

        private void UnRegisterEvent()
        {
            skillButton.onClick.RemoveListener(OnSKillButtonClick);
        }

        private void OnSKillButtonClick()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            OnSkillButtonClick?.Invoke(_skillData);
        }
    }
}