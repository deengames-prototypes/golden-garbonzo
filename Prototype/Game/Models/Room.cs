using Prototype.Game.Enums;
using Prototype.Game.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prototype.Game.Models
{
    class Room
    {
        private static Random random = new Random();

        public string Id { get; private set; }
        public readonly List<Monster> Monsters = new List<Monster>();
        public bool IsSealed { get; set; }
        public GemSocket Socket { get; private set; }
        public WorkBench WorkBench { get; private set; }

        internal StairsType Stairs = StairsType.NONE;
        internal bool IsLocked = false;

        private readonly int floorNum;
        private readonly List<Room> connectedTo = new List<Room>();
        private List<AbstractItem> items = new List<AbstractItem>();

        public Room(int floorNum, string id, int numMonsters, int maxGoblins)
        {
            this.Id = id;
            this.floorNum = floorNum;
            this.GenerateMonsters(numMonsters, maxGoblins);
        }

        public void ConnectTo(Room target)
        {
            if (!connectedTo.Contains(target))
            {
                connectedTo.Add(target);
            }
        }

        public Monster GetMonster(string monsterName)
        {
            return this.Monsters.FirstOrDefault(m => m.CurrentHealth > 0 && m.Name.ToUpperInvariant().Contains(monsterName.ToUpperInvariant()));
        }

        public bool HasMonster(string monsterName)
        {
            return this.GetMonster(monsterName) != null;
        }

        public string GetContents()
        {
            switch (Options.SpeechMode)
            {
                case SpeechMode.Detailed: return DetailedGetContents();
                case SpeechMode.Summary: return ShortGetContents();
                default: return DetailedGetContents();
            }
        }

        internal void CreateGemSocket()
        {
            this.Socket = new GemSocket(2);
        }

        internal void CreateWorkBench()
        {
            this.WorkBench = new WorkBench();
        }

        internal bool IsConnectedTo(Room target)
        {
            return this.connectedTo.Contains(target);
        }

        internal bool IsConnectedTo(string roomName)
        {
            return this.GetConnection(roomName) != null;
        }

        internal Room GetConnection(string roomName)
        {
            return this.connectedTo.FirstOrDefault(r => r.Id.ToUpperInvariant().Contains(roomName.ToUpperInvariant()));
        }

        internal bool HasMachine()
        {
            return this.connectedTo.SingleOrDefault(r => r is MachineRoom) != null;
        }

        internal string StairsString()
        {
            switch (this.Stairs)
            {
                case StairsType.PREVIOUS_FLOOR:
                    return "down";
                case StairsType.NEXT_FLOOR:
                    return "up";
                case StairsType.ESCAPE:
                    return "out of here";
                default:
                    throw new ArgumentException(this.Stairs.ToString());
            }
        }

        virtual protected string DetailedGetContents()
        {
            var builder = new StringBuilder();
            var aliveMonsters = this.Monsters.Where(m => m.CurrentHealth > 0);
            string numberOfMonsters = aliveMonsters.Any() ? $"the following {aliveMonsters.Count()}" : "no";

            builder.Append($"You are in the {this.Id} room on floor {this.floorNum}F. ");

            builder.Append($"This room contains {numberOfMonsters} monsters: ");
            foreach (var monsterGroup in aliveMonsters.GroupBy(m => m.Name))
            {
                builder.Append($"{monsterGroup.Count()} {monsterGroup.Key}s, ");
            }


            if (!this.IsSealed)
            {
                builder.Append(" This room connects to ");
                foreach (var room in this.connectedTo.Where(r => !(r is MachineRoom)))
                {
                    builder.Append($"The {room.Id} room, ");

                    if (this.connectedTo.Count > 1 && room == this.connectedTo[this.connectedTo.Count - 2])
                    {
                        builder.Append(" and ");
                    }
                };
            }
            else
            {
                builder.Append("All the doors are sealed shut!");
            }

            // Presence of an incomplete gem socket hides stairs
            if (this.Socket == null || this.Socket.IsSolved())
            {
                if (this.Stairs != StairsType.NONE)
                {
                    builder.Append($". You see stairs leading {this.StairsString()}.");
                }
            }
            else
            {
                // Socket present and unsolved
                var socketsMessage = "";
                if (Socket.GemsSocketed > 0)
                {
                    socketsMessage = $"{Socket.GemsSocketed} of them hold gems. ";
                }
                builder.Append($". You see a socket in the wall with {Socket.RequiredGems} slots in it. {socketsMessage} ");
            }

            if (this.WorkBench != null)
            {
                builder.Append("You see a workbench here. You can use it to build things.");
            }

            if (this.connectedTo.Any(r => r is MachineRoom))
            {
                builder.Append("You see a huge machine here. You can USE it.");
            }

            if (this.items.Any())
            {
                this.items.ForEach(i => builder.Append($" You see a {i.Name}. "));
            }

            return builder.ToString();
        }

        virtual protected string ShortGetContents()
        {
            var builder = new StringBuilder();
            var aliveMonsters = this.Monsters.Where(m => m.CurrentHealth > 0);
            string numberOfMonsters = aliveMonsters.Any() ? $"the following {aliveMonsters.Count()}" : "no";

            builder.Append($"{this.Id} room on {this.floorNum}F. ");

            builder.Append($"Monsters: ");
            foreach (var monsterGroup in aliveMonsters.GroupBy(m => m.Name))
            {
                builder.Append($"{monsterGroup.Count()} {monsterGroup.Key}s, ");
            }


            if (!this.IsSealed)
            {
                builder.Append(" Connects to ");
                var rooms = this.connectedTo.Where(r => !(r is MachineRoom));

                if (rooms.Any())
                {
                    foreach (var room in rooms)
                    {
                        builder.Append($"{room.Id}, ");
                    };
                }
                else
                {
                    builder.Append("no other");
                }
                builder.Append(" rooms.");
            }
            else
            {
                builder.Append("The doors are sealed!");
            }

            // Presence of an incomplete gem socket hides stairs
            if (this.Socket == null || this.Socket.IsSolved())
            {
                if (this.Stairs != StairsType.NONE)
                {
                    builder.Append($". You see stairs leading {this.StairsString()}.");
                }
            }
            else
            {
                // Socket present and unsolved
                var socketsMessage = "";
                if (Socket.GemsSocketed > 0)
                {
                    socketsMessage = $"{Socket.GemsSocketed} of them hold gems. ";
                }
                builder.Append($". You see a socket in the wall with {Socket.RequiredGems} slots in it. {socketsMessage} ");
            }

            if (this.WorkBench != null)
            {
                builder.Append("You see a usable workbench here.");
            }

            if (this.connectedTo.Any(r => r is MachineRoom))
            {
                builder.Append("You see a huge machine here. You can USE it.");
            }

            if (this.items.Any())
            {
                this.items.ForEach(i => builder.Append($" You see a {i.Name}. "));
            }

            return builder.ToString();
        }

        internal void AddItem(AbstractItem item)
        {
            this.items.Add(item);
        }

        internal void RemoveItem(AbstractItem item)
        {
            this.items.Remove(item);
        }

        internal AbstractItem GetItem(string itemName)
        {
            var item = this.items.FirstOrDefault(i => i.Name.ToUpperInvariant().Contains(itemName.ToUpperInvariant()));
            return item;
        }

        internal void AddToRandomMonster(AbstractItem item, bool canBeOnGoblin = true)
        {
            var candidateMonsters = this.Monsters;
            if (!canBeOnGoblin)
            {
                candidateMonsters = candidateMonsters.Where(c => !c.Name.ToUpperInvariant().Contains("GOBLIN")).ToList();
            }

            var monster = candidateMonsters[random.Next(candidateMonsters.Count)];
            monster.Item = item;
        }

        private void GenerateMonsters(int numMonsters, int maxGoblins)
        {
            var totalMonsterProbability = GlobalConfig.MONSTER_PROBABILITY.Values.Sum();
            var weightSoFar = 0f;
            while (Monsters.Count < numMonsters)
            {
                var picked = random.NextDouble();
                if (picked < GlobalConfig.MONSTER_PROBABILITY[MonsterType.Weak] + weightSoFar)
                {
                    this.Monsters.Add(Monster.Generate(MonsterType.Weak));
                }
                else
                {
                    weightSoFar += GlobalConfig.MONSTER_PROBABILITY[MonsterType.Weak];
                    if (picked < GlobalConfig.MONSTER_PROBABILITY[MonsterType.Regular] + weightSoFar)
                    {
                        this.Monsters.Add(Monster.Generate(MonsterType.Regular));
                    }
                    else
                    {
                        weightSoFar += GlobalConfig.MONSTER_PROBABILITY[MonsterType.Regular];
                        if (maxGoblins > 0 && picked < GlobalConfig.MONSTER_PROBABILITY[MonsterType.Strong] + weightSoFar)
                        {
                            this.Monsters.Add(Monster.Generate(MonsterType.Strong));
                            Console.WriteLine($"Goblin on {this.floorNum}F");
                        }
                        else
                        {
                            // Maybe it was a goblin pick but we already have enough goblins. Oh well. Just pick one of the other two.
                            var isWeak = random.Next(100) < 30;
                            if (isWeak)
                            {
                                this.Monsters.Add(Monster.Generate(MonsterType.Weak));
                            }
                            else
                            {
                                this.Monsters.Add(Monster.Generate(MonsterType.Regular));
                            }
                        }
                    }
                }
            }
        }
    }
}
