using UnityEngine;

namespace GameLogic.Task
{
    [CreateAssetMenu(menuName = "Battle/Tasks/SurviveRound", order = 0)]
    public class SurviveRoundTask : BaseTask
    {
        [Header("Task Settings")]
        public int targetRoundCount;

        public override void OnInitialize(BattleContext context)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsTaskCompleted(BattleContext context)
        {
            return context.RoundCount >= targetRoundCount;
        }
    }
}