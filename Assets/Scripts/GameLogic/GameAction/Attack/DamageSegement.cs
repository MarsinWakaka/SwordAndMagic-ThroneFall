namespace GameLogic.GameAction.Attack
{
    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Electric,
        Poison,
        Holy,
        Dark,
    }

    public class DamageSegment
    {
        public int Damage;
        public DamageType DamageType;
        
        public DamageSegment(int damage, DamageType damageType)
        {
            Damage = damage;
            DamageType = damageType;
        }
    }
}