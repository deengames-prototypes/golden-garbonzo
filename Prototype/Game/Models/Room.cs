using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prototype.Game.Models
{
    class Room
    {
        private static Random random = new Random();

        private static readonly List<string> unusedRoomIds = new List<string>()
        {
            "Ape", "Bison", "Cat", "Dog", "Eagle", "Fish", "Goat", "Hamster", "Kangaroo", "Lion",
            "Monkey", "Newt", "Oppossum", "Pony", "Raccoon", "Seal", "Tiger", "Whale", "Yak", "Zebra"
        };

        public string Id { get; private set;  }
        public readonly List<Monster> Monsters = new List<Monster>();

        private readonly List<Room> connectedTo = new List<Room>();

        public Room(int numMonsters)
        {
            this.Id = unusedRoomIds[random.Next(unusedRoomIds.Count)];
            unusedRoomIds.Remove(this.Id);

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

        public void ConnectTo(Room target)
        {
            if (!connectedTo.Contains(target))
            {
                connectedTo.Add(target);
            }
        }

        public string GetContents()
        {
            var builder = new StringBuilder();
            builder.Append($"You are in the {this.Id} room. This room connects to ");

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
    }
}
