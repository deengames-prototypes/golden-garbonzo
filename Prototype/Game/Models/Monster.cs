using Prototype.Game.Models.Items;
using System;

namespace Prototype.Game.Models
{
    class Monster
    {
        public int CurrentHealth { get; set; }
        public int TotalHealth { get; set; }
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public AbstractItem Item { get; set; }

        public static Monster Generate(MonsterType type)
        {
            switch (type)
            {
                case MonsterType.Weak: return new Monster("Slime", 8, 5, 1);
                case MonsterType.Regular: return new Monster("Blue Beak", 20, 8, 3);
                case MonsterType.Strong: return new Monster("Goblin", 40, 11, 5);
                default: return new Monster("Bat", 10, 6, 2);
            }

            throw new InvalidOperationException($"Not sure how to create a monster of type: {type}");
        }

        protected Monster(string name, int health, int strength, int defense)
        {
            this.Name = name;
            this.CurrentHealth = health;
            this.TotalHealth = health;
            this.Strength = strength;
            this.Defense = defense;
        }

        public override string ToString()
        {
            return $"{Name}: {CurrentHealth}/{TotalHealth} health";
        }
    }

    public enum MonsterType
    {
        Weak,
        Regular,
        Strong,
    }
}
