using Events.Battle;
using GameLogic.Grid;
using GameLogic.Grid.Area;
using GameLogic.Unit;
using GameLogic.Unit.BattleRuntimeData;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using MyFramework.Utilities.Extensions;
using MyFramework.Utilities.Stack;
using UI;
using UI.ConcretePanel.Battle;
using UnityEngine;

namespace GameLogic.ManualInput.Concrete
{
    public class CharacterActionParam : IStackNodeParams
    {
        public readonly CharacterUnitController CharacterUnitController;

        public CharacterActionParam(CharacterUnitController characterUnitController)
        {
            CharacterUnitController = characterUnitController;
        }
    }
    
    public class CharacterActionState : IPersistentStackNode
    {
        private readonly ManualInputController _manualInputController;
        
        private CharacterUnitController _curUnit;
        
        private MoveableAreaResult _moveableAreaResult;
        
        private IGridManager _gridManagerCache;
        private IGridManager GridManagerCache
        {
            get
            {
                if (_gridManagerCache != null) return _gridManagerCache;
                _gridManagerCache = ServiceLocator.Resolve<IGridManager>();
                if (_gridManagerCache == null)
                {
                    Debug.LogError("GridManagerCache is null. Please check the initialization.");
                }
                return _gridManagerCache;
            }
        }
        
        public CharacterActionState(ManualInputController manualInputController)
        {
            _manualInputController = manualInputController;
        }
        
        public void OnEnter(IStackNodeParams parameters = null)
        {
            if (parameters == null || parameters is not CharacterActionParam param)
            {
                Debug.LogError("[Input Controller] : MovementSelectState requires MovementSelectParam");
                return;
            }
            _curUnit = param.CharacterUnitController;
            
        }

        public void OnResume()
        {
            _manualInputController.EnableMouseHoverStyle = true;
            _manualInputController.OnHandleRaycastInfo += HandleRaycastInfo;
            EventBus.Channel(Channel.Gameplay).Subscribe<SkillSlotSelectedEvent>(HandleSkillSlotSelected);
            
            // 移动相关
            var curCoord = _curUnit.RuntimeData.gridCoord;
            _moveTargetCoord = new Vector3Int(curCoord.x, curCoord.y, GridManagerCache.QueryHeight(curCoord.x, curCoord.y));
            _manualInputController.OnCharacterActionEndConfirm += OnCharacterMoveConfirm;
            EventBus.Channel(Channel.Gameplay).Publish(new CalculateMoveableAreaRequest(_curUnit.CharacterRuntimeData,
                result =>
                {
                    _moveableAreaResult = result;
                    EventBus.Channel(Channel.Gameplay).Publish(new AreaDisplayEvent(AreaType.Move ,_moveableAreaResult.ToList()));
                }));
            UIManager.Instance.ShowPanel(PanelName.BattleCharacterControlPanel, OpenStrategy.Additive, new CharacterControlPanelParams(_curUnit.CharacterRuntimeData));
        }
        
        public void OnPause()
        {
            _manualInputController.EnableMouseHoverStyle = false;
            UIManager.Instance.ClosePanel(PanelName.BattleCharacterControlPanel);
            EventBus.Channel(Channel.Gameplay).Publish(new AreaHideEvent(AreaType.Move));
            EventBus.Channel(Channel.Gameplay).Unsubscribe<SkillSlotSelectedEvent>(HandleSkillSlotSelected);
            _manualInputController.OnHandleRaycastInfo -= HandleRaycastInfo;
            // 移动相关
            OnCharacterMoveConfirm();
            _manualInputController.OnCharacterActionEndConfirm -= OnCharacterMoveConfirm;
            EventBus.Channel(Channel.Gameplay).Publish(new HidePathwayEvent());
            _manualInputController.HideGhostAvatar();
        }

        public void OnExit()
        {
            
            _moveableAreaResult = null;
        }

        public void OnUpdate()
        {
            
        }
        
        private CharacterUnitController _ghostAvatar;
        private CharacterBattleRuntimeData _ghostAvatarData;
        private Vector3Int _moveTargetCoord;
        
        /// <summary>
        /// 技能按钮被点击
        /// </summary>
        private void HandleSkillSlotSelected(SkillSlotSelectedEvent evt)
        {
            _manualInputController.StackManager.Push(InputState.AttackSelect, 
                new AttackSelectParams(evt.ActiveSkillInstance, _curUnit));
        }
        
        /// <summary>
        /// 进入行动选择后，处理射线检测到的对象
        /// </summary>
        /// <param name="hitInfo"></param>
        private void HandleRaycastInfo(RaycastHit2D hitInfo)
        {
            if (hitInfo.collider == null) return;
            
            // 优先处理角色
            var unit = hitInfo.collider.GetComponent<CharacterUnitController>();
            if (unit != null && TryHandleObjectSelected(unit)) return;
            
            // 处理格子
            var gridCtrl = hitInfo.collider.GetComponent<GridController>();
            if (gridCtrl == null) return;
            var gridData = gridCtrl.RuntimeData;
            if (gridData.EntitiesOnThis != null)
            {
                if (TryHandleObjectSelected(gridData.EntitiesOnThis)) return;
            }
            
            var gridCoord = gridData.GridCoord.Value;
            if (_curUnit.RuntimeData.gridCoord.IsSameOnXYAxis(gridCoord)) return;
            if (_moveableAreaResult.TryGetPathWayTo(new Vector2Int(gridCoord.x, gridCoord.y), out var pathWay))
            {
                EventBus.Channel(Channel.Gameplay).Publish(new DisplayPathWayEvent(pathWay));
                // TODO 绘制残影效果
                _moveTargetCoord = gridCoord;
                _manualInputController.SetGhostSprite(_curUnit.Animator.runtimeAnimatorController, _moveTargetCoord);
            }
            else
            {
                // 切换回选择状态
                _manualInputController.StackManager.Pop();
            }
        }
        
        /// <summary>
        /// 角色确认移动
        /// </summary>
        private void OnCharacterMoveConfirm()
        {
            // 确认结束角色行动时，角色移动到目标格子
            if (!_curUnit.RuntimeData.gridCoord.IsSameOnXYAxis(_moveTargetCoord))
            {
                // TODO 角色移动
                // _curUnit.Teleport(new Vector2Int(_moveTargetCoord.x, _moveTargetCoord.y));
                ServiceLocator.Resolve<IUnitManager>().MoveUnit(
                    _curUnit, 
                    _curUnit.RuntimeData.gridCoord, 
                    new Vector2Int(_moveTargetCoord.x, _moveTargetCoord.y));
            }
        }
        
        private bool TryHandleObjectSelected(EntityController entity)
        {
            switch (entity)
            {
                case CharacterUnitController unit:
                    // TODO 选中角色
                    // TODO 处理选中对象
                    // A：如果点击的是玩家角色优先切换角色
                    // B: 如果点击的是敌人、障碍物
                    // - 如果可以攻击(类型匹配且范围内)，进入[攻击确认状态](默认选中平A)
                    // - 如果不可以攻击，则进入[选择状态] 并打开对应的查看界面
                    var faction = unit.CharacterRuntimeData.faction;
                    switch (faction)
                    {
                        case Faction.Player:
                            // TODO 切换角色
                            _manualInputController.StackManager.Pop();
                            _manualInputController.StackManager.Push(InputState.MovementSelect, new CharacterActionParam(unit));
                            return true;
                        case Faction.Enemy:
                            // TODO 进入攻击确认状态
                            break;
                        default:
                            Debug.LogError($"Invalid faction: {faction} for unit {unit.gameObject.name}");
                            break;
                    }
                    break;
            }

            return false;
        }
    }
}