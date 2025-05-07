using System;
using System.Collections.Generic;
using GameLogic.Character;
using GameLogic.Unit;
using MyFramework.Utilities;
using Player;
using UnityEngine;

namespace Events.Battle
{
    public class StartDeployEvent : IEventArgs
    {
        public readonly List<Vector2Int> CanDeployPositions;
        public readonly int MaxDeployCount;
        // 关卡限定提供的额外角色数据
        public readonly List<CharacterData> DeployCharacterData;
        // 关卡限定提供的额外其它数据...
        
        public event Action OnDeployComplete;
        
        public StartDeployEvent(List<CharacterData> deployCharacterData, List<Vector2Int> canDeployPositions, int maxDeployCount, Action onDeployComplete)
        {
            DeployCharacterData = deployCharacterData;
            CanDeployPositions = canDeployPositions;
            MaxDeployCount = maxDeployCount;
            OnDeployComplete = onDeployComplete;
        }

        public void OnOnDeployComplete()
        {
            OnDeployComplete?.Invoke();
        }
    }
}