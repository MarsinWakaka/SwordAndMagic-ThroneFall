using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Core.Log;
using DG.Tweening;
using Events.Battle;
using GameLogic.GameAction.Attack;
using GameLogic.Grid;
using GameLogic.Unit.BattleRuntimeData;
using MyFramework.Utilities;
using MyFramework.Utilities.Extensions;
using UnityEngine;

namespace GameLogic.Unit.Controller
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AnimationType
    {
        public const string Default = "Attack_0";
        public const string Idle = "Idle";
        public const string Move = "Move";
        public const string Hurt = "Hurt";
        public const string Dead = "Dead";
        public const string Attack_0 = "Attack_0";
        public const string Skill_0 = "Skill_0";
        public const string Skill_1 = "Skill_1";
        public const string Skill_2 = "Skill_2";
    }
    
    public class CharacterUnitController : EntityController
    {
        private static readonly int Default = Animator.StringToHash(AnimationType.Default);
        public CharacterBattleRuntimeData CharacterRuntimeData { get; private set; }
        public bool IsDead => CharacterRuntimeData.CurHp.Value <= 0;

        public void Initialize(CharacterBattleRuntimeData runtimeData)
        {
            base.Initialize(runtimeData);
            CharacterRuntimeData = runtimeData;
            OnCharacterInitialize();
            var statusBar = GetComponentInChildren<StatusBar>();
            if (statusBar == null)
            {
                Debug.LogError("Status Bar is Null");
            }
            else
            {
                statusBar.Initialize(runtimeData);
            }
            EventBus.Channel(Channel.Gameplay).Publish(new CharacterSpawnedEvent(this));
        }

        /// <summary>
        /// 角色生成初始化函数，应当在战斗开始时第一回合时调用
        /// </summary>
        public void OnCharacterInitialize()
        {
            // TODO 移到战斗回合开始后才初始化
            CharacterRuntimeData.InitializeSkill(this);
        }

        /// <summary>
        /// 处理当角色回合开始时，自身状态的更新
        /// </summary>
        public void OnStartTurn()
        {
            CharacterRuntimeData.BuffManager.OnTurnStart();
            foreach (var activeSkillInstance in CharacterRuntimeData.ActiveSkills)
            {
                activeSkillInstance.OnTurnStart();
            }
            foreach (var passiveSkillInstance in CharacterRuntimeData.PassiveSkills)
            {
                passiveSkillInstance.OnTurnStart();
            }
            CharacterRuntimeData.CurMoveRange.Value = CharacterRuntimeData.MaxMoveRange.Value;
            CharacterRuntimeData.CanAction.Value = true;
        }

        // TODO : 技能释放完毕，和玩家控制器退出 => End Action
        public void OnEndAction()
        {
            CharacterRuntimeData.BuffManager.OnTurnEnd();
            CharacterRuntimeData.CanAction.Value = false;
            CharacterRuntimeData.CurMoveRange.Value = 0;
        }

        public IEnumerator Move(Vector2Int newGridCoord)
        {
            // var triggerService = ServiceLocator.Resolve<ITriggerService>();
            var oldGridCoord = CharacterRuntimeData.gridCoord;
            
            // 查询触发器服务，触发关于离开区域的触发器
            // yield return triggerService.HandleCharacterLeaveArea(this, oldGridCoord);
            if (IsDead) yield break;
            
            // 移动单位, 并通知位置变化
            var height = ServiceLocator.Resolve<IGridManager>().QueryHeight(newGridCoord.x, newGridCoord.y);
            Trans.position = CoordinateConverter.CoordToWorldPos(new Vector3Int(newGridCoord.x, newGridCoord.y, height));
            EventBus.Channel(Channel.Gameplay).Publish(new UnitMoveEvent(this, oldGridCoord, newGridCoord));
            
            // 查询触发器服务，触发关于进入区域的触发器
            // yield return triggerService.HandleCharacterEnterArea(this, newGridCoord);
            yield return null;
        }

        public void OnCharacterHurt()
        {
            // TODO 播放受伤动画
            // TODO 播放受伤音效
            // TODO 播放受伤特效
            // TODO 播放受伤UI
            PlayAnimation(AnimationType.Hurt);
        }

        public void OnCharacterDead()
        {
            // TODO 取消自身在管理器里注册的全部信息
            CharacterRuntimeData.BuffManager.RemoveAll();
            EventBus.Channel(Channel.Gameplay).Publish(new CharacterDespawnEvent(this));
            BattleLogManager.Instance.Log($"{FriendlyInstanceID()}死了。");
            PlayAnimation(AnimationType.Dead);
            Destroy(gameObject, 1.8f);
        }
        
        public CharacterUnitController GetAvatar()
        {
            var avatar = Instantiate(this, transform.position, Quaternion.identity);
            avatar.Renderer.color = new Color(0.4f, 0.4f, 1, 0.5f);
            return avatar;
        }

        /// <summary>
        /// 受伤函数
        /// </summary>
        /// <param name="damageSegment">伤害分段数据</param>
        public override void TakeDamage(DamageSegment damageSegment)
        {
            if (IsDead) return;
            var damage = damageSegment.Damage;
            CharacterRuntimeData.CurHp.Value -= damage;
            
            if (CharacterRuntimeData.CurHp.Value <= 0)
            {
                OnCharacterDead();
            }
            else
            {
                OnCharacterHurt();
            }
        }
        
        public void PlayAnimation(string animName)
        {
            try
            {
                switch (animName)
                {
                    case AnimationType.Idle:
                    case AnimationType.Attack_0:
                        if (Anim == null)
                        {
                            Debug.LogError("Animator is null");
                            Anim = GetComponent<Animator>();
                            Debug.Log("Animator is null, try to get it again, result : " + (Anim == null ? "Null" : Anim));
                            return;
                        }
                        Anim.Play(animName);
                        break;
                    case AnimationType.Hurt:
                        // 使用DoTween播放受伤动画
                        var hurtSequence = DOTween.Sequence();
                        hurtSequence.Append(Renderer.DOColor(new Color(1, 0.3f, 0.3f), 0.2f));
                        hurtSequence.AppendInterval(0.2f);
                        hurtSequence.Append(Renderer.DOColor(Color.white, 0.4f));
                        hurtSequence.Play();
                        break;
                    case AnimationType.Dead:
                        // 使用DoTween播放受伤动画
                        var deadSequence = DOTween.Sequence();
                        deadSequence.Append(Renderer.DOColor(new Color(1, 0.3f, 0.3f), 0.2f));
                        deadSequence.AppendInterval(0.2f);
                        deadSequence.Append(Renderer.DOColor(Color.black, 1f));
                        deadSequence.Play();
                        break;
                    default:
                        Debug.LogWarning($"Animation {animName} not found in Animator.");
                        Anim.Play(AnimationType.Default);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"PlayAnimation Error: {e.Message}");
            }
        }

#if UNITY_EDITOR
        #region DEBUG

        [ContextMenu("ShowRuntimeData")]
        private void ShowRuntimeData()
        {
            CharacterRuntimeData.ToString().LogWithColor(Color.white);
        }
        #endregion
#endif
        public override string FriendlyInstanceID()
        {
            return $"{CharacterRuntimeData.faction}-{CharacterRuntimeData.EntityID}-{CharacterRuntimeData.InstanceID[..3]}";
        }
    }
}