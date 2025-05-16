using System;
using System.Collections;
using Config;
using GameLogic.BUFF;
using GameLogic.GameAction.Attack;
using GameLogic.Grid;
using GameLogic.Skill.Active;
using GameLogic.Unit;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using SoundSystem;
using UnityEngine;
using Utilities;

namespace GameLogic.Skill.Concrete
{
    [Serializable]
    public class ElainaFireVoltData : ActiveSKillConfigData
    {
        public int damage;
    }
    
    [CreateAssetMenu(menuName = "Character/Elaina/Skill/FireVolt")]
    public class ElainaFireVoltConfig : ActiveSkillConfig
    {
        [SerializeField] private ElainaFireVoltData[] growthData;
        
        public ElainaFireVoltData GetSkillData(int level)
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
            return "召唤火焰魔法，对目标位置以及相邻位置的目标造成" + damage + "点火焰伤害，并为最近的友军施加一层防御提升提升BUFF";
        }

        public override ActiveSKillConfigData GetActiveSKillData(int level)
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
            return new FireVoltSkillInstance(SkillID, level)
            {
                remainCooldown = 0
            };
        }
    }

    public class FireVoltSkillInstance : ActiveSkillInstance
    {
        public readonly ElainaFireVoltData Data;

        public FireVoltSkillInstance(string skillId, int skillLevel)
            : base(skillId, skillLevel)
        {
            var config = SkillConfigManager.Instance.GetConfig(skillId) as ElainaFireVoltConfig;
            if (config == null)
            {
                Debug.LogError($"Failed to load skill config for {skillId}");
                return;
            }

            Data = config.GetSkillData(skillLevel);
        }

        public override IEnumerator Execute(ActiveSkillSelectContext context)
        {
            yield return base.Execute(context);
            var targetGrid = context.Grid.GetGrid2DCoord();
            var gridManager = ServiceLocator.Resolve<IGridManager>();
            var config = ActiveConfig;
            // TODO 看是否需要替换为对应方向

            context.Caster.PlayAnimation(AnimationType.Attack_0);
            SoundManager.Instance.PlaySFXOneShot(SFX.FireVolt);
            yield return new WaitForSeconds(1);

            var impactScope = config.GetSkillScope(Owner.Coordinate(), targetGrid, gridManager.IsWalkableTerrain);
            foreach (var impactCoord in impactScope)
            {
                var grid = gridManager.GetGridController(impactCoord);
                if (grid == null) continue;
                var gridRT = grid.RuntimeData;
                var entity = gridRT.EntitiesOnThis;
                if (entity is not CharacterUnitController target) continue;
                VFXManager.Instance.GetFX().transform.position = CoordinateConverter.CoordToWorldPos(gridRT.GridCoord.Value);
                new AttackAction(
                    context.Caster, target, context.ActionType,
                    new DamageSegment[]
                    {
                        new(Data.damage, DamageType.Physical)
                    }).Execute();
            }

            var characterManager = ServiceLocator.Resolve<ICharacterManager>();
            // TODO 施加buff
            var buffTarget = characterManager.GetNearestAlly(context.Caster);
            if (buffTarget != null)
            {
                buffTarget.CharacterRuntimeData.BuffManager.AddBuff(BuffID.DefenseUp, 0, 1);
            }
            else
            {
                Debug.Log("No ally found for buff application");
            }

            yield return new WaitForSeconds(.5f);
        }
    }
}