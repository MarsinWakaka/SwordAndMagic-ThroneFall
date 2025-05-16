using Events.Battle;
using Events.Battle.Skill;
using GameLogic.GameAction.Attack;
using GameLogic.Grid;
using GameLogic.Grid.Area;
using GameLogic.Skill.Active;
using GameLogic.TimeLine;
using GameLogic.Unit;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using MyFramework.Utilities.Stack;
using UI;
using UI.ConcretePanel.SubPanel;
using UnityEngine;
using Utilities;

namespace GameLogic.ManualInput.Concrete
{
    public class AttackSelectParams : IStackNodeParams
    {
        public readonly CharacterUnitController Character;
        public readonly ActiveSkillInstance ActiveSkillInstance;

        public AttackSelectParams(ActiveSkillInstance activeSkillInstance, CharacterUnitController character)
        {
            ActiveSkillInstance = activeSkillInstance;
            Character = character;
        }
    }
    
    public class AttackSelectState : IPersistentStackNode
    {
        private readonly ManualInputController _manualInputController;
        private CharacterUnitController _character;
        private ActiveSkillInstance _activeSkillInstance;
        
        // 临时变量
        private AttackableAreaResult _attackableAreaResult;
        private ActiveSkillSelectContext _activeSkillSelectContext;
        // private ActiveSkillSelectResult _activeSkillSelectResult;

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

        public AttackSelectState(ManualInputController manualInputController)
        {
            _manualInputController = manualInputController;
        }
        
        public void OnEnter(IStackNodeParams parameters = null)
        {
            if (parameters is not AttackSelectParams selectParams)
            {
                Debug.LogError("[Input Controller] : AttackSelectState requires AttackSelectParams");
                return;
            }
            // TODO 重置临时变量
            _attackableAreaResult = null;
            // _activeSkillSelectResult = null;
            
            _activeSkillInstance = selectParams.ActiveSkillInstance;
            _character = selectParams.Character;
            EventBus.Channel(Channel.Gameplay).Publish(
                new CalculateAttackableAreaRequest(
                    new AttackParam()
                    {
                        GridCoord = _character.Coordinate(),
                        AttackRange = _activeSkillInstance.ActiveConfig.attackRange
                    }, HandleAreaResult));
        }

        public void OnResume()
        {
            _manualInputController.EnableMouseHoverStyle = true;
            _manualInputController.OnHandleRaycastInfo += HandleRaycastInfo;
            UIManager.Instance.ShowPanel(PanelName.BattleSkillReleasePanel, OpenStrategy.Additive, new SkillReleaseConfirmPanelParams(_activeSkillInstance));
            
            EventBus.Channel(Channel.Gameplay).Subscribe<SkillReleaseConfirmInput>(HandleConfirmButtonClicked);
            EventBus.Channel(Channel.Gameplay).Subscribe<CancelSkillReleaseInput>(HandleCancelButtonClicked);
        }

        public void OnPause()
        {
            _manualInputController.EnableMouseHoverStyle = false;
            _manualInputController.OnHandleRaycastInfo -= HandleRaycastInfo;
            UIManager.Instance.ClosePanel(PanelName.BattleSkillReleasePanel);
            
            EventBus.Channel(Channel.Gameplay).Unsubscribe<SkillReleaseConfirmInput>(HandleConfirmButtonClicked);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<CancelSkillReleaseInput>(HandleCancelButtonClicked);
        }

        public void OnExit()
        {
            // TODO 隐藏攻击范围
            EventBus.Channel(Channel.Gameplay).Publish(new AreaHideEvent(AreaType.Attackable));
            EventBus.Channel(Channel.Gameplay).Publish(new AreaHideEvent(AreaType.Skill));
            _attackableAreaResult = null;
        }

        public void OnUpdate()
        {
            
        }
        
        private void HandleAreaResult(AttackableAreaResult result)
        {
            _attackableAreaResult = result;
            EventBus.Channel(Channel.Gameplay).Publish(new AreaDisplayEvent(AreaType.Attackable, _attackableAreaResult.Area));
        }

        /// <summary>
        /// 选择技能后，处理射线检测到的对象
        /// </summary>
        private void HandleRaycastInfo(RaycastHit2D hitInfo)
        {
            if (hitInfo.collider == null) return;
            var grid = hitInfo.collider.GetComponent<GridController>();
            if (grid == null) return;
            var gridCoord = grid.GetGrid2DCoord();
            // 判断是否在攻击范围内
            if (!_attackableAreaResult.IsInAttackableArea(gridCoord)) return;
            
            // 实际可攻击到的范围，计算并显示
            var skillScope = _activeSkillInstance.ActiveConfig.GetSkillScope(
                _character.Coordinate(),
                gridCoord, 
                GridManagerCache.IsWalkableTerrain);
            EventBus.Channel(Channel.Gameplay).Publish(new AreaDisplayEvent(AreaType.Skill, skillScope, true));
            
            // 构造技能选择上下文
            var curSelectContext = new ActiveSkillSelectContext(_character, grid, ActionType.Active);
            // var isValid = _activeSkillInstance.IsTargetTypeValid(curSelectContext);
            var isValid = true; // TODO : 处理技能类型
            if (isValid)
            {
                Debug.Log($"Valid target: {grid.RuntimeData.GridCoord.Value} scopeCount: {skillScope.Count}");
                _activeSkillSelectContext = curSelectContext;
            }
            else
            {
                // TODO 处理无效目标
                Debug.Log($"Invalid target: {grid.RuntimeData.GridCoord.Value}");
            }
            // var unit = hitInfo.collider.GetComponent<CharacterUnitController>();
            // if (unit == null) return;
            // var hasValidTarget = TryHandleObjectSelected(unit);
            EventBus.Channel(Channel.Gameplay).Publish(new SkillTargetSelectedUpdateUIEvent(isValid));
        }
        
        private bool TryHandleObjectSelected(EntityController entity)
        {
            switch (entity)
            {
                case CharacterUnitController target:
                    // TODO 选中角色
                    // TODO 处理选中对象
                    // A：如果点击的是玩家角色优先切换角色
                    // B: 如果点击的是敌人、障碍物
                    // - 如果可以攻击(类型匹配且范围内)，进入[攻击确认状态](默认选中平A)
                    // - 如果不可以攻击，则进入[选择状态] 并打开对应的查看界面
                    var faction = target.CharacterRuntimeData.faction;
                    switch (faction)
                    {
                        case Faction.Player:
                        case Faction.Enemy:
                            var config = _activeSkillInstance.ActiveConfig;
                            return true;
                        default:
                            Debug.LogError($"Invalid faction: {faction} for unit {target.gameObject.name}");
                            break;
                    }
                    break;
            }
            return false;
        }
        
        private void HandleCancelButtonClicked(CancelSkillReleaseInput cancel)
        {
            // TODO 取消影响范围显示
            _manualInputController.StackManager.Pop();
        }

        /// <summary>
        /// 处理攻击确认
        /// </summary>
        private void HandleConfirmButtonClicked(SkillReleaseConfirmInput confirm)
        {
            if (_activeSkillSelectContext == null)
            {
                Debug.LogError("[Input Controller] : SkillSelectState _skillSelectResult is null");
                return;
            }
            var wrapper = new ActiveSkillExecuteContextWrapper(_activeSkillSelectContext, _activeSkillInstance.Execute);
            // TODO 处理攻击确认
            TimeLineManager.Instance.AddPerform(wrapper.Execute);
            _manualInputController.OnCharacterAction(_character.CharacterRuntimeData);
        }
    }
}