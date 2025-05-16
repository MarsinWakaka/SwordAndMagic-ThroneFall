using System;
using Config;
using GameLogic.Unit;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UISlots
{
    public class SkillSlot : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private Button skillButton;
        [SerializeField] private GameObject skillLockMask;
        
        public SkillData skillData;
        public event Action<SkillSlot> OnSkillButtonClick;

        private void Awake()
        {
            skillButton.interactable = false;
            skillLockMask.SetActive(false);
            skillButton.onClick.AddListener(OnSKillButtonClick);
        }

        private void OnDestroy()
        {
            skillButton.onClick.RemoveListener(OnSKillButtonClick);
        }

        public void Initialize(SkillData data)
        {
            skillButton.interactable = true;
            skillData = data;
            RefreshVisual();
        }

        public void RefreshVisual()
        {
            if (skillData == null)
            {
                Debug.LogError("SkillData is null");
                return;
            }
            skillLockMask.SetActive(skillData.level == 0);
            skillIcon.sprite = SkillConfigManager.Instance.GetConfig(skillData.skillID).skillIcon;
        }
        
        private void OnSKillButtonClick()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            OnSkillButtonClick?.Invoke(this);
        }
    }
}