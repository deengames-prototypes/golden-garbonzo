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
        public int AttacksPerRound { get; set; }
        public AbstractItem Item { get; set; }

        public static Monster Generate(MonsterType type)
        {
            switch (type)
            {
                case MonsterType.Weak: return new Monster("Slime", 16, 5, 1, 2);
                case MonsterType.Regular: return new Monster("Blue Beak", 40, 8, 3, 2);
                case MonsterType.Strong: return new Monster("Goblin", 80, 11, 5, 2);
                default: throw new InvalidOperationException($"Not sure how to create a monster of type: {type}");
            }
        }

        protected Monster(string name, int health, int strength, int defense, int attacksPerRound)
        {
            this.Name = name;
            this.CurrentHealth = health;
            this.TotalHealth = health;
            this.Strength = strength;
            this.Defense = defense;
            this.AttacksPerRound = attacksPerRound;
        }

        public override string ToString()
        {
            return $"{Name}: {CurrentHealth}/{TotalHealth} health";
        }

        public int ExperiencePoints { get { return this.TotalHealth * this.Strength; } }
    }

    public enum MonsterType
    {
        Weak,
        Regular,
        Strong,
    }
}
