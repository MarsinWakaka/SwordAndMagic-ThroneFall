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
    public class ClubAttackSkillData : ActiveSKillData
    {
        public int damage;
    }
    
    [CreateAssetMenu(menuName = "SkillConfig/Active/ClubAttack")]
    public class ClubAttackConfig : ActiveSkillConfig
    {
        [SerializeField] private ClubAttackSkillData[] growthData;
        
        public ClubAttackSkillData GetGrowthData(int level)
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
            return "挥舞棍棒，对目标位置目标造成" + damage + "点伤害";
        }

        public override ActiveSKillData GetActiveSKillData(int level)
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
            return new ClubAttackSkillInstance(skillID, level)
            {
                remainCooldown = 0
            };
        }
    }

    public class ClubAttackSkillInstance : ActiveSkillInstance
    {
        public readonly ClubAttackSkillData Data;

        public ClubAttackSkillInstance(string skillId, int skillLevel)
            : base(skillId, skillLevel)
        {
            var config = ActiveSkillConfigManager.Instance.GetConfig(skillId) as ClubAttackConfig;
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
            var gridDataProvider = ServiceLocator.Resolve<IGridManager>();
            var config = ActiveConfig;
            // TODO 看是否需要替换为对应方向

            context.Caster.PlayAnimation(AnimationType.Attack_0);
            // TODO 改为动画事件回调播放音效
            SoundManager.Instance.PlaySFXOneShot(SFX.ClubAttack);
            yield return new WaitForSeconds(1);

            var impactScope = config.GetSkillScope(targetGrid, Direction.Up, gridDataProvider.IsGridExist);
            foreach (var impactCoord in impactScope)
            {
                var grid = gridDataProvider.GetGridController(impactCoord);
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