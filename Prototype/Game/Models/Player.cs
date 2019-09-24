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
        public List<AbstractItem> Inventory = new List<AbstractItem>();

        public int CurrentSkillPoints { get; private set; }
        public int TotalSkillPoints { get; private set; }
        public readonly List<Skill> Skills = new List<Skill>();

        private readonly Dictionary<Skill, int> SkillCosts = new Dictionary<Skill, int>()
        {
            { Skill.Heal, 5 },
            { Skill.StoneSkin, 8 },
            { Skill.PhaseShield, 12 },
            { Skill.Kick, 5 },
            { Skill.Teleport, 7 },
            { Skill.Focus, 10 },
        };

        public Player() : base("Player", 50, 7, 3, 2)
        {
            this.TotalSkillPoints = 20;
            this.CurrentSkillPoints = this.TotalSkillPoints;
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
    }
}
