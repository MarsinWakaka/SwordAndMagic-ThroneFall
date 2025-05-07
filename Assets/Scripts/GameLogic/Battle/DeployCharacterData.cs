using System.Collections.Generic;
using GameLogic.Character;
using GameLogic.Unit;
using MyFramework.Utilities;
using Player;

namespace GameLogic.Battle
{
    /// <summary>
    /// [Function]
    /// 部署数据
    /// [Purpose]
    /// 由于有些游戏中可能会出现一个角色存在多条部署数据的情况(例如关卡预设数据和玩家自身拥有的角色数据)
    /// 1. 为了区分这些数据，我们需要一个拥有唯一ID的类来存储这些数据
    /// 2. 防止角色重复部署
    /// </summary>
    public class DeployCharacterData
    {
        public readonly string Guid = System.Guid.NewGuid().ToString();
        public readonly Bindable<bool> CanUse = new(true);

        public readonly CharacterData Data;
        
        public DeployCharacterData(CharacterData characterData)
        {
            Data = characterData;
        }
    }
}