using System;
using System.Collections.Generic;
using Config;
using GameLogic.BUFF;
using GameLogic.Character.BattleRuntimeData;
using GameLogic.Skill.Active;
using GameLogic.Skill.Passive;
using GameLogic.Unit.ConfigData;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using UnityEngine;
using Utilities;

namespace GameLogic.Unit.BattleRuntimeData
{
    [Serializable]
    public class CharacterBattleRuntimeData : EntityBattleRuntimeData
    {
        // 角色属性
        public Bindable<int> MaxHp;
        public Bindable<int> MaxMoveRange;
        public const int MaxSkillPoint = 6;
        public Bindable<int> CurHp;
        public Bindable<int> CurMoveRange;
        public Bindable<int> CurSkillPoint;
        // 技能
        public List<ActiveSkillInstance> ActiveSkills;
        public List<PassiveSkillInstance> PassiveSkills;
        // 角色状态
        public Bindable<bool> CanAction;
        public BuffManager BuffManager;

        public override EntityConfigData ConfigData => CharacterConfigData;
        public CharacterConfigData CharacterConfigData => CharacterConfigManager.Instance.GetConfig(EntityID);
        
        public CharacterBattleRuntimeData(string instanceID, CharacterData characterData, Faction faction, Vector2Int gridCoord, Direction dir)
            : base(instanceID, characterData.characterID, faction, dir, gridCoord)
        {
            var level = characterData.level;
            var characterProperty = CharacterConfigData.GetCharacterProperties(level);
            
            MaxHp = new Bindable<int>(characterProperty.maxHp);
            MaxMoveRange = new Bindable<int>(characterProperty.maxMoveRange);
            CurHp = new Bindable<int>(characterProperty.maxHp);
            CurMoveRange = new Bindable<int>(characterProperty.maxMoveRange);
            CurSkillPoint = new Bindable<int>(characterProperty.initSkillPoint);
            
            CanAction = new Bindable<bool>(true);
            
            // 主动技能初始化
            ActiveSkills = new List<ActiveSkillInstance>();
            foreach (var activeSkillData in characterData.activeSkillsData)
            {
                var skillInstance = ActiveSkillConfigManager.Instance.GetConfig(activeSkillData.skillID)
                    .CreateActiveSkillInstance(activeSkillData.level);
                ActiveSkills.Add(skillInstance);
            }
            // 被动技能初始化
            PassiveSkills = new List<PassiveSkillInstance>();
            foreach (var passiveSkillData in characterData.passiveSkillsData)
            {
                var skillInstance = PassiveSkillConfigManager.Instance.GetConfig(passiveSkillData.skillID)
                    .CreatePassiveSkillInstance(passiveSkillData.level);
                PassiveSkills.Add(skillInstance);
            }
        }
        
        public void InitializeSkill(CharacterUnitController owner)
        {
            BuffManager = new BuffManager(owner);
            foreach (var skill in ActiveSkills)
            {
                skill.TakeEffect(owner);
            }
            foreach (var skill in PassiveSkills)
            {
                skill.TakeEffect(owner);
            }
        }

        public override string ToString()
        {
            return $"[CharacterRuntimeData] ID: {InstanceID}" +
                   $"\nEntityID: {EntityID}" +
                   $"\nFaction: {faction}" +
                   $"\nGridCoord: {gridCoord}" +
                   $"\nDir: {dir}" +
                   $"\nMaxHp: {MaxHp.Value}" +
                   $"\nMaxMoveRange: {MaxMoveRange.Value}" +
                   $"\n, CurHp: {CurHp.Value}" +
                   $"\nCurMoveRange: {CurMoveRange.Value}" +
                   $"\nCurSkillPoint: {CurSkillPoint.Value}";
        }
        
/*        
        public CharacterRuntimeData(string instanceID, string entityID, Faction faction, Vector2Int gridCoord, Direction dir)
            : base(instanceID, entityID, faction, dir, gridCoord)
        {
            var configData = CharacterConfigData;
            var level = PlayerDataManager.Instance.GetPlayerLevel(EntityID);
            var characterData = configData.GetCharacterData(level);
            
            // 属性初始化
            MaxHp = new Bindable<int>(characterData.maxHp);
            MaxMoveRange = new Bindable<int>(characterData.maxMoveRange);
            CurHp = new Bindable<int>(characterData.maxHp);
            CurMoveRange = new Bindable<int>(characterData.maxMoveRange);
            CurSkillPoint = new Bindable<int>(characterData.initSkillPoint);
            
            CanAction = new Bindable<bool>(true);
        }
*/
    }
}