using System.Collections.Generic;
using GameLogic.Skill;
using GameLogic.Unit;

namespace SaveSystem
{
    public class TemporarySaveDataProvider : ISaveDataProvider
    {
        public SaveData CreateDefaultData()
        {
            // CreateCharacterData
            // 1. Elaina
            var elaina = new CharacterData
            {
                characterID = CharacterID.Elaina,
                level = 1,
                skillsData = new List<SkillData>()
                {
                    new(SkillID.ElainaDefaultAttack, 1),
                    // new(SkillID.ElainaFireVolt, 1),
                    // new(SkillID.ElainaPassive, 1),
                },
                // passiveSkillsData = new List<SkillData>()
                // {
                //     // new(SkillID.ElainaPassive, 1),
                // }
            };
            // 2. Ais
            var ais = new CharacterData
            {
                characterID = CharacterID.Ais,
                level = 1,
                skillsData = new List<SkillData>()
                {
                    new(SkillID.AisDefaultAttack, 1),
                    // new(SkillID.AisDeltaSlash, 1),
                    // new(SkillID.AisPassive, 1),
                },
                // passiveSkillsData = new List<SkillData>()
                // {
                //     new(SkillID.AisPassive, 1),
                // }
            };

            return new SaveData()
            {
                ownedCharacters = new List<CharacterData>()
                {
                    elaina,
                    ais,
                },
                resources = 120,
                
                levelPassedData = new List<LevelProgressData>()
                {
                }
            };
        }
    }
}