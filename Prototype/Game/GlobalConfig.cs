using Prototype.Game.Models;
using System.Collections.Generic;

namespace Prototype.Game
{
    public static class GlobalConfig
    {
        public const int MIN_ROOMS_PER_FLOOR = 5;
        public const int MAX_ROOMS_PER_FLOOR = 8;
        public const int NUM_FLOORS = 3;

        public const int MIN_MONSTERS = 2;
        public const int MAX_MONSTERS = 5;
        public const float PROBABILITY_OF_NO_MONSTERS = 0f;
        public static Dictionary<MonsterType, float> MONSTER_PROBABILITY = new Dictionary<MonsterType, float>()
        {
            { MonsterType.Weak, 0.3f },
            { MonsterType.Regular, 0.5f },
            { MonsterType.Strong, 0.2f },
        };
    }
}
