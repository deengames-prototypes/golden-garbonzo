using Prototype.Game.Enums;
using Prototype.Game.Models;
using System;

namespace Prototype.Game
{
    static class SkillExecutor
    {
        public static string Execute(Skill skill, Player player)
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
                default:
                    throw new ArgumentException(skill.ToString());
            }
        }
    }
}
