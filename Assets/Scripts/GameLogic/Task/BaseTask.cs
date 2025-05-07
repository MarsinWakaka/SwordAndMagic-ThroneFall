using UnityEngine;

namespace GameLogic.Task
{
    public abstract class BaseTask : ScriptableObject
    {
        [Header("Base Task Info")]
        public string taskName;
        public string taskDescription;
        public Sprite taskIcon;
        
        public abstract void OnInitialize(BattleContext context);

        public abstract bool IsTaskCompleted(BattleContext context);
    }
}