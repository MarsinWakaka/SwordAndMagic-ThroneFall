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
        // TODO 进一步继续封装
        // 角色属性
        public Bindable<int> MaxHp;
        public Bindable<int> MaxMoveRange;
        public const int MaxSkillPoint = 6;
        public Bindable<int> CurHp;
        public Bindable<int> CurMoveRange;
        private Bindable<int> _curSkillPoint;
        // 技能
        public List<ActiveSkillInstance> ActiveSkills;
        public List<PassiveSkillInstance> PassiveSkills;
        // 角色状态
        public Bindable<bool> CanAction;
        public BuffManager BuffManager;
        
        public void AddListenerOnSkillPoint(Action<int> action)
        {
            _curSkillPoint.AddListener(action);
        }
        
        public void RemoveListenerOnSkillPoint(Action<int> action)
        {
            _curSkillPoint.RemoveListener(action);
        }
        
        public int CurSkillPoint
        {
            get => _curSkillPoint.Value;
            set => _curSkillPoint.Value = Mathf.Clamp(value, 0, MaxSkillPoint);
        }

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
            _curSkillPoint = new Bindable<int>(characterProperty.initSkillPoint);
            
            CanAction = new Bindable<bool>(true);
            
            // 主动技能初始化
            ActiveSkills = new List<ActiveSkillInstance>();
            PassiveSkills = new List<PassiveSkillInstance>();
            foreach (var skillData in characterData.skillsData)
            {
                // var skillInstance = ActiveSkillConfigManager.Instance.GetConfig(activeSkillData.skillID)
                //     .CreateActiveSkillInstance(activeSkillData.level);
                // ActiveSkills.Add(skillInstance);                
                var skillConfig = SkillConfigManager.Instance.GetConfig(skillData.skillID);
                switch (skillConfig)
                {
                    case PassiveSkillConfig psc:
                    {
                        var skillInstance = psc.CreatePassiveSkillInstance(skillData.level);
                        PassiveSkills.Add(skillInstance);
                        break;
                    }
                    case ActiveSkillConfig asc:
                    {
                        var skillInstance = asc.CreateActiveSkillInstance(skillData.level);
                        ActiveSkills.Add(skillInstance);
                        break;
                    }
                    default:
                        Debug.LogError($"Unknown skill config type: {skillConfig.GetType()}");
                        break;
                }
            }
        }
        
        public void InitializeSkill(CharacterUnitController owner)
        {
            BuffManager = new BuffManager(owner);
            foreach (var skill in ActiveSkills)
            {
                skill.Initialize(owner);
            }
            foreach (var skill in PassiveSkills)
            {
                skill.Initialize(owner);
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
                   $"\nCurSkillPoint: {CurSkillPoint}";
        }
    }
}