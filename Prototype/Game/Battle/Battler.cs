using Prototype.Game.Models;
using System;
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

                if (attacker is Player)
                {
                    var player = (attacker as Player);
                    var message = player.PostBattleRound();
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        messages.Add(message);
                    }
                }
                if (defender is Player)
                {
                    var player = (defender as Player);
                    var message = player.PostBattleRound();
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        messages.Add(message);
                    }
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
            var shieldAbsorbs = 0;
            var player = defender as Player;

            var damage = (attacker.Strength - defender.Defense) * attacker.AttacksPerRound;

            if (defender is Player)
            {
                if (player.PhaseShieldLeft > 0)
                {
                    shieldAbsorbs = Math.Min(player.PhaseShieldLeft, damage);
                    player.PhaseShieldLeft -= shieldAbsorbs;
                    damage -= shieldAbsorbs;
                }
            }

            defender.CurrentHealth -= damage;
            var message = "";

            switch (Options.SpeechMode)
            {
                case SpeechMode.Detailed:
                    message = $"{attacker.Name} with {attacker.CurrentHealth} health attacks {attacker.AttacksPerRound} times for {damage} damage";
                    break;
                case SpeechMode.Summary:
                    message = $"{attacker.Name} attacks {defender.Name}.";
                    break;
            }

            if (shieldAbsorbs > 0)
            {
                message += $" Your shield absorbed {shieldAbsorbs} damage {(player != null && player.PhaseShieldLeft == 0 ? "and dissipated" : "")}";
            }

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
