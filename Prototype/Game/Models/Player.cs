using Prototype.Game.Enums;
using Prototype.Game.Models.Items;
using Prototype.Game.Models.Items.Assemblable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Game.Models
{
    class Player : Monster
    {
        private const float STONE_SKIN_DEFENSE_MULITPLIER = 2f;
        private const int STONE_SKIN_TURNS_PER_USE = 3;
        public const float KICK_MULTIPLIER = 3.5f;
        public const float NANITE_DAMAGE_PERCENT = 0.25f; // 0.3f => damage 30% of monsters' max health

        public const int HEALTH_GROWTH_ON_STAT_POINT = 5;
        public const int STR_DEF_GROWTH_ON_STAT_POINT = 1;

        private const float HEAL_PERCENT_PER_MOVE = 0.3f;
        private const int SKILL_POINTS_REGAIN_PER_MOVE = 3; // Need to heal? Leave and come back, DONE.
        
        public List<AbstractItem> Inventory = new List<AbstractItem>();

        public int CurrentSkillPoints { get; set; }
        public int TotalSkillPoints { get; private set; }
        public bool IsFocused = false;
        public readonly List<Skill> Skills = new List<Skill>();

        public int Level { get; private set; } = 1;
        new public int ExperiencePoints { get; private set; } = 0;

        public int PhaseShieldLeft = 0;

        private int TurnsOfStoneSkinLeft = 0;

        internal static readonly Dictionary<Skill, int> SkillCosts = new Dictionary<Skill, int>()
        {
            { Skill.Heal, 5 },
            { Skill.StoneSkin, 8 },
            { Skill.PhaseShield, 12 },
            { Skill.Kick, 5 },
            { Skill.Focus, 9 },
            { Skill.NanoSwarm, 12 },
        };

        internal static readonly Dictionary<int, List<Skill>> SkillChoicesByLevel = new Dictionary<int, List<Skill>>()
        {
            { 2, new List<Skill>() { Skill.Heal, Skill.Kick } },
            { 3, new List<Skill>() { Skill.StoneSkin, Skill.Focus } },
            { 4, new List<Skill>() { Skill.PhaseShield, Skill.NanoSwarm } },
        };

        public Player() : base("Player", 50, 7, 3, 2)
        {
            this.TotalSkillPoints = 20;
            this.CurrentSkillPoints = this.TotalSkillPoints;
        }

        public override int Defense
        {
            get
            {
                var baseDefense = base.Defense;
                var multiplier = (this.HasStoneSkin() ? STONE_SKIN_DEFENSE_MULITPLIER : 1);
                return (int)(baseDefense * multiplier);
            }
        }

        internal void RemoveItem(string itemName)
        {
            itemName = itemName.ToUpperInvariant();
            var item = this.Inventory.FirstOrDefault(i => i.Name.ToUpperInvariant() == itemName);
            if (item != null)
            {
                this.Inventory.Remove(item);
            }
        }

        /// <summary>
        /// Returns the first item you can build, if any.
        /// </summary>
        /// <returns></returns>
        internal Type GetBuildableType(WorkBench workBench)
        {
            if (workBench.CanAssemble(typeof(PowerCube), this))
            {
                return typeof(PowerCube);
            }
            else if (workBench.CanAssemble(typeof(GlassCube), this))
            {
                return typeof(GlassCube);
            }
            else if (workBench.CanAssemble(typeof(PositronEmitter), this))
            {
                return typeof(PositronEmitter);
            }
            else if (workBench.CanAssemble(typeof(AntimatterCoil), this))

            {
                return typeof(AntimatterCoil);
            }

            return null;
        }
        
        /// <summary>
        /// Gain the specified amount of experience points.
        /// </summary>
        /// <returns>True if the player leveled up; otherwise, false.</returns>
        internal bool GainExperience(int xp)
        {
            this.ExperiencePoints += xp;
            if (this.ExperiencePoints >= this.MaxExperiencePointsForCurrentLevel())
            {
                this.Level++;
                return true;
            }

            return false;
        }
        

        internal void AddStoneSkin()
        {
            this.TurnsOfStoneSkinLeft += STONE_SKIN_TURNS_PER_USE;
        }

        internal string PostBattleRound()
        {
            if (this.HasStoneSkin())
            {
                this.TurnsOfStoneSkinLeft -= 1;
                if (!this.HasStoneSkin())
                {
                    return "Your stone-armoured skin returns to normal.";
                }
            }
            return null;
        }

        internal void PostMove()
        {
            int healAmount = (int)(this.TotalHealth * HEAL_PERCENT_PER_MOVE);
            this.CurrentHealth = Math.Min(this.CurrentHealth + healAmount, this.TotalHealth);

            if (this.CurrentSkillPoints < this.TotalSkillPoints)
            {
                this.CurrentSkillPoints += SKILL_POINTS_REGAIN_PER_MOVE;
                this.CurrentSkillPoints = Math.Min(this.CurrentSkillPoints, this.TotalSkillPoints);
            }
        }

        private int MaxExperiencePointsForCurrentLevel()
        {
            return 800 + ((this.Level - 1) * 1500); // 800, 1800, 2800, ...
        }

        internal bool HasStoneSkin()
        {
            return this.TurnsOfStoneSkinLeft > 0;
        }
    }
}
