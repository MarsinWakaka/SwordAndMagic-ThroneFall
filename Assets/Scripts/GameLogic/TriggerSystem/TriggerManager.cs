using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using Events.Battle;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using UnityEngine;
using IServiceProvider = MyFramework.Utilities.IServiceProvider;

namespace GameLogic.TriggerSystem
{
    public enum AttackType
    {
        Move,
        Skill,
        Item,
        EndTurn
    }
    
    public interface ITriggerService : IServiceProvider
    {
        int RegisterTrigger(Trigger trigger);
        void UnRegisterTrigger(int triggerId);
        IEnumerator HandleCharacterLeaveArea(CharacterUnitController character, Vector2Int leavePosition);
        IEnumerator HandleCharacterEnterArea(CharacterUnitController character, Vector2Int enterPosition);
    }
    
    /// <summary>
    /// 需要监听角色移动事件、角色行动时间、回合更新事件
    /// </summary>
    public class TriggerManager : MonoBehaviour
    {
        private readonly Dictionary<int, Trigger> _triggers = new();
        private static int _triggerId;
        
        private void OnEnable()
        {
            EventBus.Channel(Channel.Gameplay).Subscribe<TriggerSceneEventArgs>(OnTurnUpdate);
        }
        
        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<TriggerSceneEventArgs>(OnTurnUpdate);
        }
        
        private void OnTurnUpdate(TriggerSceneEventArgs args)
        {
            // Debug.Log($"TriggerManager: OnTurnUpdate {args.NewTurn}");
            // 触发器检测
            // var removeList = 
            //     from trigger in _triggers.Values 
            //     where !trigger.NextTurn() 
            //     select trigger.ID;
            // foreach (var removeID in removeList) _triggers.Remove(removeID);
            args.OnComplete();
        }

        public int RegisterTrigger(Trigger trigger)
        {
            _triggers.Add(++_triggerId, trigger);
            return _triggerId;
        }
        
        public void UnRegisterTrigger(int triggerId)
        {
            _triggers.Remove(triggerId);
        }
    }
}