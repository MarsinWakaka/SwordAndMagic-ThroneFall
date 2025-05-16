namespace GameLogic.Unit
{
    public enum Faction
    {
        Player,
        Enemy,
    }

    public static class FactionUtil
    {
        public static Faction Opposite(this Faction faction)
        {
            return faction == Faction.Player ? Faction.Enemy : Faction.Player;
        }
    }
}