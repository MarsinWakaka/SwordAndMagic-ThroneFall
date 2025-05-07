using GameLogic.Unit;

namespace GameLogic.GameAction.Attack
{
    public interface ITakeDamageableEntity : IEntity
    {
        public void TakeDamage(DamageSegment damageSegment);
    }
}