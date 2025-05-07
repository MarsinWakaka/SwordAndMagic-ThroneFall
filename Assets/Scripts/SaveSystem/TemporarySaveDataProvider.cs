using System.Collections.Generic;
using System.IO;
using GameLogic.Unit;
using UnityEngine;

namespace SaveSystem
{
    public class TemporarySaveDataProvider : ISaveDataProvider
    {
        public SaveData CreateDefaultData()
        {
            // CreateCharacterData
            // 1. Elaina
            var elaina = ScriptableObject.CreateInstance<CharacterData>();
            elaina.characterID = "Elaina";
            elaina.level = 2;
            elaina.activeSkillsData = new List<SkillData>()
            {
                new() { skillID = "FireVolt", level = 1 },
            };
            elaina.passiveSkillsData = new List<SkillData>()
            {
                new() { skillID = "FoxPlan", level = 1 },
            };
            // 2. Ais
            var ais = ScriptableObject.CreateInstance<CharacterData>();
            ais.characterID = "Ais";
            ais.level = 1;
            ais.activeSkillsData = new List<SkillData>()
            {
                new() { skillID = "DeltaSlash", level = 1 },
            };
            
            return new SaveData()
            {
                ownedCharacters = new List<CharacterData>()
                {
                    elaina,
                    ais,
                },
                resources = 400,
                
                levelPassedData = new List<LevelProgressData>()
                {
                    new LevelProgressData("1-1", 3),
                    new LevelProgressData("1-2", 1),
                }
            };
        }
    }
}