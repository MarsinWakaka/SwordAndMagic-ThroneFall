using System.Collections.Generic;
using Config;
using GameLogic.Unit;
using Player;
using SoundSystem;
using UI.UISlots;
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
        [SerializeField] private GameObject skillLockGroup; // 技能锁定提示
        [SerializeField] private Button skillUnlockButton; // 技能解锁按钮
        [SerializeField] private Text skillUnlockCostText; // 技能解锁所需资源
        
        private CharacterData _curCharacterData;
        private SkillSlot _curSkillSlot; 
        private Dictionary<string, int> _skillLevelMap;

        private void OnEnable()
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
            skillUnlockButton.onClick.AddListener(OnSkillUnlockButtonClicked);
        }
        
        private void OnDisable()
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClick);
            skillUnlockButton.onClick.RemoveListener(OnSkillUnlockButtonClicked);
            ClearSlots();
        }
        
        private void OnUpgradeButtonClick()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            UIManager.Instance.ShowPanel(PanelName.CharacterUpgradePanel, OpenStrategy.PauseCurrent, _curCharacterData);
        }

        private void ClearSkillInfo()
        {
            skillNameText.text = string.Empty;
            skillLevelText.text = string.Empty;
            skillDescriptionText.text = string.Empty;
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

        /// <summary>
        /// 显示角色的数据
        /// </summary>
        /// <param name="characterData"></param>
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
        
        /// <summary>
        /// 设置角色的技能格子
        /// </summary>
        /// <param name="characterData"></param>
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
            
            // 将数据替换为配置表的数据
            var characterConfig = CharacterConfigManager.Instance.GetConfig(characterData.characterID);
            if (characterConfig == null)
            {
                Debug.LogError($"Character config not found for ID: {characterData.characterID}");
                return;
            }
            _skillLevelMap = characterData.GetSkillLevelMap();
            
            // 生成技能格子
            var skillCount = characterConfig.skillIDs.Count;
            for(var i = 0; i < skillCount; i++)
            {
                var skillSlot = Instantiate(skillSlotPrefab, skillSlotParent);
                // 查询玩家是否拥有该技能
                var skillID = characterConfig.skillIDs[i];
                var skillLevel = 0;
                if(_skillLevelMap.TryGetValue(skillID, out var level) && level > 0)
                {
                    skillLevel = level;
                }
                
                skillSlot.Initialize(new SkillData(skillID, skillLevel));
                skillSlot.OnSkillButtonClick += OnSkillButtonClicked;
                
                if (i == 0)
                {
                    OnSkillButtonClicked(skillSlot);
                }
            }
        }
        
        private void OnSkillButtonClicked(SkillSlot slot)
        {
            _curSkillSlot = slot;
            // 显示技能介绍
            var skillConfig = SkillConfigManager.Instance.GetConfig(slot.skillData.skillID);
            skillNameText.text = skillConfig.skillName;
            
            if(_skillLevelMap.TryGetValue(slot.skillData.skillID, out var level) && level > 0)
            {
                skillLevelText.text = $"Lv.{level}";
                skillDescriptionText.text = skillConfig.GetSkillDescription(level);
                skillLockGroup.SetActive(false);
            }
            else
            {
                skillLevelText.text = "Lv.0";
                skillUnlockCostText.text = skillConfig.GetUpgradeCost(0).ToString();
                skillDescriptionText.text = skillConfig.GetSkillDescription(1); // 显示解锁后的技能描述
                skillLockGroup.SetActive(true);
            }
        }
        
        private void OnSkillUnlockButtonClicked()
        {
            // TODO 技能解锁逻辑
            // 查询资源是否足够
            if (PlayerDataManager.Instance.TryUnlockSkill(_curCharacterData.characterID, ref _curSkillSlot.skillData))
            {
                // 更新技能信息
                skillLevelText.text = "Lv.1";
                skillLockGroup.SetActive(false);
                _curSkillSlot.RefreshVisual();
            }
            else
            {
                Debug.Log("资源不足");
                // TODO 显示资源不足提示
                // UIManager.Instance.ShowPanel(PanelName.ResourceNotEnoughPanel, OpenStrategy.PauseCurrent, cost);
            }
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