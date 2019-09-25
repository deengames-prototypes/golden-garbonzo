using Prototype.Game.Enums;
using Prototype.Game.Models;

namespace Prototype.Game
{
    static class SkillExecutor
    {
        public static void Execute(Skill skill, Player player)
        {
            switch (skill)
            {
                case Skill.Heal:
                    player.CurrentHealth = player.TotalHealth;
                    break;
            }
        }
    }
}
