using System;
using GameLogic.Skill.Active;
using GameLogic.Unit.BattleRuntimeData;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UISlots
{
    public class BattleSkillSlot : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private Button skillButton;
        [SerializeField] private GameObject cdGroup;
        [SerializeField] private Text remainingCooldownText;
        
        public Action<ActiveSkillInstance> OnSkillButtonClick;
        private ActiveSkillInstance _activeSkillInstance;
        private CharacterBattleRuntimeData _characterRuntimeData;
        private bool _canUseSkill;
        
        private bool CanUseSkill
        {
            get => _canUseSkill;
            set
            {
                _canUseSkill = value;
                skillIcon.color = _canUseSkill ? Color.white : Color.gray;
            }
        }


        private void Awake()
        {
            CanUseSkill = false;
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
            SetRemainingCooldownText(_activeSkillInstance.remainCooldown);
        }
        
        /// <summary>
        /// 根据剩余冷却时间设置文本
        /// </summary>
        /// <param name="remainCooldown"></param>
        private void SetRemainingCooldownText(int remainCooldown)
        {
            if (remainCooldown == 0)
            {
                cdGroup.gameObject.SetActive(false);
                return;
            }
            cdGroup.gameObject.SetActive(true);
            remainingCooldownText.text = remainCooldown.ToString();
        }

        private void RegisterEvent()
        {
            skillButton.onClick.AddListener(OnSKillButtonClick);
            _characterRuntimeData.CanAction.AddListener(OnCanActionChanged);
            _characterRuntimeData.AddListenerOnSkillPoint(OnSkillPointChanged);
            _activeSkillInstance.AddListenerOnCooldownEnd(OnSkillCDChanged);
        }

        private void UnRegisterEvent()
        {
            if (_activeSkillInstance == null) return;
            skillButton.onClick.RemoveListener(OnSKillButtonClick);
            _characterRuntimeData.CanAction.RemoveListener(OnCanActionChanged);
            _characterRuntimeData.RemoveListenerOnSkillPoint(OnSkillPointChanged);
            _activeSkillInstance.RemoveListenerOnCooldownEnd(OnSkillCDChanged);
        }

        private void OnCanActionChanged(bool canAction)
        {
            // Debug.Log($"OnCanActionChanged: {canAction}");
            CheckCondition();
        }
        
        private void OnSkillPointChanged(int remainSkillPoint)
        {
            // Debug.Log($"OnCanActionChanged: {remainCooldown}");
            CheckCondition();
        }
        
        private void OnSkillCDChanged(int remainCooldown)
        {
            // Debug.Log($"OnCanActionChanged: {remainCooldown}");
            SetRemainingCooldownText(remainCooldown);
            CheckCondition();
        }

        private void CheckCondition()
        {
            // Debug.Log($"OnCanActionChanged: {canAction}");
            if (!_characterRuntimeData.CanAction.Value)
            {
                CanUseSkill = false;
                return;
            }
            
            // 技能点不足
            if (_characterRuntimeData.CurSkillPoint < _activeSkillInstance.SKillData.skillPointCost)
            {
                CanUseSkill = false;
                return;
            }
            
            // 技能冷却
            CanUseSkill = _activeSkillInstance.remainCooldown <= 0;
        }

        private void OnSKillButtonClick()
        {
            if (!CanUseSkill)
            {
                // TODO 技能不可用提示
                return;
            }
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            OnSkillButtonClick?.Invoke(_activeSkillInstance);
        }
    }
}