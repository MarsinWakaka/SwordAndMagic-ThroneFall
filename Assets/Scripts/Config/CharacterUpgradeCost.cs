using System;

namespace Config
{
    public abstract class CharacterUpgradeCost
    {
        // 最高12级
        private static readonly int[] Cost = { 0, 25, 40, 50, 70, 100, 130, 150, 200 };
        private static int[] _accumulatedCost;
        
        public static int GetAccumulatedCost(int from, int to)
        {
            if (from < 0 || to >= Cost.Length || from > to)
            {
                throw new ArgumentOutOfRangeException($"Invalid range: {from} to {to}");
            }
            _accumulatedCost ??= CalculateAccumulatedCost();
            return _accumulatedCost[to] - _accumulatedCost[from];
        }

        private static int[] CalculateAccumulatedCost()
        {
            var accumulatedCost = new int[Cost.Length];
            for (var i = 1; i < Cost.Length; i++)
            {
                accumulatedCost[i] = accumulatedCost[i - 1] + Cost[i];
            }
            return accumulatedCost;
        }

        public static int GetUpgradeCost(int level)
        {
            if (level < 1)
            {
                throw new ArgumentException("Level must be greater than 0");
            }
            if (level >= Cost.Length)
            {
                throw new ArgumentOutOfRangeException($"Level {level} exceeds maximum defined level {Cost.Length - 1}");
            }
            return Cost[level];
            // return level * (level + 5) * 10;
        } 
    }
}