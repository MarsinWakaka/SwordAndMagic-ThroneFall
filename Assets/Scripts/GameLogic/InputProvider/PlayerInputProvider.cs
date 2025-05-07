// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Events;
// using Events.Battle;
// using GameLogic.Grid;
// using GameLogic.InputProvider.PlayerInputState;
// using GameLogic.Unit;
// using GameLogic.Unit.ConfigData;
// using GameLogic.Unit.Controller;
// using MyFramework.Utilities;
// using MyFramework.Utilities.FSM;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// namespace GameLogic.InputProvider
// {
//     public enum InputState
//     {
//         Idle,
//         SelectTarget,
//         MoveAction,
//         AttackSelect,
//     }
//     
//     public class PlayerInputProvider : MonoBehaviour
//     {
//         [SerializeField] private List<CharacterUnitController> playableCharacters;
//         private Camera _viewCamera;
//         
//         public LayerMask groundLayer;
//         public LayerMask entityLayer;
//         
//         private void Awake()
//         {
//             _viewCamera = Camera.main;
//             Fsm = new Fsm<InputState>(InputState.Idle);
//             Fsm.AddState(InputState.SelectTarget, new PlayerSelectTargetState(this));
//             Fsm.AddState(InputState.MoveAction, new PlayerMovementSelectState(this));
//             Fsm.AddState(InputState.AttackSelect, new PlayerAttackSelectState(this));
//         }
//
//         private void OnEnable()
//         {
//             // 注册玩家输入事件
//             EventBus.Channel(Channel.Battle).Subscribe<StartTurnEvent>(GameEvent.OnTurnStart, HandleTurnStartEvent);
//             EventBus.Channel(Channel.Battle).Subscribe<ActionEndEvent>(GameEvent.OnActionEnd, HandleCharacterActionEnd);
//         }
//
//         private void OnDisable()
//         {
//             // 注销玩家输入事件
//             EventBus.Channel(Channel.Battle).Unsubscribe<StartTurnEvent>(GameEvent.OnTurnStart, HandleTurnStartEvent);
//             EventBus.Channel(Channel.Battle).Unsubscribe<ActionEndEvent>(GameEvent.OnActionEnd, HandleCharacterActionEnd);
//         }
//
//         private void Update()
//         {
//             // HandleTest();
//             RaycastTarget();
//             Fsm.UpdateFsm();
//         }
//         private const int RaycastDistance = 100;
//
//         #region 状态机共享数据
//
//         [NonSerialized] public Fsm<InputState> Fsm;
//         [NonSerialized] public GridController SelectedGrid;
//         [NonSerialized] public GridController CurHoverGrid;
//         [NonSerialized] public readonly RaycastHit2D[] RaycastResults = new RaycastHit2D[10];
//         [NonSerialized] public int ResultCount;
//         private int _curIndex;
//         
//         public Action<RaycastHit2D> OnHandleRaycastInfo;
//         
//         #endregion
//         
//         // 选择状态时启用，包括移动选择和攻击选择
//         public void ShowMouseHoverStyleOnGrid()
//         {
//             // TODO 显示鼠标悬停在格子上时的样式
//             var hitInfo = Physics2D.Raycast(
//                 _viewCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 
//                 100f, groundLayer);
//             if (hitInfo.collider != null)
//             {
//                 var gridCtrl = hitInfo.collider.GetComponent<GridController>();
//                 if (gridCtrl != null)
//                 {
//                     CurHoverGrid?.OnCancelMouseHover();
//                     CurHoverGrid = gridCtrl;
//                     // TODO 显示鼠标悬停在格子上时的样式
//                     CurHoverGrid.OnMouseHover();
//                 }
//             }
//             else
//             {
//                 // TODO 清空鼠标悬停在格子上时的样式
//                 CurHoverGrid?.OnCancelMouseHover();
//             }
//         }
//         
//         public void HandleMouseScroll()
//         {
//             // 滚轮切换选择对象
//             if (Input.mouseScrollDelta.y != 0)
//             {
//                 if (ResultCount <= 0) return;
//                 _curIndex += (int)Input.mouseScrollDelta.y > 0 ? 1 : -1;
//                 _curIndex = Mathf.Clamp(_curIndex, 0, ResultCount - 1);
//                 OnHandleRaycastInfo?.Invoke(RaycastResults[_curIndex]);
//             }
//         }
//         
//         public void RayCastAll(int layerMask)
//         {
//             // 1. 将鼠标屏幕坐标转换为世界坐标
//             Vector2 mouseWorldPos = _viewCamera.ScreenToWorldPoint(Input.mousePosition);
//             
//             // 2. 定义射线方向（从鼠标位置沿摄像机前方发射）
//             Vector2 rayDirection = _viewCamera.transform.forward; // 2D中通常用 Vector2.right 或自定义方向
//             
//             // 3. 执行射线检测
//             ResultCount = Physics2D.RaycastNonAlloc(
//                 origin: mouseWorldPos,
//                 direction: rayDirection,
//                 results: RaycastResults,
//                 distance: RaycastDistance,
//                 layerMask: layerMask
//             );
//
//             // 4. 调试绘制射线（可视化）
//             Debug.DrawRay(mouseWorldPos, rayDirection * RaycastDistance, Color.red, 1f);
//                 
//             if (ResultCount <= 0) return;
//             _curIndex = 0;
//             OnHandleRaycastInfo?.Invoke(RaycastResults[_curIndex]);
//         }
//         
//         private void RaycastTarget()
//         {
//             // [选择状态] (允许碰撞图层为ground, entity)
//             // 不启用鼠标悬浮预览效果
//             // 如果是地格则选中地格(状态不变)
//             // 如果是玩家角色则选中玩家并进入[玩家操作状态]
//             // 如果是敌方角色、障碍物则查看敌方角色的移动范围
//                 
//             // [移动选择状态] (允许碰撞图层为ground, entity)
//             // 启用鼠标悬浮预览效果
//             // A:如果点击的是玩家角色优先切换角色
//             // B:如果点击的是敌人、障碍物
//             // - 如果可以攻击(类型匹配且范围内)，进入[攻击确认状态](默认选中平A)
//             // - 如果不可以攻击，则进入[选择状态] 并打开对应的查看界面
//             // C:地块上是否有物体
//             // - - 如果有，依次进行AB判断
//             // - - 如果没有，则判断是否位于角色移动范围内
//             // - - - 如果成立，则预览移动效果
//             // - - - 如果不成立，则进入[选择状态]
//             // [退出] 清空角色显示
//             
//             // [攻击选择状态] (允许碰撞图层为ground, entity)
//             // 启用鼠标悬浮预览效果
//             // 如果选中的物体位置位于攻击范围内，则进行攻击类型以及阵营判断
//             // - 如果符合类型以及阵营，则进入攻击确认状态
//             // - 如果不符合类型或者阵营，则是简单显示地块选中
//             // 玩家点击取消按钮时，退出该状态
//                 
//             // [攻击确认状态]
//             // 预览本次攻击效果并进入攻击
//             // - 如果点击确认，则执行指令。
//             // - 如果点击取消，则退出该状态
//         }
//
//         #region 测试
//
// #if TEST
//
//         private void HandleTest()
//         {
//             // 处理玩家输入
//             if (playableCharacters.Count == 0) return;
//             if (Input.GetKeyDown(KeyCode.E))
//             {
//                 _selectIndex = (_selectIndex + 1) % playableCharacters.Count;
//                 Debug.Log($"Selected character: {playableCharacters[_selectIndex].EntityData.StaticData.name}");
//             }
//             else if (Input.GetKeyDown(KeyCode.Q))
//             {
//                 // 选择上一个角色
//                 _selectIndex = (_selectIndex - 1 + playableCharacters.Count) % playableCharacters.Count;
//                 Debug.Log($"Selected character: {playableCharacters[_selectIndex].EntityData.StaticData.name}");
//             }
//             
//             var moveLeft = Input.GetKeyDown(KeyCode.A);
//             var moveRight = Input.GetKeyDown(KeyCode.D);
//             var moveUp = Input.GetKeyDown(KeyCode.W);
//             var moveDown = Input.GetKeyDown(KeyCode.S);
//             
//             if (moveLeft || moveRight || moveUp || moveDown)
//             {
//                 Vector2Int direction;
//                 if (moveLeft) direction = Vector2Int.left;
//                 else if (moveRight) direction = Vector2Int.right;
//                 else if (moveUp) direction = Vector2Int.up;
//                 else direction = Vector2Int.down;
//
//                 var character = playableCharacters[_selectIndex];
//                 var gridCoord = character.EntityData.gridCoord;
//                 character.Teleport(gridCoord + direction);
//             }
//         }
// #endif
//         #endregion
//         
//         private StartTurnEvent _startTurnEvent;
//         
//         private void HandleTurnStartEvent(StartTurnEvent args)
//         {
//             if (args.Faction != Faction.Player) return;
//             _startTurnEvent = args;
//             
//             // 处理玩家回合开始事件
//             var units = ServiceLocator.Resolve<IUnitDataProvider>().GetEntities<CharacterUnitController>(EntityType.Character);
//             playableCharacters = units.Where(unit => unit.CharacterRuntimeData.faction == Faction.Player).ToList();
//             
//             foreach (var character in playableCharacters)
//             {
//                 // 刷新角色资源
//                 character.OnStartAction();
//             }
//             
//             Fsm.ChangeState(InputState.SelectTarget);
//         }
//
//         /// <summary>
//         /// 响应角色行动结束事件，合适时机结束回合
//         /// </summary>
//         private void HandleCharacterActionEnd(ActionEndEvent args)
//         {
//             // 处理角色行动结束事件
//             if (args.CharacterRTData.faction != Faction.Player) return;
//             var character = playableCharacters.Find(c =>
//                 c.CharacterRuntimeData == args.CharacterRTData);
//             character.OnEndAction();
//             playableCharacters.Remove(character);
//
//             if (playableCharacters.Count == 0) _startTurnEvent.Complete();
//             
//             Fsm.ChangeState(InputState.SelectTarget);
//         }
//     }
// }