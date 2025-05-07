using UnityEngine;

namespace GameLogic.Task
{
    public class ClearAllEnemiesTask : BaseTask
    {
        [Header("Task Settings")]
        public int targetEnemyCount;

        public override void OnInitialize(BattleContext context)
        {
            // Initialize the task with the current enemy count
            // targetEnemyCount = context.EnemyCount;
        }

        public override bool IsTaskCompleted(BattleContext context)
        {
            // return context.EnemyCount <= 0;
            return true;
        }
    }
}