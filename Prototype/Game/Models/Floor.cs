using Prototype.Game.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Game.Models
{
    class Floor
    {
        private static Random random = new Random();

        internal List<Room> Rooms = new List<Room>();

        private static readonly List<string> unusedRoomIds = new List<string>()
        {
            "Ape", "Bison", "Cat", "Dog", "Eagle", "Fish", "Goat", "Hamster", "Kangaroo", "Lion",
            "Monkey", "Nighthawk", "Oppossum", "Pony", "Raccoon", "Seal", "Tiger", "Whale", "Yak", "Zebra"
        };

        public Floor(int floorNum)
        {
            var numRooms = random.Next(GlobalConfig.MIN_ROOMS_PER_FLOOR, GlobalConfig.MAX_ROOMS_PER_FLOOR); // 5-8 rooms
            while (this.Rooms.Count < numRooms)
            {
                // 40% chance of an empty room
                var numMonsters = random.NextDouble() <= GlobalConfig.PROBABILITY_OF_NO_MONSTERS ?
                    0 : random.Next(GlobalConfig.MIN_MONSTERS, GlobalConfig.MAX_MONSTERS);

                var roomId = unusedRoomIds[random.Next(unusedRoomIds.Count)];
                unusedRoomIds.Remove(roomId);

                var room = new Room(floorNum, roomId, numMonsters);
                // Guaranteed connectedness: each room connects to the next
                if (this.Rooms.Any())
                {
                    this.Rooms[this.Rooms.Count - 1].ConnectTo(room);
                }

                this.Rooms.Add(room);
            }

            // Also connect each room to a random target
            for (var i = 0; i < this.Rooms.Count; i++)
            {
                var room = this.Rooms[i];
                var target = room;

                while (target == room || room.IsConnectedTo(target))
                {
                    target = this.Rooms[random.Next(this.Rooms.Count)];
                }

                room.ConnectTo(target);
                target.ConnectTo(room);
            }

            // Predictably, first and last room are stairs up/down respectively.
            if (floorNum > 1)
            {
                this.Rooms[0].Stairs = StairsType.PREVIOUS_FLOOR;
            }

            if (floorNum < GlobalConfig.NUM_FLOORS)
            {
                this.Rooms.Last().Stairs = StairsType.NEXT_FLOOR;
            }
        }
    }
}
