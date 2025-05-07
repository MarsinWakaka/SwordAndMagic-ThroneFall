using System.Collections.Generic;
using Events.Global;
using GameLogic.Unit;
using MyFramework.Utilities;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    /// <summary>
    /// 角色名册面板
    /// </summary>
    public class CharacterRosterPanel : BaseUIPanel
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button navigationButton;
        
        [SerializeField] private CharacterDetailPanel characterDetailPanel;

        [SerializeField] private CharacterRosterSlot rosterSlotPrefab;
        [SerializeField] private Transform rosterSlotParent;
        private readonly List<CharacterRosterSlot> _activeRosterSlots = new();
        
        private void Awake()
        {
            backButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ClosePanel(PanelName.CharacterRosterPanel);
            });
            navigationButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
                UIManager.Instance.ShowPanel(PanelName.NavigationPanel, OpenStrategy.ReplaceCurrent);
            });
        }
        
        private CharacterData _currentCharacter;

        public override void OnCreate(object data)
        {
            base.OnCreate(data);
            
            // 查询玩家数据
            var playerData = Player.PlayerDataManager.Instance.GetAllOwnedCharacters();
            // 清空当前面板
            ClearSlots();
            // 生成玩家数据格子
            for (var i = 0; i < playerData.Count; i++)
            {
                var characterData = playerData[i];
                var slot = Instantiate(rosterSlotPrefab, rosterSlotParent);
                slot.Initialize(characterData);
                slot.OnClickAction += OnCharacterSlotClick;

                // 设置选中第一个角色
                if (i == 0)
                {
                    _currentCharacter = characterData;
                    characterDetailPanel.SetData(characterData);
                }
                
                _activeRosterSlots.Add(slot);
            }
            
            RegisterEvent();
        }
        
        
        public override void OnRelease()
        {
            _currentCharacter = null;
            ClearSlots();
            UnregisterEvent();
            // 类似析构函数的执行顺序，请先释放子类
            base.OnRelease();
        }
        
        // 监听角色数据更新事件
        private void RegisterEvent()
        {
            EventBus.Channel(Channel.Global).Subscribe<CharacterUpgradeSuccessEvent>(HandleCharacterUpgradeSuccess);
        }
        
        private void UnregisterEvent()
        {
            EventBus.Channel(Channel.Global).Unsubscribe<CharacterUpgradeSuccessEvent>(HandleCharacterUpgradeSuccess);
        }
        
        private void HandleCharacterUpgradeSuccess(CharacterUpgradeSuccessEvent upgradeEvent)
        {
            // 更新角色数据
            if (upgradeEvent.CharacterData.characterID == _currentCharacter.characterID)
            {
                characterDetailPanel.SetData(upgradeEvent.CharacterData);
            }
        }

        private void OnCharacterSlotClick(CharacterData data)
        {
            if (_currentCharacter.characterID == data.characterID) return;
            // 打开角色详情面板
            _currentCharacter = data;
            characterDetailPanel.SetData(data);
        }

        private void ClearSlots()
        {
            foreach (Transform child in rosterSlotParent)
            {
                Destroy(child.gameObject);
            }
            _activeRosterSlots.Clear();
        }
    }
}