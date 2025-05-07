using System;
using System.Collections;
using Config;
using GameLogic.GameAction.Attack;
using GameLogic.Grid;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using SoundSystem;
using UnityEngine;
using Utilities;

namespace GameLogic.Skill.Active.Concrete
{
    [Serializable]
    public class DeltaSlashSkillData : ActiveSKillData
    {
        public int damage;
    }
    
    [CreateAssetMenu(menuName = "SkillConfig/Active/DeltaSlash")]
    public class DeltaSlashConfig : ActiveSkillConfig
    {
        [SerializeField] private DeltaSlashSkillData[] growthData;

        public override ActiveSKillData GetActiveSKillData(int level) => GetGrowthData(level);

        public DeltaSlashSkillData GetGrowthData(int level)
        {
            if (level < 1 || level > growthData.Length)
            {
                Debug.LogError($"Invalid skill level: {level}");
                return null;
            }
            return growthData[level - 1];
        }

        public override string GetSkillDescription(int level)
        {
            if (level < 1 || level > growthData.Length)
            {
                Debug.LogError($"Invalid skill level: {level}");
                return "技能描述请求错误";
            }

            var damage = growthData[level - 1].damage;
            return "发动华丽的一击\n对身边的目标造成" + damage + "点斩击伤害";
        }
        
        public DeltaSlashSkillData GetSkillData(int level)
        {
            if (level < 1 || level > growthData.Length)
            {
                Debug.LogError($"Invalid skill level: {level}");
                return null;
            }

            return growthData[level - 1];
        }
        
        public override ActiveSkillInstance CreateActiveSkillInstance(int level)
        {
            return new DeltaSlashSkillInstance(skillID, level)
            {
                remainCooldown = 0
            };
        }
    }

    public class DeltaSlashSkillInstance : ActiveSkillInstance
    {
        public DeltaSlashSkillData Data;
        
        public DeltaSlashSkillInstance(string skillId, int skillLevel)
            : base(skillId, skillLevel)
        {
            var config = ActiveSkillConfigManager.Instance.GetConfig(skillId) as DeltaSlashConfig;
            if (config == null)
            {
                Debug.LogError($"Failed to load skill config for {skillId}");
                return;
            }

            Data = config.GetGrowthData(skillLevel);
        }

        public override IEnumerator Execute(ActiveSkillSelectContext context)
        {
            yield return base.Execute(context);
            var targetGrid = context.Grid.GetGrid2DCoord();
            var gridManager = ServiceLocator.Resolve<IGridManager>();
            var config = ActiveConfig;
            // TODO 看是否需要替换为对应方向
            
            context.Caster.PlayAnimation(AnimationType.Attack_0);
            SoundManager.Instance.PlaySFXOneShot(SFX.DeltaSlash);
            yield return new WaitForSeconds(1);
            
            var impactScope = config.GetSkillScope(targetGrid, Direction.Up, gridManager.IsGridExist);
            foreach (var impactCoord in impactScope)
            {
                var grid = gridManager.GetGridController(impactCoord);
                if (grid == null) continue;
                var gridRT = grid.RuntimeData;
                var entity = gridRT.EntitiesOnThis;
                if (entity is not CharacterUnitController target) continue;
                new AttackAction(
                    context.Caster, target, context.ActionType,
                    new DamageSegment[]
                    {
                        new(Data.damage, DamageType.Physical)
                    }).Execute();
            }
            yield return new WaitForSeconds(.5f);
        }
    }
}