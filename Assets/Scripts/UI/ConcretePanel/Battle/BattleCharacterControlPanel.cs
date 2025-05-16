using System.Collections.Generic;
using Events.Battle;
using GameLogic.BUFF;
using GameLogic.Skill.Active;
using GameLogic.Unit.BattleRuntimeData;
using MyFramework.Utilities;
using SoundSystem;
using UI.ConcretePanel.SubPanel;
using UI.UISlots;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.Battle
{
    public class CharacterControlPanelParams
    {
        public CharacterBattleRuntimeData CharacterRuntimeData { get; }
        
        public CharacterControlPanelParams(CharacterBattleRuntimeData characterRuntimeData)
        {
            CharacterRuntimeData = characterRuntimeData;
        }
    }
    
    public class BattleCharacterControlPanel : BaseUIPanel
    {
        // 控件
        [SerializeField] private Button endTurnButton;
        
        [Header("角色")]
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text characterFactionText;
        [SerializeField] private Text characterHpText;
        
        [Header("技能")]
        // 技能相关
        [SerializeField] private Transform slotParent;
        [SerializeField] private BattleSkillSlot battleSkillSlotPrefab;
        private readonly List<BattleSkillSlot> _activeSkillSlots = new();

        [Header("BUFF")]
        // BUFF相关
        [SerializeField] private BuffDetailInfoPanel buffDetailInfoPanel;
        [SerializeField] private Transform buffSlotParent;
        [SerializeField] private BuffSlot buffSlotPrefab;
        private readonly List<BuffSlot> _activeBuffSlots = new();
        
        private CharacterControlPanelParams _controlPanelParams;
        private CharacterBattleRuntimeData _runtimeData;
        private List<ActiveSkillInstance> _activeSkills;
        private List<BuffInstance> _buffs;
        
        private void Awake()
        {
            endTurnButton.onClick.AddListener(OnEndTurnButtonClicked);
            buffDetailInfoPanel.gameObject.SetActive(false);
        }

        public override void OnCreate(object data)
        {
            if (data is CharacterControlPanelParams panelParams)
            {
                _controlPanelParams = panelParams;
                ShowOperation(panelParams.CharacterRuntimeData);
            }
            else
            {
                Debug.LogError("Invalid data for BattleCharacterControlPanel");
            }
        }

        public void ShowOperation(CharacterBattleRuntimeData rtData)
        {
            gameObject.SetActive(true);
            
            // 如果角色不能行动，则不能点击结束回合
            endTurnButton.interactable = rtData.CanAction.Value;
            
            _runtimeData = rtData;
            characterNameText.text = _runtimeData.ConfigData.unitName;
            characterFactionText.text = _runtimeData.faction.ToString();
            characterHpText.text = $"{_runtimeData.CurHp.Value}/{_runtimeData.MaxHp.Value}";
            ClearSkillSlots();
            _activeSkills = _runtimeData.ActiveSkills;
            ShowSkills();
            
            // TODO BUFF相关
            ClearBuffSlots();
            _buffs = _runtimeData.BuffManager.GetAllBuffs();
            ShowBuffs();
        }
        
        #region 技能相关
        
        /// <summary>
        /// 显示技能
        /// </summary>
        private void ShowSkills()
        {
            // TODO 显示技能
            foreach (var activeSkill in _activeSkills)
            {
                var skillSlot = Instantiate(battleSkillSlotPrefab, slotParent);
                skillSlot.Initialize(activeSkill, _runtimeData);
                skillSlot.OnSkillButtonClick += HandleSkillButtonClicked;
                _activeSkillSlots.Add(skillSlot);
            }
        }

        /// <summary>
        /// 清除技能槽
        /// </summary>
        private void ClearSkillSlots()
        {
            foreach (var skillSlot in _activeSkillSlots)
            {
                skillSlot.OnSkillButtonClick -= HandleSkillButtonClicked;
                Destroy(skillSlot.gameObject);
            }
            _activeSkillSlots.Clear();
        }
        
        private void HandleSkillButtonClicked(ActiveSkillInstance skill)
        {
            if (skill == null) return;
            EventBus.Channel(Channel.Gameplay).Publish(new SkillSlotSelectedEvent(skill));
        }
        
        #endregion

        #region BUFF相关

        private void ShowBuffs()
        {
            // TODO 显示BUFF
            foreach (var buff in _buffs)
            {
                var buffSlot = Instantiate(buffSlotPrefab, buffSlotParent);
                buffSlot.Initialize(buff);
                buffSlot.OnBuffSlotClicked += HandleBuffButtonClicked;
                _activeBuffSlots.Add(buffSlot);
            }
        }
        
        private void ClearBuffSlots()
        {
            foreach (var buffSlot in _activeBuffSlots)
            {
                buffSlot.OnBuffSlotClicked -= HandleBuffButtonClicked;
                Destroy(buffSlot.gameObject);
            }
            _activeBuffSlots.Clear();
        }
        
        private void HandleBuffButtonClicked(BuffInstance instance)
        {
            // TODO 处理BUFF槽点击事件
            Debug.Log($"Buff {instance.BuffConfig.buffName} clicked");
            // TODO 显示BUFF详细信息
            buffDetailInfoPanel.gameObject.SetActive(true);
            buffDetailInfoPanel.Initialize(instance);
        }

        #endregion
        
        private void OnEndTurnButtonClicked()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            EventBus.Channel(Channel.Gameplay).Publish(new ActionEndEvent(_controlPanelParams.CharacterRuntimeData));
        }
    }
}