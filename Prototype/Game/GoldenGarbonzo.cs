﻿using Prototype.Game.Battle;
using Prototype.Game.Models;
using Prototype.TextToSpeech;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var floors = dungeon.Floors;
            
            this.currentRoom = floors[0].Rooms[0];

            // TODO: this should probably be more random. For now, it's a progression of difficulty, amirite?
            floors.ForEach(f => f.SealRandomRoom());

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
                    SpeakAndPrint("Type 'quit' to quit, 'list' or 'l' to list the current room.");
                    break;
                case "LIST":
                case "L":
                    SpeakAndPrint(this.currentRoom.GetContents());
                    break;
                case "ATTACK":
                case "A":
                case "FIGHT":
                case "F":
                    this.ProcessAttack(inputTokens);
                    break;
                case "GO":
                case "G":
                    this.ProcessMove(inputTokens);
                    break;
                case "QUIT":
                case "Q":
                case "EXIT":
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
                    SpeakAndPrint($"You attack the {target.Name}! After {results.RoundMessages.Length} rounds, {winnerMessage}");
                    player.CurrentHealth = player.TotalHealth;

                    if (this.currentRoom.IsSealed && !this.currentRoom.Monsters.Any(m => m.CurrentHealth > 0) && results.Winner == player)
                    {
                        this.currentRoom.IsSealed = false;
                        SpeakAndPrint("The magic seals on all the doors dissipate.");
                    }
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
                if (this.currentRoom.IsSealed)
                {
                    SpeakAndPrint("You can't leave - all the doors are sealed shut!");
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
        }

        private void SpeakAndPrint(string text)
        {
            Console.WriteLine(text);
            this.speaker.Speak(text);
        }
    }
}
