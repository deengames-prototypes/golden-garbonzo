using Prototype.Game.Battle;
using Prototype.Game.Models;
using Prototype.Game.Models.Items;
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

        // TODO: extract to class. Extract each command into a subclass.
        private void ProcessInput(string input)
        {
            var inputTokens = input.ToUpperInvariant().Split(' ');
            var command = inputTokens[0];

            switch (command)
            {
                case "HELP":
                    SpeakAndPrint("Type 'quit' to quit, 'list' or 'l' to list the current room; type ATTACK to attack a target, GO to go somewhere, GET to get something, or OPTIONS to change options");
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
                case "GET":
                    this.ProcessGet(inputTokens);
                    break;
                case "GO":
                case "G":
                    this.ProcessMove(inputTokens);
                    break;
                case "INVENTORY":
                case "INV":
                case "I":
                    this.ListInventory();
                    break;
                case "OPTIONS":
                case "O":
                    this.ProcessOptions(inputTokens);
                    break;
                case "PUT":
                case "P":
                    this.ProcessPut(inputTokens);
                    break;
                // TODO: use stairs: make sure there is no socket or it's solved
                case "QUIT":
                case "Q":
                case "EXIT":
                    return;
                default:
                    SpeakAndPrint($"Not sure how to {input}");
                    break;
            }
        }

        private void ProcessPut(string[] inputTokens)
        {
            if (inputTokens.Length < 2)
            {
                SpeakAndPrint("Type PUT then the name of the item in your inventory.");
            }
            else
            {
                var itemName = inputTokens[1].ToUpperInvariant();
                var item = this.player.Inventory.FirstOrDefault(a => a.Name.ToUpperInvariant().Contains(itemName));
                if (item == null)
                {
                    SpeakAndPrint($"You don't have any {itemName}.");
                }
                else
                {
                    if (this.currentRoom.Socket == null)
                    {
                        SpeakAndPrint($"There's nowhere to put the {item.Name}.");
                    }
                    else if (this.currentRoom.Socket.IsSolved())
                    {
                        SpeakAndPrint($"The socket is already full of gems.");
                    }
                    else if (!(item is Gemstone))
                    {
                        SpeakAndPrint("You can't put that.");
                    }
                    else
                    {
                        // Socket is non-null, unsolved, and the item is a gem.
                        this.player.Inventory.Remove(item);
                        this.currentRoom.Socket.Socket(item as Gemstone);
                        SpeakAndPrint($"You place the {item.Name} in the gem socket. It clicks into place.");

                        if (this.currentRoom.Socket.IsSolved())
                        {
                            SpeakAndPrint($"The stairs {(this.currentRoom.Stairs == Enums.StairsType.NEXT_FLOOR ? "down" : "up")} appear!");
                        }
                    }
                }
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
                    if (results.Winner == player && target.Item != null)
                    {
                        var item = target.Item;
                        this.currentRoom.AddItem(item);
                        this.SpeakAndPrint($"The {target.Name} drops a {item.Name}.");
                        target.Item = null;
                    }
                    
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
            if (inputTokens.Length != 2)
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
                        var targetRoom = this.currentRoom.GetConnection(targetName); // validates the typed name is a real room that exists
                        if (targetRoom.IsLocked)
                        {
                            if (this.player.Inventory.Any(i => i is DoorKey))
                            {
                                SpeakAndPrint($"You unlock the {targetRoom.Id} room and go in.");
                                this.currentRoom = targetRoom;
                                SpeakAndPrint(this.currentRoom.GetContents());
                            }
                            else
                            {
                                SpeakAndPrint($"The {targetRoom.Id} room is locked.");
                            }
                        }
                        else
                        {
                            this.currentRoom = targetRoom;
                            SpeakAndPrint(this.currentRoom.GetContents());
                        }
                    }
                }
            }
        }

        private void ProcessGet(string[] inputTokens)
        {
            if (inputTokens.Length != 2)
            {
                SpeakAndPrint("Type get and then the item to get. Type list to list all items in this room.");
            }
            else
            {
                var targetName = inputTokens[1];
                var item = this.currentRoom.GetItem(targetName);
                if (item != null)
                {
                    this.player.Inventory.Add(item);
                    this.currentRoom.RemoveItem(item);
                    SpeakAndPrint($"You pick up the {item.Name}");
                }
                else
                {
                    SpeakAndPrint($"There doesn't seem to be a {targetName} here.");
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

        private void ListInventory()
        {
            if (player.Inventory.Any())
            {
                this.SpeakAndPrint($"You're carrying {player.Inventory.Count} items:");
                for (var i = 0; i < player.Inventory.Count; i++)
                {
                    var item = player.Inventory[i];
                    this.SpeakAndPrint($"Item #{i + 1}: {item.Name}: {item.Description}");
                }
            }
            else
            {
                this.SpeakAndPrint("You are not carrying anything.");
            }
        }

        private void SpeakAndPrint(string text)
        {
            Console.WriteLine(text);
            this.speaker.Speak(text);
        }
    }
}
