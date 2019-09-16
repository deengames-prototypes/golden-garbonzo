using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prototype.Game.Models
{
    class Room
    {
        private static Random random = new Random();

        public string Id { get; private set;  }
        public readonly List<Monster> Monsters = new List<Monster>();

        private readonly int floorNum;
        private readonly List<Room> connectedTo = new List<Room>();

        public Room(int floorNum, string id, int numMonsters)
        {
            this.Id = id;
            this.floorNum = floorNum;
            this.GenerateMonsters(numMonsters);
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
            var builder = new StringBuilder();
            var aliveMonsters = this.Monsters.Where(m => m.CurrentHealth > 0);
            string numberOfMonsters = aliveMonsters.Any() ? $"the following {aliveMonsters.Count()}" : "no";
            builder.Append($"You are in the {this.Id} room on floor {this.floorNum}F. ");

            builder.Append($"This room contains {numberOfMonsters} monsters: ");
            foreach (var monsterGroup in aliveMonsters.GroupBy(m => m.Name))
            {
                builder.Append($"{monsterGroup.Count()} {monsterGroup.Key}s, ");
            }

            builder.Append(" This room connects to ");
            foreach (var room in this.connectedTo)
            {
                builder.Append($"The {room.Id} room, ");

                if (this.connectedTo.Count > 1 && room == this.connectedTo[this.connectedTo.Count - 2])
                {
                    builder.Append(" and ");
                }
            };

            return builder.ToString();
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
            return this.connectedTo.SingleOrDefault(r => r.Id.ToUpperInvariant().Contains(roomName.ToUpperInvariant()));
        }

        private void GenerateMonsters(int numMonsters)
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
                        if (picked < GlobalConfig.MONSTER_PROBABILITY[MonsterType.Strong] + weightSoFar)
                        {
                            this.Monsters.Add(Monster.Generate(MonsterType.Strong));
                        }
                        else
                        {
                            throw new InvalidOperationException("Random-weight algorithm is wrong");
                        }
                    }
                }
            }
        }
    }
}
