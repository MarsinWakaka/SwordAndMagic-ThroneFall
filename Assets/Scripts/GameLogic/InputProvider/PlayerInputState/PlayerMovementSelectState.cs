// using Events;
// using Events.Battle;
// using GameLogic.Grid;
// using GameLogic.Grid.Area;
// using GameLogic.Unit;
// using GameLogic.Unit.Controller;
// using MyFramework.Utilities;
// using MyFramework.Utilities.FSM;
// using UI;
// using UI.ConcretePanel.Battle;
// using UnityEngine;
//
// namespace GameLogic.InputProvider.PlayerInputState
// {
//     public class MovementStateEnterParams : IStateParams
//     {
//         public readonly CharacterUnitController UnitController;
//
//         public MovementStateEnterParams(CharacterUnitController unitController)
//         {
//             UnitController = unitController;
//         }
//     }
//     
//     public class MovementStateExitParams : IStateParams
//     {
//         public InputState NextState;
//     }
//     
//     public class PlayerMovementSelectState : IState
//     {
//         private readonly PlayerInputProvider _inputProvider;
//         private readonly int _rayCastLayer;
//         
//         private CharacterUnitController _selectedUnit;
//         private MoveableAreaResult _moveableAreaResult;
//         
//         public PlayerMovementSelectState(PlayerInputProvider inputProvider)
//         {
//             _inputProvider = inputProvider;
//             _rayCastLayer = _inputProvider.groundLayer | _inputProvider.entityLayer;
//         }
//         
//         public void OnEnter(IStateParams stateParams)
//         {
//             if (stateParams is not MovementStateEnterParams args) 
//                 throw new System.ArgumentException("Invalid state parameters for MovementState");
//             _inputProvider.OnHandleRaycastInfo += HandleRaycastInfo;
//             _selectedUnit = args.UnitController;
//             // 显示移动范围
//             EventBus.Channel(Channel.Battle).Publish(GameEvent.CalculateMovableAreaRequest, new CalculateMoveableAreaRequest(_selectedUnit.CharacterRuntimeData, 
//                     result => _moveableAreaResult = result));
//             EventBus.Channel(Channel.Battle).Publish(GameEvent.AreaDisplayEvent, new AreaDisplayEvent(AreaType.Move, _moveableAreaResult.ToList()));
//             // 监听技能按钮点击事件
//             EventBus.Channel(Channel.Battle).Subscribe<SkillSlotSelectedEvent>(GameEvent.OnSkillSlotSelected, HandleSkillSlotSelected);
//             
//             UIManager.Instance.ShowPanel(PanelName.BattleCharacterControlPanel, OpenStrategy.Additive, 
//                 new CharacterControlPanelParams(args.UnitController.CharacterRuntimeData));
//         }
//
//         /// <summary>
//         /// 技能按钮被点击
//         /// </summary>
//         private void HandleSkillSlotSelected(SkillSlotSelectedEvent evt)
//         {
//             _inputProvider.Fsm.ChangeState(InputState.AttackSelect, new AttackStateParams(_selectedUnit, evt.ActiveSkillInstance));
//         }
//
//         public void OnUpdate()
//         {
//             _inputProvider.ShowMouseHoverStyleOnGrid();
//             _inputProvider.HandleMouseScroll();
//             
//             if (Input.GetMouseButtonDown(0))
//             {
//                 _inputProvider.RayCastAll(_rayCastLayer);
//             }
//         }
//
//         public void OnExit(IStateParams stateParams)
//         {
//             // 监听技能按钮点击事件
//             EventBus.Channel(Channel.Battle).Unsubscribe<SkillSlotSelectedEvent>(GameEvent.OnSkillSlotSelected, HandleSkillSlotSelected);
//             EventBus.Channel(Channel.Battle).Publish(GameEvent.ClearStyleEvent, new ClearAreaDisplay(AreaType.Move));
//             _inputProvider.OnHandleRaycastInfo -= HandleRaycastInfo;
//             
//             UIManager.Instance.ClosePanel(PanelName.BattleCharacterControlPanel);
//         }
//         
//         private void HandleRaycastInfo(RaycastHit2D hitInfo)
//         {
//             if (hitInfo.collider == null) return;
//             
//             // 优先处理角色
//             var unit = hitInfo.collider.GetComponent<CharacterUnitController>();
//             if (unit != null) 
//             {
//                 if (TryHandleObjectSelected(unit)) return;
//             }
//                 
//             // 处理格子
//             var gridCtrl = hitInfo.collider.GetComponent<GridController>();
//             if (gridCtrl == null) return;
//             var gridData = gridCtrl.RuntimeData;
//             if (gridData.UnitsOnGrid.Count > 0)
//             {
//                 var index = 0;
//                 while (index < gridData.UnitsOnGrid.Count)
//                 {
//                     if (TryHandleObjectSelected(gridData.UnitsOnGrid[index])) return;
//                     index++;
//                 }
//             }
//             
//             var gridCoord = gridData.GridCoord.Value;
//             if (_moveableAreaResult.TryGetPathWayTo(new Vector2Int(gridCoord.x, gridCoord.y), out var pathWay))
//             {
//                 EventBus.Channel(Channel.Battle).Publish(GameEvent.ShowPathWayEvent, new DisplayPathWayEvent(pathWay));
//
//                 var avatar = _selectedUnit.GetAvatar();
//                 // TODO 绘制残影效果
//                 _selectedUnit.Teleport(new Vector2Int(gridCoord.x, gridCoord.y));
//             }
//             else
//             {
//                 // TODO 选中格子
//                 // 切换回选择状态
//                 _inputProvider.Fsm.ChangeState(InputState.SelectTarget);
//             }
//         }
//
//         private bool TryHandleObjectSelected(EntityController entity)
//         {
//             switch (entity)
//             {
//                 case CharacterUnitController unit:
//                     // TODO 选中角色
//                     // TODO 处理选中对象
//                     // A：如果点击的是玩家角色优先切换角色
//                     // B: 如果点击的是敌人、障碍物
//                     // - 如果可以攻击(类型匹配且范围内)，进入[攻击确认状态](默认选中平A)
//                     // - 如果不可以攻击，则进入[选择状态] 并打开对应的查看界面
//                     var faction = unit.CharacterRuntimeData.faction;
//                     switch (faction)
//                     {
//                         case Faction.Player:
//                             // TODO 切换角色
//                             _inputProvider.Fsm.ChangeState(InputState.MoveAction, 
//                                 new MovementStateEnterParams(unit));
//                             return true;
//                         case Faction.Enemy:
//                             // TODO 进入攻击确认状态
//                             break;
//                         default:
//                             Debug.LogError($"Invalid faction: {faction} for unit {unit.gameObject.name}");
//                             break;
//                     }
//                     break;
//                 // case ObstacleController obstacle:
//                 //     // TODO 进入选择状态
//                 //     // var unitType = obstacle.entityData.StaticData.entityType;
//                 //     Debug.Log($"选中障碍物: {obstacle.gameObject.name}");
//                 //     break;
//             }
//
//             return false;
//         }
//     }
// }