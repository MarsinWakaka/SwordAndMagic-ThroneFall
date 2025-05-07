using System;
using GameLogic.Skill.Active;
using GameLogic.Unit.BattleRuntimeData;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.Battle
{
    public class BattleSkillSlot : MonoBehaviour
    {
        private ActiveSkillInstance _activeSkillInstance;
        private CharacterBattleRuntimeData _characterRuntimeData;
        
        [SerializeField] private Image skillIcon;
        [SerializeField] private Button skillButton;
        
        public Action<ActiveSkillInstance> OnSkillButtonClick;

        private void Awake()
        {
            skillButton.interactable = false;
        }

        private void OnDestroy()
        {
            UnRegisterEvent();
        }

        public void Initialize(ActiveSkillInstance skillIns, CharacterBattleRuntimeData characterRuntimeData)
        {
            UnRegisterEvent();
            _activeSkillInstance = skillIns;
            _characterRuntimeData = characterRuntimeData;
            RegisterEvent();
            SetVisual();
            
            OnCanActionChanged(_characterRuntimeData.CanAction.Value);
        }

        private void SetVisual()
        {
            skillIcon.sprite = _activeSkillInstance.ActiveConfig.skillIcon;
        }

        private void RegisterEvent()
        {
            skillButton.onClick.AddListener(OnSKillButtonClick);
            _characterRuntimeData.CanAction.AddListener(OnCanActionChanged);
        }

        private void UnRegisterEvent()
        {
            if (_activeSkillInstance == null) return;
            skillButton.onClick.RemoveListener(OnSKillButtonClick);
            _characterRuntimeData.CanAction.RemoveListener(OnCanActionChanged);
        }

        private void OnCanActionChanged(bool canAction)
        {
            Debug.Log($"OnCanActionChanged: {canAction}");
            if (!canAction)
            {
                skillButton.interactable = false;
                return;
            }
            skillButton.interactable = _activeSkillInstance.remainCooldown <= 0;
        }

        private void OnSKillButtonClick()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            OnSkillButtonClick?.Invoke(_activeSkillInstance);
        }
    }
}