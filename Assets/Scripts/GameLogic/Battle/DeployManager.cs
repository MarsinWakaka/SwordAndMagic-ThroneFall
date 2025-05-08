using System.Collections.Generic;
using Events.Battle;
using GameLogic.Grid;
using GameLogic.Grid.Area;
using GameLogic.LevelSystem;
using GameLogic.Map;
using GameLogic.Unit;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using Player;
using SoundSystem;
using UI;
using UI.ConcretePanel.Battle;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace GameLogic.Battle
{
    /// <summary>
    /// [Function]
    /// 负责单位部署的管理类
    /// [Purpose]
    /// 打开部署界面(显示玩家可以部署的单位)，数据来源来自于MapContext和PlayerData
    /// 玩家左键点击部署界面的单位后，再次点击地面即可在对应位置部署该角色（并禁用该角色的再次部署）
    /// 玩家右键点击已部署的角色可以取消该角色的部署
    /// ID基于角色InstanceID
    /// </summary>
    public class DeployManager : MonoBehaviour
    {
        [SerializeField] private Faction belongFaction;
        private Camera _viewCamera;
        public LayerMask groundLayer;
        private void Awake()
        {
            
            // 获取主摄像机
            _viewCamera = Camera.main;
            if (_viewCamera == null)
            {
                Debug.LogError("Main camera not found!");
                return;
            }
        }

        // 监听部署事件
        private void OnEnable()
        {
            EventBus.Channel(Channel.Gameplay).Subscribe<StartDeployEvent>(HandleStartDeployUnitEvent);
            EventBus.Channel(Channel.Gameplay).Subscribe<SelectDeployUnitEvent>(HandleSelectDeployUnitEvent);
            EventBus.Channel(Channel.Gameplay).Subscribe<FinishDeployEvent>(HandleDeployEnd);
        }

        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<StartDeployEvent>(HandleStartDeployUnitEvent);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<SelectDeployUnitEvent>(HandleSelectDeployUnitEvent);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<FinishDeployEvent>(HandleDeployEnd);
        }
        
        private DeployCharacterData _selectedCharacterData;
        private int _curDeployCount;

        private void Update()
        {
            HandleMouseHover();
            HandleMouseClick();
        }
        
        private void HandleMouseClick()
        {
            // TODO : 处理鼠标点击事件
            // 1. 鼠标左键点击地面，部署角色
            if (Input.GetMouseButtonDown(0))
            {
                // Debug.Log("Left mouse button clicked.");
                // 判断有没有鼠标有没有在UI上
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // Debug.Log("Mouse is over UI.");
                    return;
                }
                // 如果当前选择角色为空，则直接返回
                if (_selectedCharacterData == null)
                {
                    Debug.Log("No character selected.");
                    return;
                }
                //
                // var hitInfo = Physics2D.Raycast(
                //     _viewCamera.ScreenToWorldPoint(Input.mousePosition), 
                //     default, 
                //     100f,
                //     groundLayer);
                //
                // // 如果点击到地面则判断地面上是否有角色
                // var grid = hitInfo.collider?.GetComponent<GridController>();
                
                var grid = CurPointGrid;
                
                if (grid == null)
                {
                    Debug.Log("No grid found.");
                    return;
                }
                
                // 检查是否可以部署
                var statusCode = CheckDeployCondition(grid);
                if (statusCode != 0)
                {
                    // TODO 给用户相应的提示
                    Debug.Log($"Cannot deploy here , STATUS CODE: {statusCode}"); 
                    return;
                }
                // 成功部署
                _curDeployCount++;
                _selectedCharacterData.CanUse.Value = false;
                EventBus.Channel(Channel.Gameplay).Publish(new DeployCharacterCountChangeEvent(_curDeployCount));
                EventBus.Channel(Channel.Gameplay).Publish(new SpawnCharacterEvent(belongFaction, new []
                {
                    new CharacterSpawnData(
                        _selectedCharacterData.Data,
                        grid.GetGrid2DCoord(),
                        default)
                }));
                // 这里这样子做，容易导致后续可能UI更新不及时导致不同步，需要细致处理
                _selectedCharacterData = null;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                // Debug.Log("Right mouse button clicked.");
                // TODO : 取消部署
            }
        }
        
        private StartDeployEvent _deployParams;

        private void HandleStartDeployUnitEvent(StartDeployEvent evt)
        {
            _deployParams = evt;
            
            SoundManager.Instance.PlayBGM(BGM.BattleDeployBGM);
            // Display the deployable area
            EventBus.Channel(Channel.Gameplay).Publish(new AreaDisplayEvent(AreaType.Deployable, evt.CanDeployPositions));
            
            // 创建部署数据
            var charactersData = evt.DeployCharacterData;
            var deployDataList = new List<DeployCharacterData>();
            foreach (var characterData in charactersData)
            {
                deployDataList.Add(new DeployCharacterData(characterData));
            }
            
            // TODO 根据条件筛选不满足的角色
            
            Debug.Log("HandleDeployUnitEvent");
            UIManager.Instance.ShowPanel(PanelName.BattleDeployPanel, OpenStrategy.Additive, 
                new DeployPanelData(deployDataList, evt.MaxDeployCount));
        }
        
        private void HandleSelectDeployUnitEvent(SelectDeployUnitEvent obj)
        {
            Debug.Log($"Cur Selected character ID: {obj.SelectedCharacterData.Data.characterID}");
            _selectedCharacterData = obj.SelectedCharacterData;
        }

        private void HandleDeployEnd(FinishDeployEvent evt)
        {
            Debug.Log("HandleDeployEnd");
            UIManager.Instance.ClosePanel(PanelName.BattleDeployPanel);
            EventBus.Channel(Channel.Gameplay).Publish(new AreaHideEvent(AreaType.Deployable));
            // TODO 清空数据
            _deployParams.OnOnDeployComplete();
            Destroy(gameObject);
        }

        /// <summary>
        /// 返回检测结果的状态码
        /// </summary>
        private int CheckDeployCondition(GridController targetGrid)
        {
            if (_curDeployCount >= _deployParams.MaxDeployCount) return 1;
            var targetCoord = targetGrid.GetGrid2DCoord();
            // TODO 检查是否在地图范围内
            // 检查该位置是否在可部署范围内
            if (!_deployParams.CanDeployPositions.Contains(targetCoord)) return 2;
            // 检查该位置是否有单位
            if (targetGrid.RuntimeData.EntitiesOnThis != null) return 3;
            return 0;
        }

        #region 交互控制器功能
        
        private GridController _curPointGrid;
        public GridController CurPointGrid
        {
            get => _curPointGrid;
            set
            {
                if (_curPointGrid == value) return;
                _curPointGrid?.OnCancelMouseHover();
                _curPointGrid = value;
                _curPointGrid?.OnMouseHover();
            }
        }
        
        private void HandleMouseHover()
        {
            var hitInfo = Physics2D.Raycast(
                _viewCamera.ScreenToWorldPoint(Input.mousePosition), 
                default, 
                100f, 
                groundLayer);
            if (hitInfo.collider == null) 
            {
                CurPointGrid = null;
                return;
            }
            if (hitInfo.collider == null) return;
            var grid = hitInfo.collider.GetComponent<GridController>();
            CurPointGrid = grid;
        }

        #endregion
    }
}