using Prototype.Game.Battle;
using Prototype.Game.Enums;
using Prototype.Game.Models;
using Prototype.Game.Models.Items;
using Prototype.Game.Models.Items.Assemblable;
using Prototype.TextToSpeech;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        // This shouldn't be here?
        private readonly Dictionary<Skill, string> SkillKeyBinding = new Dictionary<Skill, string>()
        {
            { Skill.Heal, "HEAL" },
            { Skill.StoneSkin, "STONE" },
            { Skill.PhaseShield, "PHASE" },
            { Skill.Kick, "KICK" },
            { Skill.Focus, "FOCUS" },
            { Skill.NanoSwarm, "SWARM" },
        };

        public void Run()
        {
            this.speaker = new DummySpeaker();
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
                //SpeakAndPrint($"You typed: {input}");
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
                    if (this.currentRoom is MachineRoom)
                    {
                        SpeakAndPrint(MachineRoom.HelpText);
                    }
                    else
                    {
                        SpeakAndPrint(@"Commands: quit to quit, L or list to see room contents, " +
                            "ATTACK and then an enemy name to attack a target, GO to go somewhere, GET to get something, and sometimes you can PUT items into things." +
                            "Type I or INVENTORY to list items you picked up. You can type USE to use room fixtures. Type STAIRS to use stairs." +
                            "Type stats or c to see your player stats.  You can also type OPTIONS to list or change options.");
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
                case "CHARACTER":
                case "STATS":
                case "C":
                case "S":
                    this.ShowPlayerStats();
                    break;
                case "GET":
                    this.ProcessGet(inputTokens);
                    break;

                case "GO":
                case "G":
                    this.ProcessMove(inputTokens);
                    break;

                // Skills
                case "HEAL":
                case "H":
                    this.TrySkill(Skill.Heal);
                    break;
                case "STONE SKIN":
                case "STONE":
                case "SS":
                    this.TrySkill(Skill.StoneSkin);
                    break;
                case "PHASE SHIELD":
                case "PHASE":
                case "PS":
                    this.TrySkill(Skill.PhaseShield);
                    break;
                case "KICK":
                case "K":
                    this.ProcessKick(inputTokens);
                    break;
                case "FOCUS":
                    this.TrySkill(Skill.Focus);
                    break;
                case "NANOSWARM":
                case "SWARM":
                case "NS":
                    this.TrySkill(Skill.NanoSwarm);
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
                case "CRAFT":
                case "CR":
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

        private void TrySkill(Enums.Skill skill)
        {
            var skillCost = Player.SkillCosts[skill];
            if (!player.Skills.Contains(skill))
            {
                SpeakAndPrint($"You haven't learned {skill.ToString()}.");
            }
            else if (player.CurrentSkillPoints < skillCost)
            {
                SpeakAndPrint($"Not enough skill points (have {player.CurrentSkillPoints}, need {skillCost})");
            }
            else
            {
                var message = SkillExecutor.Execute(skill, player, currentRoom.Monsters);
                player.CurrentSkillPoints -= skillCost;
                SpeakAndPrint(message);
            }
        }

        private void ShowPlayerStats()
        {
            var message = $"You have {player.CurrentHealth} out of {player.TotalHealth} health, " +
                $"{player.CurrentSkillPoints} out of {player.TotalSkillPoints} skill points, {player.ExperiencePoints} experience points. " +
                $" Your have {this.player.Skills.Count} skills: {string.Join(", ", this.player.Skills.Select(s => s.ToString()))}. " +
                $" You have {player.Strength} strength and {player.Defense} defense. ";

            if (player.HasStoneSkin())
            {
                message += " Stone reinforces your skin.";
            }

            if (player.PhaseShieldLeft > 0)
            {
                message += $" You have a phase shield that can absorb {player.PhaseShieldLeft} more damage.";
            }

            if (player.IsFocused)
            {
                message += " You are intensely focused.";
            }

            this.SpeakAndPrint(message);
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

        private void ProcessKick(string[] inputTokens)
        {
            // This is a copy-paste of ProcessAttack
            if (inputTokens.Length != 2)
            {
                SpeakAndPrint("Type kick and then the name of the target. Type list to list targets.");
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
                    // This is a copy-paste of TrySkill(Skill.Kick)
                    var skill = Skill.Kick;
                    var skillCost = Player.SkillCosts[skill];
                    if (!player.Skills.Contains(skill))
                    {
                        SpeakAndPrint($"You haven't learned {skill.ToString()}.");
                    }
                    else if (player.CurrentSkillPoints < skillCost)
                    {
                        SpeakAndPrint($"Not enough skill points (have {player.CurrentSkillPoints}, need {skillCost})");
                    }
                    else
                    {
                        var target = this.currentRoom.GetMonster(targetName);
                        var damage = (int)(player.Strength * Player.KICK_MULTIPLIER) - target.Defense;
                        SpeakAndPrint($"You kick the {target.Name} for {damage} damage!");
                        target.CurrentHealth -= damage;

                        // Start a battle
                        if (target.CurrentHealth > 0)
                        {
                            this.ProcessAttack(inputTokens);
                        }
                        else
                        {
                            this.SpeakAndPrint($"The {target.Name} dies!");
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
                            winnerMessage = results.Winner == player ? $"you vanquish your foe! You have {player.CurrentHealth} out of {player.TotalHealth} health." : $"you collapse to the ground in a heap! (The {target.Name} has {target.CurrentHealth} out of {target.TotalHealth} health remaining.)";
                            break;
                        case SpeechMode.Summary:
                            winnerMessage = results.Winner == player ? $"you win!" : $"you lose! {target.Name} has {target.CurrentHealth} health left.";
                            break;
                    }                    

                    SpeakAndPrint($"You attack a {target.Name}!", "Battle begins!");

                    switch (Options.CombatType)
                    {
                        case CombatType.RoundByRound:
                            foreach (var round in results.RoundMessages)
                            {
                                SpeakAndPrint(round);
                            }
                            break;
                        case CombatType.Summary:
                            var messages = results.RoundMessages.Where(r => r.ToUpperInvariant().Contains("SKIN") || r.ToUpperInvariant().Contains("FOCUS"));
                            foreach (var message in messages)
                            {
                                SpeakAndPrint(message);
                            }
                            break;
                    }

                    SpeakAndPrint($"After {(int)Math.Ceiling(results.RoundMessages.Length / 2f)} rounds, {winnerMessage}", winnerMessage);
                    if (results.Winner == player)
                    {
                        if (target.Item != null)
                        {
                            var item = target.Item;
                            this.currentRoom.AddItem(item);
                            this.SpeakAndPrint($"The {target.Name} drops a {item.Name}.");
                            target.Item = null;
                        }

                        var didLevelUp = player.GainExperience(target.ExperiencePoints);
                        var message = $"You get {target.ExperiencePoints} experience.";
                        this.SpeakAndPrint(message);

                        if (didLevelUp)
                        {
                            this.SpeakAndPrint($" You reached level {player.Level}!");

                            // Grant a skill
                            if (Player.SkillChoicesByLevel.ContainsKey(player.Level))
                            {
                                var skills = Player.SkillChoicesByLevel[player.Level];

                                var input = "";
                                int response = 0;
                                while (!int.TryParse(input, out response) || response < 1 || response > skills.Count)
                                {
                                    this.SpeakAndPrint("Type the number of the skill to learn, or question-mark to list them again.");
                                    this.EnumerateSkills(skills);
                                    input = Console.ReadLine();
                                }

                                var pickedSkill = skills[response - 1];
                                player.Skills.Add(pickedSkill);
                                var skillKey = SkillKeyBinding[pickedSkill];
                                this.SpeakAndPrint($"You learned {pickedSkill.ToString()}. Type {skillKey} when you want to use it.");
                            }

                            // Grant a +5 to HP or +1 to str/def
                            var choice = "";
                            int asInt = 0;
                            while (!int.TryParse(choice, out asInt) || asInt < 1 || asInt > 3)
                            {
                                this.SpeakAndPrint(" You gained a permanent stats boost. Type 1 to increase your maximum health, type 2 to increase strength, type 3 to increase defense.");
                                choice = Console.ReadLine();
                            }
                            switch (asInt)
                            {
                                case 1:
                                    player.TotalHealth += Player.HEALTH_GROWTH_ON_STAT_POINT;
                                    player.CurrentHealth += Player.HEALTH_GROWTH_ON_STAT_POINT;
                                    this.SpeakAndPrint($" You gain {Player.HEALTH_GROWTH_ON_STAT_POINT} health. You now have {player.TotalHealth} total health.");
                                    break;
                                case 2:
                                    player.Strength += Player.STR_DEF_GROWTH_ON_STAT_POINT;
                                    this.SpeakAndPrint($" You gain a strength point; you now have {player.Strength}.");
                                    break;
                                case 3:
                                    player.Defense += Player.STR_DEF_GROWTH_ON_STAT_POINT;
                                    this.SpeakAndPrint($" You gain a defense point; you now have {player.Defense}. ");
                                    break;

                            }

                        }
                    }
                    else
                    {
                        player.CurrentHealth = 1;
                    }
                    
                    if (this.currentRoom.IsSealed && !this.currentRoom.Monsters.Any(m => m.CurrentHealth > 0) && results.Winner == player)
                    {
                        this.currentRoom.IsSealed = false;
                        SpeakAndPrint("The forcefield surrounding the room disappears.");
                    }
                }
            }
        }

        private void EnumerateSkills(List<Skill> skills)
        {
            this.SpeakAndPrint(" Please pick one of the following skills:");

            for (var i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];

                // Um. Use reflection to look up the [Description] that we annotated. Makes lots of assumptions, like a) enum
                // value is valid, and 2) the attribute has a single description.
                // Source: https://codereview.stackexchange.com/a/157981
                var description = (skill.GetType().GetMember(skill.ToString()).Single()
                    .GetCustomAttributes(typeof(DescriptionAttribute), false).Single() as DescriptionAttribute).Description;

                this.SpeakAndPrint($" {i + 1}: {skill}: {description}");
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
                    SpeakAndPrint("You can't leave - all the doors are blocked by a forcefield!", "A forcefield blocks the doors.");
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
                            player.PostMove();
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
            var text = shortFormText == null || Options.SpeechMode == SpeechMode.Detailed ? longFormText : shortFormText;
            Console.WriteLine(text);
            this.speaker.Speak(text);
        }
    }
}
