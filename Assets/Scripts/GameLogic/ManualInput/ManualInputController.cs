using System;
using System.Collections;
using System.Collections.Generic;
using Events.Battle;
using GameLogic.Grid;
using GameLogic.ManualInput.Concrete;
using GameLogic.TimeLine;
using GameLogic.Unit;
using GameLogic.Unit.BattleRuntimeData;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using MyFramework.Utilities.Extensions;
using MyFramework.Utilities.Stack;
using UnityEngine;

namespace GameLogic.ManualInput
{
    public enum InputState
    {
        Idle,
        MovementSelect,
        AttackSelect,
    }
    
    public class ManualInputController : MonoBehaviour
    {
        public Faction canControlFaction;
        private List<CharacterUnitController> _playableCharacters;
        private Camera _viewCamera;
        public LayerMask groundLayer;
        public LayerMask entityLayer;
        
        private StartTurnEvent _curTurnEvent;

        private void Awake()
        {
            _viewCamera = Camera.main;
            StackManager = new PersistentStackManager<InputState>();
            StackManager.RegisterStackNode(InputState.Idle, new IdleState(this));
            StackManager.RegisterStackNode(InputState.MovementSelect, new CharacterActionState(this));
            StackManager.RegisterStackNode(InputState.AttackSelect, new AttackSelectState(this));
            
            HideGhostAvatar();
        }
        
        private void Update()
        {
            if (EnableMouseHoverStyle) DrawMouseHoverStyleOnGrid();
            if (Input.GetMouseButtonDown(0)) Raycast();
            // StackManager.OnUpdateTop();
        }

        #region 事件处理
        
        public PersistentStackManager<InputState> StackManager { get; private set; }
        public event Action<RaycastHit2D> OnHandleRaycastInfo;
        public event Action OnCharacterActionEndConfirm; 
        
        private void OnEnable()
        {
            EventCenter.Register<StartTurnEvent>(HandleTurnStartEvent);
            // 注册玩家输入事件
            EventBus.Channel(Channel.Gameplay).Subscribe<StartTurnEvent>(HandleTurnStartEvent);
            EventBus.Channel(Channel.Gameplay).Subscribe<ActionEndEvent>(HandleCharacterActionEndEvent);
        }

        private void OnDisable()
        {
            // 注销玩家输入事件
            EventBus.Channel(Channel.Gameplay).Unsubscribe<ActionEndEvent>(HandleCharacterActionEndEvent);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<StartTurnEvent>(HandleTurnStartEvent);
        }
        
        private void HandleTurnStartEvent(StartTurnEvent startTurnEvent)
        {
            if (startTurnEvent.Faction != canControlFaction) return;
            _curTurnEvent = startTurnEvent;
            
            // 处理玩家回合开始事件
            _playableCharacters = ServiceLocator.Resolve<ICharacterManager>().GetEntities(canControlFaction);
            // _playableCharacters = units.Where(unit => unit.CharacterRuntimeData.faction == canControlFaction).ToList();

            if (_playableCharacters.Count == 0)
            {
                $"[{canControlFaction} all dead]".LogWithColor(Color.green);
                EventBus.Channel(Channel.Gameplay).Publish(new FactionWipeEvent(canControlFaction));
                // _curTurnEvent.Complete();
            }
            else
            {
                foreach (var character in _playableCharacters)
                {
                    character.OnStartTurn();
                }
                StackManager.Push(InputState.Idle);
                $"[{canControlFaction} Turn]: {_playableCharacters.Count} characters can Action".LogWithColor(Color.green);
            }
        }
        
        /// <summary>
        /// 响应角色行动结束事件，合适时机结束回合
        /// </summary>
        private void HandleCharacterActionEndEvent(ActionEndEvent args)
        {
            OnCharacterAction(args.CharacterRTData);
        }
        
        public void OnCharacterAction(CharacterBattleRuntimeData rtData)
        {
            // 处理角色行动结束事件
            if (rtData.faction != canControlFaction) return;
            var character = _playableCharacters.Find(c =>
                c.CharacterRuntimeData == rtData);
            character.OnEndAction();
            _playableCharacters.Remove(character);
            
            OnCharacterActionEndConfirm?.Invoke();
            StackManager.PopAll();
            
            $"[{rtData.ConfigData.name} action end, remain {_playableCharacters.Count} characters can Action".LogWithColor(Color.white);
            if (_playableCharacters.Count == 0)
            {
                ReadyToComplete();
            }
            else
            {
                StackManager.Push(InputState.Idle);
            }
        }

        private void Raycast()
        {
            var hitInfo = Physics2D.Raycast(
                _viewCamera.ScreenToWorldPoint(Input.mousePosition), 
                default, 
                100f, 
                groundLayer );
            OnHandleRaycastInfo?.Invoke(hitInfo);
        }

        #endregion
        
        private GridController _curPointGrid;
        public GridController CurPointGrid
        {
            get => _curPointGrid;
            set
            {
                _curPointGrid?.OnCancelMouseHover();
                _curPointGrid = value;
                _curPointGrid?.OnMouseHover();
            }
        }
        
        private bool _enableMouseHoverStyle;
        public bool EnableMouseHoverStyle 
        {
            get => _enableMouseHoverStyle;
            set
            {
                _enableMouseHoverStyle = value;
                if (value) return;
                CurPointGrid = null;
            }
        }

        private void DrawMouseHoverStyleOnGrid()
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
            // if (grid == null)
            // {
            //     // 如果是实体，则通过实体获取到格子
            //     var entity = hitInfo.collider.GetComponent<EntityController>();
            //     grid = _gridManagerCache.GetGridController(entity.RuntimeData.gridCoord);
            // }
            CurPointGrid = grid;
        }
        
        [SerializeField] private Animator ghostAnimator;
        
        public void SetGhostSprite(Animator animator, Vector3Int gridCoord) 
        {
            if (ghostAnimator == null || animator == null) 
            {
                Debug.LogError("Animator 未初始化！", this);
                return;
            }

            // 重置动画状态
            ghostAnimator.Rebind();
            ghostAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
            ghostAnimator.Update(0); // 立即应用第一帧

            // 设置位置
            ghostAnimator.transform.position = CoordinateConverter.CoordToWorldPos(gridCoord);
            ghostAnimator.enabled = true;
        }

        public void HideGhostAvatar() 
        {
            if (ghostAnimator == null) return;

            ghostAnimator.runtimeAnimatorController = null;
            ghostAnimator.enabled = false;
        }

        private void ReadyToComplete()
        {
            StartCoroutine(WaitForComplete());
        }
        
        private IEnumerator WaitForComplete()
        {
            yield return new WaitUntil(() => TimeLineManager.Instance.IsRunning == false);
            // TODO 结束回合
            _curTurnEvent.Complete();
        }
    }
}