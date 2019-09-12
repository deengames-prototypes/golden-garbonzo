using Prototype.Game.Models;
using System.Collections.Generic;

namespace Prototype.Game.Battle
{
    class Battler
    {
        private Monster attacker;
        private Monster defender;

        public Battler(Monster attacker, Monster defender)
        {
            this.attacker = attacker;
            this.defender = defender;
        }

        public BattleResults FightToTheDeath()
        {
            var results = new BattleResults();
            var messages = new List<string>();

            while (attacker.CurrentHealth > 0 && defender.CurrentHealth > 0)
            {
                messages.Add(this.Attack(attacker, defender));
                if (defender.CurrentHealth > 0)
                {
                    messages.Add(this.Attack(defender, attacker));
                }
            }

            results.Winner = attacker.CurrentHealth > 0 ? attacker : defender;
            results.Loser = attacker.CurrentHealth == 0 ? attacker : defender;
            results.RoundMessages = messages.ToArray();

            return results;
        }

        // One round of combat
        private string Attack(Monster attacker, Monster defender)
        {
            var damage = attacker.Strength - defender.Defense;
            defender.CurrentHealth -= damage;
            var message = $"{attacker.Name} attacks {defender.Name} for {damage} damage!";
            return message;
        }
    }

    class BattleResults
    {
        public Monster Winner { get; set; }
        public Monster Loser { get; set; }
        public string[] RoundMessages { get; set; }
    }
}
