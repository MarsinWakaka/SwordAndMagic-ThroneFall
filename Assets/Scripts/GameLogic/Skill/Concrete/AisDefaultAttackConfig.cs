using System.Collections;
using Config;
using GameLogic.GameAction.Attack;
using GameLogic.Grid;
using GameLogic.Skill.Active;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using SoundSystem;
using UnityEngine;
using Utilities;

namespace GameLogic.Skill.Concrete
{
    [CreateAssetMenu(fileName = Skill.SkillID.AisDefaultAttack, menuName = "Character/Ais/Skill/Ais_DefaultAttack")]
    public class AisDefaultAttackConfig : ActiveSkillConfig
    {
        [SerializeField] private AttackSkillData[] growthData;

        public override string GetSkillDescription(int level)
        {
            if (level < 1 || level > growthData.Length)
            {
                Debug.LogError($"Invalid skill level: {level}");
                return "技能描述请求错误";
            }

            var damage = growthData[level - 1].damage;
            return $"艾丝发动一次伤害为{damage}的普通攻击";
        }

        public override ActiveSKillConfigData GetActiveSKillData(int level) => GetGrowthData(level);
        
        public AttackSkillData GetGrowthData(int level)
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
            return new AisNormalAttackSkillInstance(SkillID, level)
            {
                remainCooldown = 0
            };
        }
    }
    
    public class AisNormalAttackSkillInstance : ActiveSkillInstance
    {
        public readonly AttackSkillData Data;
        
        public AisNormalAttackSkillInstance(string skillId, int skillLevel)
            : base(skillId, skillLevel)
        {
            var config = SkillConfigManager.Instance.GetConfig(skillId) as AisDefaultAttackConfig;
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

            
            
            // TODO 需要确保攻击前，队友已经响应完毕，得将触发监听的逻辑从AttackAction中剥离出来
            context.Caster.PlayAnimation(AnimationType.Attack_0);
            SoundManager.Instance.PlaySFXOneShot(SFX.DeltaSlash);
            yield return new WaitForSeconds(1);
            
            var impactScope = config.GetSkillScope(Owner.Coordinate(), targetGrid, gridManager.IsWalkableTerrain);
            foreach (var impactCoord in impactScope)
            {
                var grid = gridManager.GetGridController(impactCoord);
                if (grid == null) continue;
                var gridRT = grid.RuntimeData;
                var entity = gridRT.EntitiesOnThis;
                if (entity is not CharacterUnitController target) continue;
                // 设置技能特效
                VFXManager.Instance.GetFX().transform.position = CoordinateConverter.CoordToWorldPos(gridRT.GridCoord.Value);
                
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