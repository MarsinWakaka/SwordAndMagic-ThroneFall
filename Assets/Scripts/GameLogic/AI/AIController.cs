using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events.Battle;
using GameLogic.GameAction.Attack;
using GameLogic.Grid;
using GameLogic.Skill.Active;
using GameLogic.TimeLine;
using GameLogic.Unit;
using GameLogic.Unit.ConfigData;
using GameLogic.Unit.Controller;
using MyFramework.Algorithm;
using MyFramework.Utilities;
using MyFramework.Utilities.Extensions;
using UnityEngine;

namespace GameLogic.AI
{
    public class AIController : MonoBehaviour
    {
        // private List<CharacterUnitController> _playableCharacters;
        public Faction canControlFaction = Faction.Enemy;
        
        private StartTurnEvent _startTurnEvent;
        
        private void OnEnable()
        {
            // 订阅事件
            EventBus.Channel(Channel.Gameplay).Subscribe<StartTurnEvent>(OnTurnStart);
        }
        
        private void OnDisable()
        {
            // 取消订阅事件
            EventBus.Channel(Channel.Gameplay).Unsubscribe<StartTurnEvent>(OnTurnStart);
        }

        private void OnTurnStart(StartTurnEvent turnEvent)
        {
            if (turnEvent.Faction != canControlFaction) return;
            _startTurnEvent = turnEvent;
            StartCoroutine(MakeDecision());
        }

        private IEnumerator MakeDecision()
        {
            // TODO : AI决策逻辑
            // 获取所有决策信息
            
            // 处理玩家回合开始事件
            var units = ServiceLocator.Resolve<IUnitManager>()
                .GetEntities<CharacterUnitController>(EntityType.Character);
            List<CharacterUnitController> playableCharacters = new();
            List<CharacterUnitController> hostileCharacters = new();
            // TODO 制作行动区域，热力图
            
            foreach (var unit in units)
            {
                if (unit.IsDead) continue;
                if (unit.CharacterRuntimeData.faction == canControlFaction)
                {
                    playableCharacters.Add(unit);
                }
                else
                {
                    hostileCharacters.Add(unit);
                }
            }
            
            if (playableCharacters.Count == 0)
            {
                $"[{canControlFaction} all dead]".LogWithColor(Color.green);
                EventBus.Channel(Channel.Gameplay).Publish(new FactionWipeEvent(canControlFaction));
                yield break;
            }
            
            foreach (var character in playableCharacters)
            {
                character.OnStartTurn();
            }

            var gridManager = ServiceLocator.Resolve<IGridManager>();

            foreach (var caster in playableCharacters)
            {
                var casterRtData = caster.CharacterRuntimeData;
                // TODO 基于效益函数进行决策
                // 遍历所有可用技能，根据优先级排序
                var canUseSKill = caster.CharacterRuntimeData.ActiveSkills
                    .Where(skill => ActiveSkillUtil.CanUseSkill(skill, caster.CharacterRuntimeData))
                    .OrderByDescending(skill => skill.ActiveConfig.priority);
                
                var hasMadeDecision = false;
                var moveableArea = gridManager.GetAllMoveableArea(casterRtData.gridCoord, casterRtData.CurMoveRange.Value);
                foreach (var skill in canUseSKill)
                {
                    if (hasMadeDecision) break;
                    switch (skill.ActiveConfig.canImpactTarget)
                    {
                        case SkillCanImpactTarget.Enemy:
                            // 根据角色的距离排序
                            var targets = hostileCharacters
                                .OrderBy(target => caster.GetCoordinate().ManhattanDistance(target.GetCoordinate()));
                            
                            // 就近选择目标
                            foreach (var target in targets )
                            {
                                if (target.IsDead) continue;
                                
                                // 判断目标可达？
                                if (caster.CharacterRuntimeData.CurMoveRange.Value + skill.GetAttackRange().y 
                                    < target.GetCoordinate().ManhattanDistance(caster.GetCoordinate()))
                                {
                                    continue;
                                }

                                if (!AttackUtil.TryCalculateAttackPosition(
                                    moveableArea,
                                    skill.ActiveConfig,
                                    target.GetCoordinate(),
                                    out var attackPosition))
                                {
                                    continue;
                                }
                                
                                // 移动到攻击位置
                                caster.Teleport(attackPosition);
                                
                                // 选择目标
                                var grid = gridManager.GetGridController(target.CharacterRuntimeData.gridCoord);
                                var selectContext = new ActiveSkillSelectContext(caster, grid, ActionType.Active);
                                
                                // 选择目标成功，执行技能
                                var executeContext = new ActiveSkillExecuteContextWrapper(selectContext, skill.Execute);
                                TimeLineManager.Instance.AddPerform(executeContext.Execute);
                                // 记录决策
                                hasMadeDecision = true;
                                break;
                            }
                            break;
                        default:
                            Debug.LogWarning($"技能类型不支持: {skill.ActiveConfig.canImpactTarget}");
                            break;
                    }
                }

                if (!hasMadeDecision)
                {
                    // 则找到最近的敌人，移动到敌人身边
                    
                }
            }
            
            foreach (var character in playableCharacters)
            {
                character.OnEndAction();
            }
            
            // TODO 结束回调添加至时间线
            TimeLineManager.Instance.AddPerform(EndDecision);
            
            yield return null;
        }
        
        private IEnumerator EndDecision()
        {
            // TODO : AI决策结束逻辑
            _startTurnEvent.Complete();
            // 如何理解Yield
            yield return null;
        }
    }
}