using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using Core.Log;
using GameLogic.GameAction.Attack;
using GameLogic.Grid;
using GameLogic.Unit.BattleRuntimeData;
using GameLogic.Unit.Controller;
using MyFramework.Utilities.Extensions;
using UnityEngine;
using Utilities;

// ReSharper disable InconsistentNaming

namespace GameLogic.Skill.Active
{
    [Flags]
    public enum SkillCanImpactTarget
    {
        None = 0,
        Self = 1,
        Ally = 2,
        Enemy = 4,
        Object = 8,
        All = Self | Ally | Enemy | Object
    }

    [Serializable]
    public class ActiveSKillData
    {
        public int coolDownTime;
        public int skillPointCost;
    }
    
    public abstract class ActiveSkillConfig : BaseSkillConfig
    {
        public abstract ActiveSKillData GetActiveSKillData(int level);

        /// 攻击范围[minAttackRange, maxAttackRange]
        public Vector2Int attackRange;
        /// 攻击范围内允许的高度差
        public Vector2Int allowHeightDiff;
        
        [Tooltip("可作用的实体类型")]
        public SkillCanImpactTarget canImpactTarget;

        /// <summary>
        /// 用于影响AI决策的技能优先级
        /// </summary>
        public int priority;
        /// 施法范围 
        public Vector2Int[] upDirScope = { new(0,0) };
        
        public Vector2Int[] GetSkillScope(Vector2Int targetCoord, Direction direction)
        {
            var skillScope = new Vector2Int[upDirScope.Length];
            switch (direction)
            {
                case Direction.Up:
                    for (var i = 0; i < skillScope.Length; i++)
                    {
                        skillScope[i] = targetCoord + upDirScope[i];
                    }
                    break;
                case Direction.Down:
                    for (var i = 0; i < skillScope.Length; i++)
                    {
                        skillScope[i] = targetCoord + upDirScope[i].Rotate180Degree();
                    }
                    break;
                case Direction.Left:
                    for (var i = 0; i < skillScope.Length; i++)
                    {
                        skillScope[i] = targetCoord + upDirScope[i].Rotate90Degree(false);
                    }
                    break;
                case Direction.Right:
                    for (var i = 0; i < skillScope.Length; i++)
                    {
                        skillScope[i] = targetCoord + upDirScope[i].Rotate90Degree(true);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            return skillScope;
        }
        
        /// <summary>
        /// 获取技能施法范围
        /// </summary>
        /// <param name="targetCoord"></param>
        /// <param name="direction"></param>
        /// <param name="CoordFilter">限制函数，会过滤掉返回值为false的坐标，返回规则使用者自定义</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<Vector2Int> GetSkillScope(Vector2Int targetCoord, Direction direction, Func<Vector2Int, bool> CoordFilter)
        {
            if (CoordFilter == null)
            {
                Debug.LogWarning("restrictFunc is null， return default");
                return GetSkillScope(targetCoord, direction).ToList();
            }
            var skillScope = new List<Vector2Int>();
            for (var i = 0; i < upDirScope.Length; i++)
            {
                Vector2Int newPos = direction switch
                {
                    Direction.Up => targetCoord + upDirScope[i],
                    Direction.Down => targetCoord + upDirScope[i].Rotate180Degree(),
                    Direction.Left => targetCoord + upDirScope[i].Rotate90Degree(false),
                    Direction.Right => targetCoord + upDirScope[i].Rotate90Degree(true),
                    _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                };
                if (CoordFilter(newPos)) skillScope.Add(newPos);
            }
            return skillScope;
        }

        public bool IsTargetTypeValid(int level, ActiveSkillSelectContext selectContext)
        {
            // var canImpactTarget = GetActiveSKillData(level).canImpactTarget;
            if (canImpactTarget == SkillCanImpactTarget.All) return true;
            // 处理角色选取
            var entity = selectContext.Grid.RuntimeData.EntitiesOnThis;
            if (entity is CharacterUnitController targetCharacter)
            {
                var casterFaction = selectContext.Caster.CharacterRuntimeData.faction;
                var targetFaction = targetCharacter.CharacterRuntimeData.faction;
                switch (canImpactTarget)
                {
                    case SkillCanImpactTarget.Ally:
                        return casterFaction == targetFaction;
                    case SkillCanImpactTarget.Enemy:
                        return casterFaction != targetFaction;
                }
            }
            // TODO 添加对其它的支持
            return false;
        }

        public abstract ActiveSkillInstance CreateActiveSkillInstance(int level);
    }
    
    [Serializable]
    public abstract class ActiveSkillInstance : BaseSkillInstance
    {
        public int maxCooldown;
        public int remainCooldown;

        public ActiveSKillData SKillData { get; private set; }

        public override BaseSkillConfig BaseConfig => ActiveConfig;
        public ActiveSkillConfig ActiveConfig => ActiveSkillConfigManager.Instance.GetConfig(skillID);

        protected ActiveSkillInstance(string skillId, int level)
            : base(skillId, level)
        {
            remainCooldown = 0;
            SKillData = ActiveSkillConfigManager.Instance.GetConfig(skillId).GetActiveSKillData(skillLevel);
        }
        
        public override void TakeEffect(CharacterUnitController owner)
        {
            base.TakeEffect(owner);
            maxCooldown = SKillData.coolDownTime;
            remainCooldown = 0;
        }
        
        public virtual IEnumerator Execute(ActiveSkillSelectContext context)
        {
            BattleLogManager.Instance.Log(
                $"[{context.ActionType}]\n" +
                $"{context.Caster.FriendlyInstanceID()} 瞄准 {context.Grid.GetGridCoord()} 释放了技能 [{skillID}];");
            yield return null;  
        }
        
        public bool IsTargetTypeValid(ActiveSkillSelectContext selectContext)
        {
            return ActiveConfig.IsTargetTypeValid(skillLevel, selectContext);
        }
        
        public bool IsCooldown()
        {
            return remainCooldown > 0;
        }
        
        public bool IsSkillPointEnough(CharacterBattleRuntimeData runtimeData)
        {
            return runtimeData.CurSkillPoint.Value >= SKillData.skillPointCost;
        }

        public Vector2Int GetAttackRange() => ActiveConfig.attackRange;

        public Vector2Int GetAllowHeightDiff()
        {
            return ActiveConfig.allowHeightDiff;
        }
    }
    
    public class ActiveSkillSelectContext
    {
        public readonly CharacterUnitController Caster;
        public readonly GridController Grid;
        public readonly ActionType ActionType;

        public ActiveSkillSelectContext(CharacterUnitController caster, GridController grid, ActionType actionType)
        {
            Caster = caster;
            Grid = grid;
            ActionType = actionType;
        }
    }
    
    // public class ActiveSkillSelectResult
    // {
    //     public bool IsValid;
    //     public CharacterUnitController Caster;
    //     public GridController TargetGrid;
    // }
    
    public class ActiveSkillExecuteContextWrapper
    {
        private readonly ActiveSkillSelectContext ExecuteContext;
        private readonly Func<ActiveSkillSelectContext, IEnumerator> ExecuteFunc;
        
        public ActiveSkillExecuteContextWrapper(ActiveSkillSelectContext executeContext, Func<ActiveSkillSelectContext, IEnumerator> executeFunc)
        {
            ExecuteContext = executeContext;
            ExecuteFunc = executeFunc;
        }
        public IEnumerator Execute()
        {
            return ExecuteFunc(ExecuteContext);
        }
    }
}