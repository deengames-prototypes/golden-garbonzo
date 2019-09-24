using Prototype.Game.Battle;
using Prototype.Game.Models;
using Prototype.Game.Models.Items;
using Prototype.Game.Models.Items.Assemblable;
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
        private Dungeon dungeon = new Dungeon();
        private int currentFloor = 0;

        public void Run()
        {
            this.speaker = new MicrosoftSpeaker();
            this.currentRoom = dungeon.Floors[currentFloor].Rooms[0];

            SpeakAndPrint($"Welcome to the dungeon! {this.currentRoom.GetContents()}");
            SpeakAndPrint("Type something and press enter. Type 'help' for help, or 'quit' to quit.");

            this.MainProcessingLoop();

            this.speaker.FinishSpeaking();
        }

        private void MainProcessingLoop()
        {
            string input = "";

            while (input.Trim().ToUpper() != "QUIT" && input.ToUpper() != "Q")
            {
                SpeakAndPrint("Your command? ", "");
                Console.Write("> ");
                input = Console.ReadLine();
                this.speaker.StopAndClearQueue();
                SpeakAndPrint($"You typed: {input}", "");
                this.ProcessInput(input);
            }

            this.SpeakAndPrint("Bye!");
        }

        // TODO: extract to class. Extract each command into a subclass.
        private void ProcessInput(string input)
        {
            var inputTokens = input.ToUpperInvariant().Split(' ');
            var command = inputTokens[0];
            if (Options.RepeatInput)
            {
                SpeakAndPrint($"You typed {input}");
            }

            switch (command)
            {
                case "HELP":
                    if (this.currentRoom is MachineRoom)
                    {
                        SpeakAndPrint(MachineRoom.HelpText);
                    }
                    else
                    {
                        SpeakAndPrint("Type 'quit' to quit, 'list' or 'l' to list the current room; type ATTACK to attack a target, GO to go somewhere, GET to get something, or OPTIONS to change options");
                    }
                    break;
                // TODO: need a command to approach the machine
                case "LEAVE":
                    if (currentRoom is MachineRoom)
                    {
                        currentRoom = (currentRoom as MachineRoom).ContainingRoom;
                        this.SpeakAndPrint("You step away from the machine.");
                        this.SpeakAndPrint(currentRoom.GetContents());
                    }
                    else
                    {
                        SpeakAndPrint("Can't leave.");
                    }
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

                case "U":
                case "USE":                    
                case "B":
                case "BUILD":
                case "C":
                case "CRAFT":
                    if (command.StartsWith("U") && this.currentRoom.HasMachine())
                    {
                        this.currentRoom = this.currentRoom.GetConnection("Machine");
                        this.SpeakAndPrint(this.currentRoom.GetContents());
                    }
                    else
                    {
                        this.UseWorkBench();
                    }
                    break;
                case "STAIRS":
                    this.UseStairsIfPresent();
                    break;
                case "SWITCH":
                    if (this.currentRoom is MachineRoom)
                    {
                        this.ProcessSwitch(inputTokens);
                    }
                    else
                    {
                        SpeakAndPrint($"Not sure how to switch.");
                    }
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

        private void UseStairsIfPresent()
        {
            if (this.currentRoom.Stairs == Enums.StairsType.NEXT_FLOOR)
            {
                this.currentFloor++;
                this.currentRoom = this.dungeon.Floors[currentFloor].Rooms[0];
                this.SpeakAndPrint($"You step on the stair teleporter and materialize on the next floor. {this.currentRoom.GetContents()}");
            }
            else if (this.currentRoom.Stairs == Enums.StairsType.PREVIOUS_FLOOR)
            {
                this.currentFloor--;
                this.currentRoom = this.dungeon.Floors[currentFloor].Rooms[0];
                this.SpeakAndPrint($"You step on the stair teleporter and materialize on the previous floor. {this.currentRoom.GetContents()}");
            }
            else if (this.currentRoom.Stairs == Enums.StairsType.ESCAPE)
            {
                this.SpeakAndPrint(@"You pounce on the teleporter pad and jam the console button. The dungeon disappears in a wash of cold air and white light.
                    You regain your senses in a stadium, thousands of cheering fans around you. 'Congratulations to our challenger!' a man shots, 'the first to conquer the dungeon and emerge victorious in over 100 years!'
                    Memories flood back of the competition - the game, the entry, the prizes. You can look forward to a luxurious life from now on.
                    Congratulations on completing the game! We hope you enjoyed playing it as much as we enjoyed making it. 
                    Please contact NightBlade with your feedback so we can improve it. We look forward to hearing from you!
                    Bye!");

                this.speaker.FinishSpeaking();
                Environment.Exit(0);
            }
            else
            {
                SpeakAndPrint("There are no stairs here.");
            }
        }

        private void ProcessSwitch(string[] inputTokens)
        {
            var machineRoom = this.currentRoom as MachineRoom;
            if (machineRoom != null)
            {
                int switchNumber = -1;
                if (inputTokens.Length >= 2 && int.TryParse(inputTokens[1], out switchNumber) && switchNumber >= 1 && switchNumber <= 3)
                {
                    machineRoom.Switch(switchNumber - 1);
                    var nth = "";
                    switch (switchNumber)
                    {
                        case 1: nth = "first";
                            break;
                        case 2: nth = "second";
                            break;
                        case 3: nth = "third";
                            break;
                        default:
                            nth = "";
                            break;
                    }

                    this.SpeakAndPrint($"You press the keypad for the {nth} switch. The energy {machineRoom.GetEnergyPoint()}.");
                    if (machineRoom.IsSolved() && machineRoom.Gem != null)
                    {
                        this.player.Inventory.Add(machineRoom.Gem);
                        this.SpeakAndPrint($"The machine whirs at a higher and higher pitch. {machineRoom.Gem.Description} materializes in front of you. Hesitantly, you touch it, then pocket it.");
                        machineRoom.Gem = null;
                    }
                }
                else
                {
                    this.SpeakAndPrint("Type switch, and a switch number from 1 to 3.");
                }
            }
        }

        private void UseWorkBench()
        {
            if (this.currentRoom.WorkBench == null)
            {
                SpeakAndPrint("There's no workbench here.");
            }
            else
            {
                var buildableType = this.player.GetBuildableType(this.currentRoom.WorkBench);
                if (buildableType == null)
                {
                    SpeakAndPrint("You don't have enough parts to build anything significant yet.", "You don't have the parts.");
                }
                else
                {
                    this.currentRoom.WorkBench.Assemble(buildableType, player);
                    var item = (AbstractItem)Activator.CreateInstance(buildableType);
                    player.Inventory.Add(item);
                    var parts = this.currentRoom.WorkBench.GetPartsFor(buildableType);
                    SpeakAndPrint($"You build the {item.Name} out of the {string.Join(", ", parts)}. ");
                }
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
                    // TODO: this was a mistake, the machine room should have its own separate processing logic. Not here.
                    if (this.currentRoom is MachineRoom)
                    {
                        var machineRoom = currentRoom as MachineRoom;

                        if (item is PowerCube)
                        {
                            machineRoom.InsertedPowerCube = true;
                            this.player.Inventory.Remove(item);
                            this.SpeakAndPrint("You place the power cube in the alcove. The machine clicks and whirs to life.");
                            if (Options.SpeechMode == SpeechMode.Detailed)
                            {
                                this.SpeakAndPrint("A breeze from the machine parts blows over you.");
                            }
                        }
                        else if (machineRoom.InsertedPowerCube == true)
                        {
                            this.SpeakAndPrint("You already put the power cube in the machine.");
                        }
                        else
                        {
                            this.SpeakAndPrint($"You twist and push the {item.Name} but it doesn't fit in the alcove.");
                        }
                    }
                    else if (this.currentRoom.Socket == null)
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
                            SpeakAndPrint($"The stairs leading {this.currentRoom.StairsString()} appear!");
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
                    SpeakAndPrint($"There is no {targetName} here");
                }
                else
                {
                    var target = this.currentRoom.GetMonster(targetName);
                    var battler = new Battler(this.player, target);
                    var results = battler.FightToTheDeath();

                    var winnerMessage = "";
                    switch (Options.SpeechMode)
                    {
                        case SpeechMode.Detailed:
                            winnerMessage = results.Winner == player ? $"you vanquish your foe! You had {player.CurrentHealth} out of {player.TotalHealth} health." : $"you collapse to the ground in a heap! (The {target.Name} has {target.CurrentHealth} out of {target.TotalHealth} health remaining.)";
                            break;
                        case SpeechMode.Summary:
                            winnerMessage = results.Winner == player ? $"you win!" : $"You die! {target.Name} has {target.CurrentHealth} health left.";
                            break;
                    }                    

                    player.CurrentHealth = player.TotalHealth;
                    SpeakAndPrint($"You attack a {target.Name}!", "Battle begins!");

                    switch (Options.CombatType)
                    {
                        case CombatType.RoundByRound:
                            foreach (var round in results.RoundMessages)
                            {
                                SpeakAndPrint(round);
                            }
                            break;
                    }

                    SpeakAndPrint($"After {results.RoundMessages.Length} rounds, {winnerMessage}", winnerMessage);
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
                        SpeakAndPrint("The pressurized seals on all the doors dissipate.");
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
                    SpeakAndPrint("You can't leave - all the doors are sealed shut!", "The doors are sealed.");
                }
                else
                {
                    var targetName = inputTokens[1];
                    if (targetName.ToUpperInvariant().Contains("MACHINE") || !this.currentRoom.IsConnectedTo(targetName))
                    {
                        SpeakAndPrint($"There doesn't seem to be a way to go to {targetName} from here.", $"Can't go to {targetName} from here.");
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
                this.SpeakAndPrint($"Type options and then the name of the option to change it. Current options: ");
                this.SpeakAndPrint($"Combat is {Options.CombatType}, ");
                this.SpeakAndPrint($"Speech is {Options.SpeechMode}");
            }
            else
            {
                var targetName = inputTokens[1].ToUpperInvariant();
                if (targetName != "COMBAT" && targetName != "SPEECH")
                {
                    SpeakAndPrint($"There doesn't seem to be a {targetName} option. Valid options are: COMBAT, SPEECH");
                }
                else
                {
                    switch (targetName)
                    {
                        case "COMBAT":
                            var newMode = Options.CombatType == CombatType.RoundByRound ? CombatType.Summary : CombatType.RoundByRound;
                            this.SpeakAndPrint($"Combat changed from {Options.CombatType.ToString()} to {newMode}");
                            Options.CombatType = newMode;
                            break;
                        case "SPEECH":
                            var newSpeech = Options.SpeechMode == SpeechMode.Detailed ? SpeechMode.Summary : SpeechMode.Detailed;
                            this.SpeakAndPrint($"Speech changed from {Options.SpeechMode.ToString()} to {newSpeech}");
                            Options.SpeechMode = newSpeech;
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

        private void SpeakAndPrint(string longFormText, string shortFormText = null)
        {
            var text = shortFormText == null ? longFormText : shortFormText;
            Console.WriteLine(text);
            this.speaker.Speak(text);
        }
    }
}
