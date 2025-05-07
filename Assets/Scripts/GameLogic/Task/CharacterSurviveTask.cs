using UnityEngine;

namespace GameLogic.Task
{
    [CreateAssetMenu(menuName = "Battle/Tasks/UnitSurvive", order = 0)]
    public class CharacterSurviveTask : BaseTask
    {
        [Header("Task Settings")]
        public string unitID;

        public override void OnInitialize(BattleContext context)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsTaskCompleted(BattleContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}