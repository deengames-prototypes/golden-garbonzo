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
        }

        private void MainProcessingLoop()
        {
            string input = "";

            while (input.Trim().ToUpper() != "QUIT")
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
                    SpeakAndPrint("Type 'quit' to quit, 'list' or 'l' to list the current room.");
                    break;
                case "LIST":
                case "L":
                    SpeakAndPrint(this.currentRoom.GetContents());
                    break;
                case "ATTACK":
                    this.ProcessAttack(inputTokens);
                    break;
                case "QUIT":
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

                    var winnerMessage = results.Winner == player ? "you vanquish your foe!" : "you collapse to the ground in a heap!";
                    SpeakAndPrint($"You attack the {target.Name}! After {results.RoundMessages.Length} rounds, {winnerMessage}");
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
