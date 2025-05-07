using Config;
using Events.Global;
using GameLogic.Unit;
using GameLogic.Unit.ConfigData;
using MyFramework.Utilities;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    public class CharacterUpgradePanel : BaseUIPanel
    {
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Text upgradeButtonText;
        
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button prevLevelButton;
        
        [Header("VIEW")]
        [Header("当前组")]
        [SerializeField] private Text curLevelText;
        [SerializeField] private Text curMaxHpText;
        [SerializeField] private Text curMoveRangeText;
        [SerializeField] private Text curInitSkillPointText;
        // 未满级
        [Header("升级后")]
        [SerializeField] private GameObject targetLevelGroup;
        [SerializeField] private Text targetLevelText;
        [SerializeField] private Text targetMaxHpText;
        [SerializeField] private Text targetMoveRangeText;
        [SerializeField] private Text targetInitSkillPointText;
        
        [Header("满级提示")]
        [SerializeField] private Text hasMaxLevelTips;
        
        [Header("其它")]
        [SerializeField] private Color canUpgradeColor;
        [SerializeField] private Color cannotUpgradeColor;
        
        public override void OnCreate(object data)
        {
            base.OnCreate(data);
            if (data is CharacterData characterData)
            {
                _data = characterData;
                SetVisual(characterData);
            }
            else
            {
                Debug.LogError("CharacterData is null");
                UIManager.Instance.ClosePanel(PanelName.CharacterUpgradePanel);
                return;
            }
            
            cancelButton.onClick.AddListener(OnCloseButtonClick);
            upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
            prevLevelButton.onClick.AddListener(OnPrevLevelButtonClick);
            EventBus.Channel(Channel.Global).Subscribe<CharacterUpgradeSuccessEvent>(HandleCharacterUpgradeSuccess);
        }
        
        public override void OnRelease()
        {
            base.OnRelease();
            _data = null;
            cancelButton.onClick.RemoveListener(OnCloseButtonClick);
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClick);
            nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClick);
            prevLevelButton.onClick.RemoveListener(OnPrevLevelButtonClick);
            EventBus.Channel(Channel.Global).Unsubscribe<CharacterUpgradeSuccessEvent>(HandleCharacterUpgradeSuccess);
        }
        
        private void HandleCharacterUpgradeSuccess(CharacterUpgradeSuccessEvent upgradeSuccessEvent)
        {
            if (upgradeSuccessEvent.CharacterData.characterID == _data.characterID)
            {
                SetVisual(_data);
            }
        }
        
        private CharacterData _data;
        private CharacterConfigData _configCache;
        private int _maxLevel;
        private int _curLevel;
        private int _targetLevel;

        private void SetVisual(CharacterData characterData)
        {
            // 查询角色数据
            _configCache = CharacterConfigManager.Instance.GetConfig(characterData.characterID);
            var curProperties = _configCache.GetCharacterProperties(characterData.level);
            
            _curLevel = characterData.level;
            _targetLevel = characterData.level + 1;
            _maxLevel = _configCache.MaxLevel;
            
            curLevelText.text = $"Lv.{characterData.level}";
            curMaxHpText.text = $"生命值 : {curProperties.maxHp}";
            curMoveRangeText.text = $"移动距离 : {curProperties.maxMoveRange}";
            curInitSkillPointText.text = $"初始技能点 : {curProperties.initSkillPoint}";
            
            UpdateTargetGroupData();
        }

        private void UpdateTargetGroupData()
        {
            var canUpgrade = false;
            if (_targetLevel <= _configCache.MaxLevel)
            {
                // 有下一级
                var nextCharacterProperties = _configCache.GetCharacterProperties(_targetLevel);
                targetLevelText.text = $"Lv.{_targetLevel}";
                targetMaxHpText.text = $"生命值 : {nextCharacterProperties.maxHp}";
                targetMoveRangeText.text = $"移动距离 : {nextCharacterProperties.maxMoveRange}";
                targetInitSkillPointText.text = $"初始技能点 : {nextCharacterProperties.initSkillPoint}";
                
                // 客户端判断够不够升级
                var needRes = CharacterUpgradeCost.GetAccumulatedCost(_curLevel, _targetLevel);
                var ownedRes = Player.PlayerDataManager.Instance.GetPlayerResources();
                canUpgrade = ownedRes >= needRes;
                upgradeButtonText.text = $"消耗 {needRes}/{ownedRes}";
            }
            else
            {
                // 已满级
                targetLevelGroup.SetActive(false);
                hasMaxLevelTips.gameObject.SetActive(true);
                
                canUpgrade = false;
                upgradeButtonText.text = "已满级";
            }
            upgradeButton.interactable = canUpgrade;
            upgradeButtonText.color = canUpgrade ? canUpgradeColor : cannotUpgradeColor;
        }
        
        private void OnNextLevelButtonClick()
        {
            if (_targetLevel < _maxLevel)
            {
                _targetLevel++;
                UpdateTargetGroupData();
            }
        }
        
        private void OnPrevLevelButtonClick()
        {
            if (_targetLevel - _curLevel > 1)
            {
                _targetLevel--;
                UpdateTargetGroupData();
            }
        }

        private void OnCloseButtonClick()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            UIManager.Instance.ClosePanel(PanelName.CharacterUpgradePanel);
        }

        private void OnUpgradeButtonClick()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            EventBus.Channel(Channel.Global).Publish(new CharacterUpgradeRequestEvent(_data.characterID, _targetLevel));
        }
    }
}