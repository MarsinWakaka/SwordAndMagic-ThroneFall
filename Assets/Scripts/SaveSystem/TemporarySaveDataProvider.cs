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
            var elaina = new CharacterData
            {
                characterID = "Elaina",
                level = 2,
                activeSkillsData = new List<SkillData>()
                {
                    new() { skillID = "FireVolt", level = 1 },
                },
                passiveSkillsData = new List<SkillData>()
                {
                    new() { skillID = "FoxPlan", level = 1 },
                }
            };
            // 2. Ais
            var ais = new CharacterData
            {
                characterID = "Ais",
                level = 1,
                activeSkillsData = new List<SkillData>()
                {
                    new() { skillID = "DeltaSlash", level = 1 },
                }
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