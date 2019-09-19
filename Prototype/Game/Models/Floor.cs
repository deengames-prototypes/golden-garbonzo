using Prototype.Game.Enums;
using Prototype.Game.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Game.Models
{
    class Floor
    {
        private static Random random = new Random();

        internal List<Room> Rooms = new List<Room>();

        private readonly List<string> unusedRoomIds = new List<string>()
        {
            "Ape", "Bison", "Cat", "Dog", "Eagle", "Fish", "Goat", "Hamster", "Kangaroo", "Lion",
            "Monkey", "Nighthawk", "Oppossum", "Pony", "Raccoon", "Snail", "Tiger", "Whale", "Yak", "Zebra"
        };

        public Floor(int floorNum)
        {
            var numRooms = random.Next(GlobalConfig.MIN_ROOMS_PER_FLOOR, GlobalConfig.MAX_ROOMS_PER_FLOOR); // 5-8 rooms
            while (this.Rooms.Count < numRooms)
            {
                // 40% chance of an empty room
                var numMonsters = random.Next(GlobalConfig.MIN_MONSTERS_PER_FLOOR, GlobalConfig.MAX_MONSTERS_PER_FLOOR);

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

        /// <summary>
        /// Creates a gem socket in the final room, and hides the stairs.
        /// </summary>
        internal void CreateGemSocketAndGems()
        {
            var stairsDownRoom = this.Rooms.Single(r => r.Stairs == StairsType.NEXT_FLOOR);
            stairsDownRoom.CreateGemSocket();

            var allMonsters = new List<Monster>();
            this.Rooms.ForEach(r => r.Monsters.Where(m => m.Item == null).ToList().ForEach(m => allMonsters.Add(m)));

            // Assumes two gems per socket.
            var monsters = allMonsters.OrderBy(r => random.Next()).Take(2);
            foreach (var monster in monsters)
            {
                monster.Item = new Gemstone();
            }
        }

        public void CreateKeyAndLockFinalRoom()
        {
            var roomToLock = this.Rooms.Last();
            roomToLock.IsLocked = true;

            var allMonsters = new List<Monster>();
            this.Rooms.Where(r => r != roomToLock).ToList().ForEach(r => allMonsters.AddRange(r.Monsters.Where(m => m.Item == null)));

            var whichMonster = random.Next(allMonsters.Count);
            var monster = allMonsters[whichMonster];
            monster.Item = new DoorKey();
        }
        
        public void SealRandomRoom()
        {
            var roomIndex = 0;
            
            // Don't seal the starting/final rooms (with stairs)
            while (roomIndex == 0 || roomIndex == this.Rooms.Count - 1 || this.Rooms[roomIndex].IsLocked)
            {
                roomIndex = random.Next(this.Rooms.Count - 1);
            }

            var sealedRoom = this.Rooms[roomIndex];
            sealedRoom.IsSealed = true;
        }
    }
}
