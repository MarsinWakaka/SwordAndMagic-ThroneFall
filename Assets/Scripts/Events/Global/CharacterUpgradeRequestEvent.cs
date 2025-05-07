using System;
using MyFramework.Utilities;

namespace Events.Global
{
    // public enum UpgradeResult
    // {
    //     // 升级成功
    //     Success,
    //     
    //     // 角色未解锁
    //     NotUnlockedCharacter,
    //     
    //     // 技能未解锁
    //     NotUnlockedSkill,
    //     
    //     // 已超过目标等级
    //     AlreadyOverTargetLevel,
    //     
    //     // 已满级
    //     AlreadyMaxLevel,
    //     
    //     // 升级所需资源不足
    //     NotEnoughResources,
    // }
    
    // TODO 做成异步请求
    public class CharacterUpgradeRequestEvent : IEventArgs
    {
        public CharacterUpgradeRequestEvent(string characterID, int targetLevel)
        {
            CharacterID = characterID;
            TargetLevel = targetLevel;
        }

        public string CharacterID { get; }
        public int TargetLevel { get; }
    }
}