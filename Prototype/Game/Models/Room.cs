using System;
using System.Collections.Generic;

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
        public readonly List<Room> ConnectedTo = new List<Room>();

        public Room()
        {
            this.Id = unusedRoomIds[random.Next(unusedRoomIds.Count)];
            unusedRoomIds.Remove(this.Id);
        }

        public void ConnectTo(Room target)
        {
            if (!ConnectedTo.Contains(target))
            {
                ConnectedTo.Add(target);
            }
        }
    }
}
