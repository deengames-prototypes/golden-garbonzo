using Prototype.Game.Battle;
using Prototype.Game.Models;
using Prototype.TextToSpeech;
using System;
using System.Collections.Generic;

namespace Prototype.Game
{
    class GoldenGarbonzo
    {
        private ISpeaker speaker;
        private Room currentRoom;
        private Player player = new Player();

        public void Run()
        {
            this.speaker = new MicrosoftSpeaker();            

            var dungeon = new Dungeon();
            this.currentRoom = dungeon.Rooms[0];

            SpeakAndPrint($"Welcome to the dungeon! {this.currentRoom.GetContents()}");
            SpeakAndPrint("Type something and press enter. Type 'help' for help, or 'quit' to quit.");

            this.MainProcessingLoop();

            this.speaker.FinishSpeaking();

            this.SpeakAndPrint("Bye!");
            System.Threading.Thread.Sleep(1250);
        }

        private void MainProcessingLoop()
        {
            string input = "";

            while (input.Trim().ToUpper() != "QUIT" && input.ToUpper() != "Q")
            {
                SpeakAndPrint("Your command? ");
                Console.Write("> ");
                input = Console.ReadLine();
                this.speaker.StopAndClearQueue();
                SpeakAndPrint($"You typed: {input}");
                this.ProcessInput(input);
            }

            this.SpeakAndPrint("Bye!");
        }

        private void ProcessInput(string input)
        {
            var inputTokens = input.ToUpperInvariant().Split(' ');
            var command = inputTokens[0];

            switch (command)
            {
                case "HELP":
                    SpeakAndPrint("Type 'quit' to quit, 'list' or 'l' to list the current room; type ATTACK to attack a target, or OPTIONS to change options");
                    break;
                case "LIST":
                case "L":
                    SpeakAndPrint(this.currentRoom.GetContents());
                    break;
                case "ATTACK":
                case "FIGHT":
                case "A":
                case "F":
                    this.ProcessAttack(inputTokens);
                    break;
                case "GO":
                case "G":
                    this.ProcessMove(inputTokens);
                    break;
                case "OPTIONS":
                case "O":
                    this.ProcessOptions(inputTokens);
                    break;
                case "QUIT":
                case "Q":
                    return;
                default:
                    SpeakAndPrint($"Not sure how to {input}");
                    break;
            }
        }

        private void ProcessAttack(string[] inputTokens)
        {
            if (inputTokens.Length != 2)
            {
                SpeakAndPrint("Type attack and then the name of the target. Type list to list targets.");
            }
            else
            {
                var targetName = inputTokens[1];
                if (!this.currentRoom.HasMonster(targetName))
                {
                    SpeakAndPrint($"There doesn't seem to be a {targetName} here to attack.");
                }
                else
                {
                    var target = this.currentRoom.GetMonster(targetName);
                    var battler = new Battler(this.player, target);
                    var results = battler.FightToTheDeath();
                    var winnerMessage = results.Winner == player ? $"you vanquish your foe! You had {player.CurrentHealth} out of {player.TotalHealth} health." : $"you collapse to the ground in a heap! (The {target.Name} had {target.CurrentHealth} out of {target.TotalHealth} health.)";
                    player.CurrentHealth = player.TotalHealth;
                    SpeakAndPrint($"You attack a {target.Name}!");

                    switch (Options.CombatType)
                    {
                        case CombatType.RoundByRound:
                            foreach (var round in results.RoundMessages)
                            {
                                SpeakAndPrint(round);
                            }
                            break;
                    }

                    SpeakAndPrint($"After {results.RoundMessages.Length} rounds, {winnerMessage}");
                }
            }
        }

        private void ProcessMove(string[] inputTokens)
        {
            if(inputTokens.Length != 2)
            {
                SpeakAndPrint("Type go and then the room to go to. Type list to list connected rooms.");
            }
            else
            {
                var targetName = inputTokens[1];
                if (!this.currentRoom.IsConnectedTo(targetName))
                {
                    SpeakAndPrint($"There doesn't seem to be a way to go to {targetName} from here.");
                }
                else
                {
                    this.currentRoom = this.currentRoom.GetConnection(targetName);
                    SpeakAndPrint(this.currentRoom.GetContents());

                }
            }
        }

        private void ProcessOptions(string[] inputTokens)
        {
            if (inputTokens.Length != 2)
            {
                this.SpeakAndPrint($"Type options combat to change the combat style. It's currently set to {Options.CombatType}.");
            }
            else
            {
                var targetName = inputTokens[1].ToUpperInvariant();
                if (targetName != "COMBAT")
                {
                    SpeakAndPrint($"There doesn't seem to be a {targetName} option. Valid options are: COMBAT");
                }
                else
                {
                    switch (targetName)
                    {
                        case "COMBAT":
                            Options.CombatType = Options.CombatType == CombatType.RoundByRound ? CombatType.Summary : CombatType.RoundByRound;
                            this.SpeakAndPrint($"Combat changed to {Options.CombatType.ToString()}");
                            break;
                    }
                }
            }
        }

        private void SpeakAndPrint(string text)
        {
            Console.WriteLine(text);
            this.speaker.Speak(text);
        }
    }
}
