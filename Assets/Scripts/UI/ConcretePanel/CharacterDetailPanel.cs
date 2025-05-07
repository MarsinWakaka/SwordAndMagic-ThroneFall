using Config;
using GameLogic.Unit;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    public class CharacterDetailPanel : MonoBehaviour
    {
        [Header("角色相关")]
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text characterLevelText;
        [SerializeField] private Text characterDescriptionText;
        [SerializeField] private Text characterMaxHpText;
        [SerializeField] private Text characterMaxMoveRangeText;
        [SerializeField] private Text characterInitSkillPointText;
        [SerializeField] private Button upgradeButton;

        [Header("技能相关")]
        [SerializeField] private Text skillNameText;
        [SerializeField] private Text skillLevelText;
        [SerializeField] private Text skillDescriptionText;
        [SerializeField] private Transform skillSlotParent;
        [SerializeField] private SkillSlot skillSlotPrefab;

        
        private CharacterData _curCharacterData;

        private void OnEnable()
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        }
        
        private void OnDisable()
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClick);
            ClearSlots();
        }
        
        private void OnUpgradeButtonClick()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            UIManager.Instance.ShowPanel(PanelName.CharacterUpgradePanel, OpenStrategy.PauseCurrent, _curCharacterData);
        }

        public void SetData(CharacterData characterData)
        {
            if (characterData == null)
            {
                Debug.LogError("CharacterData is null");
                return;
            }
            ClearSkillInfo();
            SetCharacterInfo(characterData);
            SetSkillSlots(characterData);
            _curCharacterData = characterData;
        }

        #region 角色

        private void SetCharacterInfo(CharacterData characterData)
        {
            if (characterData == null)
            {
                Debug.LogError("CharacterData is null");
                return;
            }
            var characterConfig = CharacterConfigManager.Instance.GetConfig(characterData.characterID);
            var characterProperties = characterConfig.GetCharacterProperties(characterData.level);
            characterNameText.text = characterData.characterID;
            characterLevelText.text = $"Lv.{characterData.level}";
            characterDescriptionText.text = characterConfig.description;
            characterMaxHpText.text = $"生命值 : {characterProperties.maxHp}";
            characterMaxMoveRangeText.text = $"移动距离 : {characterProperties.maxMoveRange}";
            characterInitSkillPointText.text = $"初始技能点 : {characterProperties.initSkillPoint}";
        }

        #endregion
        
        private void SetSkillSlots(CharacterData characterData)
        {
            if (characterData == null)
            {
                Debug.LogError("CharacterData is null");
                return;
            }
            
            // 假如当前角色ID与上次角色ID相同，则不需要重新生成技能格子
            if (_curCharacterData != null && _curCharacterData.characterID == characterData.characterID) return;
            
            ClearSlots(); 
            // 生成技能格子
            foreach (var skillData in characterData.activeSkillsData)
            {
                var skillSlot = Instantiate(skillSlotPrefab, skillSlotParent);
                skillSlot.Initialize(skillData);
                skillSlot.OnSkillButtonClick += OnSkillButtonClick;
            }
            
            foreach (var skillData in characterData.passiveSkillsData)
            {
                var skillSlot = Instantiate(skillSlotPrefab, skillSlotParent);
                skillSlot.Initialize(skillData);
                skillSlot.OnSkillButtonClick += OnSkillButtonClick;
            }
        }

        private void ClearSkillInfo()
        {
            skillNameText.text = string.Empty;
            skillLevelText.text = string.Empty;
            skillDescriptionText.text = string.Empty;
        }
        
        private void OnSkillButtonClick(SkillData skillData)
        {
            // 显示技能介绍
            var skillConfig = BaseSkillConfigManager.Instance.GetSkillConfig(skillData.skillID);
            skillNameText.text = skillConfig.skillName;
            skillLevelText.text = $"Lv.{skillData.level}";
            skillDescriptionText.text = skillConfig.GetSkillDescription(skillData.level);
        }
        
        private void ClearSlots()
        {
            foreach (Transform child in skillSlotParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}