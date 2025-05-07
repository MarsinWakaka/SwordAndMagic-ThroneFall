using System.Collections.Generic;
using Events.Battle;
using GameLogic.Skill.Active;
using GameLogic.Unit.BattleRuntimeData;
using MyFramework.Utilities;
using SoundSystem;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text characterFactionText;
        [SerializeField] private Text characterHpText;
        
        // 技能相关
        [SerializeField] private Transform slotParent;
        [FormerlySerializedAs("skillSlotPrefab")] 
        [SerializeField] private BattleSkillSlot battleSkillSlotPrefab;
        [SerializeField] private List<ActiveSkillInstance> activeSkills;
        [SerializeField] private List<BattleSkillSlot> activeSkillSlots;
        
        private CharacterBattleRuntimeData _runtimeData;
        
        private void Awake()
        {
            endTurnButton.onClick.AddListener(OnEndTurnButtonClicked);
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

        public void ShowOperation(CharacterBattleRuntimeData panelParams)
        {
            gameObject.SetActive(true);
            
            _runtimeData = panelParams;
            characterNameText.text = _runtimeData.ConfigData.unitName;
            characterFactionText.text = _runtimeData.faction.ToString();
            characterHpText.text = $"{_runtimeData.CurHp.Value}/{_runtimeData.MaxHp.Value}";
            ClearSlot();
            activeSkills = _runtimeData.ActiveSkills;
            foreach (var activeSkill in activeSkills)
            {
                var skillSlot = Instantiate(battleSkillSlotPrefab, slotParent);
                skillSlot.Initialize(activeSkill, _runtimeData);
                skillSlot.OnSkillButtonClick += HandleSkillButtonClicked;
                activeSkillSlots.Add(skillSlot);
            }
        }

        private void ClearSlot()
        {
            foreach (var skillSlot in activeSkillSlots)
            {
                skillSlot.OnSkillButtonClick -= HandleSkillButtonClicked;
                Destroy(skillSlot.gameObject);
            }
            activeSkillSlots.Clear();
        }

        private void HandleSkillButtonClicked(ActiveSkillInstance skill)
        {
            if (skill == null) return;
            EventBus.Channel(Channel.Gameplay).Publish(new SkillSlotSelectedEvent(skill));
        }

        public void HideView()
        {
            gameObject.SetActive(false);
        }
        
        private CharacterControlPanelParams _controlPanelParams;
        
        private void OnEndTurnButtonClicked()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            EventBus.Channel(Channel.Gameplay).Publish(new ActionEndEvent(_controlPanelParams.CharacterRuntimeData));
        }
    }
}