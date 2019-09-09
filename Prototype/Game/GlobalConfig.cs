using Prototype.Game.Models;
using System.Collections.Generic;

namespace Prototype.Game
{
    public static class GlobalConfig
    {
        public static int MIN_ROOMS_PER_FLOOR = 5;
        public static int MAX_ROOMS_PER_FLOOR = 8;

        public static int MIN_MONSTERS = 2;
        public static int MAX_MONSTERS = 5;
        public static float PROBABILITY_OF_NO_MONSTERS = 0.4f;
        public static Dictionary<MonsterType, float> MONSTER_PROBABILITY = new Dictionary<MonsterType, float>()
        {
            { MonsterType.Weak, 0.3f },
            { MonsterType.Regular, 0.5f },
            { MonsterType.Strong, 0.2f },
        };
    }
}
