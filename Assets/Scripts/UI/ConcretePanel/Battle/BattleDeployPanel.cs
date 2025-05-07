using System.Collections.Generic;
using Events.Battle;
using GameLogic.Battle;
using MyFramework.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.Battle
{
    public class DeployPanelData
    {
        public readonly int MaxDeployCount;
        // 部署面板数据
        public readonly List<DeployCharacterData> DeployCharacterData;

        public DeployPanelData(List<DeployCharacterData> deployCharacterData, int maxDeployCount)
        {
            DeployCharacterData = deployCharacterData;
            MaxDeployCount = maxDeployCount;
        }
    }
    
    /// <summary>
    /// 显示部署的数据，并提供UI供玩家交互，不涉及具体的角色部署
    /// </summary>
    public class BattleDeployPanel : BaseUIPanel
    {
        // 作战开始按钮
        [SerializeField] private Button battleStartButton;
        [SerializeField] private Text curDeployCountText;
        
        // 选择框
        [SerializeField] private Transform selectBox;
        [SerializeField] private Transform deploySlotParent;
        [SerializeField] private DeploySlot deploySlotPrefab;
        
        private int _maxDeployCount;
        private DeployCharacterData _selectedCharacterData;

        private void Awake()
        {
            battleStartButton.onClick.AddListener(() =>
            {
                EventBus.Channel(Channel.Gameplay).Publish(new FinishDeployEvent());
            });
        }

        public override void OnCreate(object data)
        {
            base.OnCreate(data);
            // 请求玩家数据
            if (data is not DeployPanelData deployData)
            {
                Debug.LogError("DeployPanelData is null");
                return;
            }
            
            ClearAllSlots(true);
            _maxDeployCount = deployData.MaxDeployCount;
            InitializeUI();
            var deployCharactersData = deployData.DeployCharacterData;
            foreach (var characterData in deployCharactersData)
            {
                var deploySlot = Instantiate(deploySlotPrefab, deploySlotParent);
                deploySlot.gameObject.SetActive(true);
                deploySlot.InitializeSlot(characterData);
                deploySlot.OnClickSlot += HandleDeploySlotClick;
            }
        }

        private void OnEnable()
        {
            EventBus.Channel(Channel.Gameplay).Subscribe<DeployCharacterCountChangeEvent>(HandleDeployCharacterCountChanged);
        }
        
        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<DeployCharacterCountChangeEvent>(HandleDeployCharacterCountChanged);
        }
        
        private void InitializeUI()
        {
            battleStartButton.interactable = false;
            selectBox.gameObject.SetActive(false);
            curDeployCountText.text = $"0/{_maxDeployCount}";
        }
        
        private void HandleDeployCharacterCountChanged(DeployCharacterCountChangeEvent evt)
        {
            curDeployCountText.text = $"{evt.CurrentCount}/{_maxDeployCount}";
            battleStartButton.interactable = evt.CurrentCount > 0;
        }

        private void HandleDeploySlotClick(Vector3 slotPos, DeployCharacterData characterData)
        {
            // TODO : 处理点击事件
            if (_selectedCharacterData != null && 
                _selectedCharacterData.Guid == characterData.Guid)
            {
                // 重复选择为取消
                _selectedCharacterData = null;
                Debug.Log("Clicked on the same deploy slot, deselecting.");
                selectBox.gameObject.SetActive(false);
            }
            else
            {
                _selectedCharacterData = characterData;
                selectBox.gameObject.SetActive(true);
                selectBox.position = slotPos;
            }
            EventBus.Channel(Channel.Gameplay).Publish(new SelectDeployUnitEvent(characterData));
            Debug.Log($"Clicked on deploy slot with character ID: {characterData.Data.characterID}");
        }

        private void ClearAllSlots(bool isAssertNone = false)
        {
            var slots = deploySlotParent.GetComponentsInChildren<DeploySlot>(true);
            if (slots.Length == 0) return;
            
            if (isAssertNone)
            {
                Debug.LogError("There are still active deploy slots, count: " + slots.Length);
            }
            
            foreach (var active in slots)
            {
                Destroy(active.gameObject);
            }
        }
    }
}