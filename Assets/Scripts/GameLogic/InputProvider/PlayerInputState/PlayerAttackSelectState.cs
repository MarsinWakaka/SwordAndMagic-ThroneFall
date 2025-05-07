// using Events;
// using Events.Battle;
// using Events.Battle.Skill;
// using GameLogic.Skill.Active;
// using GameLogic.Unit;
// using GameLogic.Unit.Controller;
// using MyFramework.Utilities;
// using MyFramework.Utilities.FSM;
// using UI;
// using UI.ConcretePanel.SubPanel;
// using UnityEngine;
//
// namespace GameLogic.InputProvider.PlayerInputState
// {
//     public class AttackStateParams : IStateParams
//     {
//         public CharacterUnitController UnitController { get; }
//         public ActiveSkillInstance ActiveSkillInstance { get; set; }
//         
//         public AttackStateParams(CharacterUnitController unitController, ActiveSkillInstance activeSkillInstance)
//         {
//             UnitController = unitController;
//             ActiveSkillInstance = activeSkillInstance;
//         }
//     }
//     
//     public class PlayerAttackSelectState : IState
//     {
//         private readonly PlayerInputProvider _inputProvider;
//         private readonly int _rayCastLayer;
//
//         private AttackStateParams _stateParams;
//         
//         public PlayerAttackSelectState(PlayerInputProvider inputProvider)
//         {
//             _inputProvider = inputProvider;
//             _rayCastLayer = inputProvider.groundLayer | inputProvider.entityLayer;
//         }
//         
//         public void OnEnter(IStateParams stateParams)
//         {
//             if (stateParams is not AttackStateParams args)
//                 throw new System.ArgumentException("Invalid state parameters for AttackState");
//             
//             // TODO 显示攻击范围
//             
//             UIManager.Instance.ShowPanel(PanelName.BattleSkillReleasePanel, OpenStrategy.ReplaceCurrent,
//                 new SkillReleaseConfirmPanelParams(args.ActiveSkillInstance));
//             
//             _stateParams = args;
//             _inputProvider.OnHandleRaycastInfo += HandleRaycastInfo;
//             EventBus.Channel(Channel.Battle).Subscribe<SkillReleaseConfirmInput>(GameEvent.SkillReleaseConfirmInput, HandleConfirmButtonClicked);
//             EventBus.Channel(Channel.Battle).Subscribe<CancelSkillReleaseInput>(GameEvent.CancelSkillSelectedInput, HandleCancelButtonClicked);
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
//             _inputProvider.OnHandleRaycastInfo -= HandleRaycastInfo;
//             EventBus.Channel(Channel.Battle).Unsubscribe<SkillReleaseConfirmInput>(GameEvent.SkillReleaseConfirmInput, HandleConfirmButtonClicked);
//             EventBus.Channel(Channel.Battle).Unsubscribe<CancelSkillReleaseInput>(GameEvent.CancelSkillSelectedInput, HandleCancelButtonClicked);
//             
//             UIManager.Instance.ClosePanel(PanelName.BattleSkillReleasePanel);
//         }
//
//         private void HandleRaycastInfo(RaycastHit2D hitInfo)
//         {
//             if (hitInfo.collider == null) return;
//             
//             var unit = hitInfo.collider.GetComponent<CharacterUnitController>();
//             if (unit == null) return;
//             var hasValidTarget = TryHandleObjectSelected(unit);
//             
//             EventBus.Channel(Channel.Battle).Publish(GameEvent.OnSkillTargetSelected, new SkillTargetSelectedUpdateUIEvent(hasValidTarget));
//         }
//         
//         private bool TryHandleObjectSelected(EntityController entity)
//         {
//             switch (entity)
//             {
//                 case CharacterUnitController target:
//                     // TODO 选中角色
//                     // TODO 处理选中对象
//                     // A：如果点击的是玩家角色优先切换角色
//                     // B: 如果点击的是敌人、障碍物
//                     // - 如果可以攻击(类型匹配且范围内)，进入[攻击确认状态](默认选中平A)
//                     // - 如果不可以攻击，则进入[选择状态] 并打开对应的查看界面
//                     var faction = target.CharacterRuntimeData.faction;
//                     switch (faction)
//                     {
//                         case Faction.Player:
//                         case Faction.Enemy:
//                             return true;
//                         default:
//                             Debug.LogError($"Invalid faction: {faction} for unit {target.gameObject.name}");
//                             break;
//                     }
//                     break;
//             }
//             return false;
//         }
//         
//         private void HandleCancelButtonClicked(CancelSkillReleaseInput cancel)
//         {
//             // TODO 取消攻击确认
//             _inputProvider.Fsm.ChangeState(InputState.SelectTarget);
//         }
//
//         private void HandleConfirmButtonClicked(SkillReleaseConfirmInput confirm)
//         {
//             // TODO 处理攻击确认
//             // TODO 查看选中角色能否继续行动，能则进入移动选择面板
//             var canAction = false;
//             if (!canAction)
//             {
//                 // 继续行动
//                 _inputProvider.Fsm.ChangeState(InputState.MoveAction, new MovementStateEnterParams(_stateParams.UnitController));
//             }
//             else
//             {
//                 _inputProvider.Fsm.ChangeState(InputState.SelectTarget);
//             }
//         }
//     }
// }