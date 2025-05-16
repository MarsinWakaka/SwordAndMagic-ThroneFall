using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events.Battle;
using GameLogic.GameAction.Attack;
using GameLogic.Grid;
using GameLogic.Skill.Active;
using GameLogic.TimeLine;
using GameLogic.Unit;
using MyFramework.Utilities;
using MyFramework.Utilities.Extensions;
using UnityEngine;

namespace GameLogic.AI
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private Faction canControlFaction;
        
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
            var playableCharacters = ServiceLocator.Resolve<ICharacterManager>().GetEntities(canControlFaction);
            var hostileCharacters = ServiceLocator.Resolve<ICharacterManager>().GetEntities(canControlFaction.Opposite());
            // TODO 制作行动区域，热力图
            
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
                var casterCoord = caster.Coordinate();
                // TODO 基于效益函数进行决策
                // 遍历所有可用技能，根据优先级排序
                var canUseSKill = caster.CharacterRuntimeData.ActiveSkills
                    .Where(skill => ActiveSkillUtil.CanUseSkill(skill, caster.CharacterRuntimeData))
                    .OrderByDescending(skill => skill.ActiveConfig.priority).ToList();
                
                var hasMadeDecision = false;
                // var moveableArea = gridManager.CalculateAllMoveablePath(casterRtData.gridCoord, casterRtData.CurMoveRange.Value);
                // // 获取坐标与移动节点的字典
                // var lookUpDict = new Dictionary<Vector2Int, PathTreeNode>(moveableArea.Count);
                // foreach (var node in moveableArea)
                // {
                //     lookUpDict[node.Coord] = node;
                // }
                
                // 根据角色的距离排序
                var hostiles = hostileCharacters
                    .OrderBy(target => casterCoord.ManhattanDistance(target.Coordinate()))
                    .ToList();
                
                foreach (var skill in canUseSKill)
                {
                    if (hasMadeDecision) break;
                    switch (skill.ActiveConfig.canImpactTarget)
                    {
                        case SkillCanImpactTarget.Enemy:
                            
                            // 就近选择目标
                            foreach (var target in hostiles )
                            {
                                if (target.IsDead) continue;
                                
                                // 判断目标可达？
                                var targetCoord = target.Coordinate();
                                
                                if (caster.CharacterRuntimeData.CurMoveRange.Value + skill.GetAttackRange().y 
                                    < casterCoord.ManhattanDistance(targetCoord))
                                {
                                    continue;
                                }
                                
                                // if (!AttackUtil.TryCalculateAttackPosition(
                                //         moveableArea,
                                //         skill.ActiveConfig,
                                //         target.Coordinate(),
                                //         out var attackPosition))
                                // {
                                //     continue;
                                // }
                                // var path = lookUpDict[attackPosition].ToPathWayList();
                                
                                var path = gridManager.TryFindPathToCloseTarget(casterCoord, targetCoord,
                                    skill.GetAttackRange().y);
                                if (path == null) continue; // 不可达
                                // 如果距离过远，则跳过
                                if (path.Count > casterRtData.CurMoveRange.Value) continue;

                                if (path.Count > 0)
                                {
                                    // 移动到攻击位置
                                    TimeLineManager.Instance.AddPerform(new MoveWrapper(caster, path));
                                    yield return new WaitUntil(() => !TimeLineManager.Instance.IsRunning);
                                }
                                
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

                // 如果没有决策，则进行默认决策：靠近敌人
                if (!hasMadeDecision)
                {
                    Debug.Log($"[{caster.FriendlyInstanceID()}] 没有决策，进行默认决策");
                    // 当前最短距离的中间节点个数(也就是不包含起始点和结束点的个数)
                    int minDist = int.MaxValue;
                    List<Vector2Int> minPathList = null;
                    
                    // TODO 改善算法，找到真正最近的敌人，移动到敌人身边; 例如可以对每个对象使用A*求出最短的那个
                    // 计算当前决策者到所有敌人的最大预估距离(也就是曼哈顿距离)，如果预估距离大于当前最小路径，则不进行进行寻路，跳出循环
                    foreach (var hostile in hostiles)
                    {
                        if (hostile.IsDead) continue;
                        var intervalDist = caster.Coordinate().ManhattanDistance(hostile.Coordinate()) - 1;
                        if (intervalDist > minDist) break;
                        // 计算路径，尝试到目标旁边一格距离。
                        var path = gridManager.TryFindPathToCloseTarget(caster.Coordinate(), hostile.Coordinate(), 1);
                        
                        if (path == null) continue;
                        if (path.Count == 0)
                        {
                            Debug.Log($"[{caster.FriendlyInstanceID()}] 无需移动");
                            break;
                        }
                        
#if GRID_DEBUG
                        // 打印路径
                        var stringBuilder = new System.Text.StringBuilder();
                        stringBuilder.Append(casterCoord);
                        foreach (var node in path)
                        {
                            stringBuilder.Append($" -> {node}");
                        }
                        stringBuilder.Append($"[{path.Count}]");
                        Debug.Log($"[{caster.FriendlyInstanceID()} 路径个数为7: {stringBuilder}");
#endif
                        if (path.Count < minDist)
                        {
                            minDist = path.Count;
                            minPathList = path;
                            Debug.Log($"[{caster.FriendlyInstanceID()}] 找到新的最短路径: {minPathList.Count}");
                        }
                    }

                    if (minPathList == null)
                    {
                        Debug.Log($"[{caster.FriendlyInstanceID()}] 没有可移动的路径");
                        continue;
                    }

                    if (minPathList.Count > casterRtData.CurMoveRange.Value)
                        minPathList = minPathList.GetRange(0, casterRtData.CurMoveRange.Value);
                    TimeLineManager.Instance.AddPerform(new MoveWrapper(caster, minPathList));
                }
                
                // 等待动作队列结束
                yield return new WaitUntil(() => !TimeLineManager.Instance.IsRunning);
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