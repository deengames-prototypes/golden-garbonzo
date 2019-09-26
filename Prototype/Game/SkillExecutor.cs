using Prototype.Game.Enums;
using Prototype.Game.Models;
using System;
using System.Collections.Generic;

namespace Prototype.Game
{
    static class SkillExecutor
    {
        public static string Execute(Skill skill, Player player, List<Monster> currentRoomMonsters)
        {
            switch (skill)
            {
                case Skill.Heal:
                    player.CurrentHealth = player.TotalHealth;
                    return "You heal to full health.";
                case Skill.StoneSkin:
                    player.AddStoneSkin();
                    return "Your skin hardens into stone.";
                case Skill.PhaseShield:
                    player.PhaseShieldLeft = 20;
                    return "A phase shield hums into existance around you.";
                case Skill.Focus:
                    player.IsFocused = true;
                    return "You focus and gather all your strength.";
                case Skill.NanoSwarm:
                    currentRoomMonsters.ForEach(m => m.CurrentHealth -= (int)(m.TotalHealth * Player.NANITE_DAMAGE_PERCENT));
                    return "Your swarm of nanite robots attack all monsters in the room.";
                default:
                    throw new ArgumentException(skill.ToString());
            }
        }
    }
}
