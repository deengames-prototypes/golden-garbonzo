using System;
using System.Collections.Generic;
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
        private readonly List<Room> connectedTo = new List<Room>();

        public Room()
        {
            this.Id = unusedRoomIds[random.Next(unusedRoomIds.Count)];
            unusedRoomIds.Remove(this.Id);
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
    }
}
