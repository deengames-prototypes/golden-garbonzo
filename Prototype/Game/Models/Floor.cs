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

        private static readonly List<string> unusedRoomIds = new List<string>()
        {
            "Ape", "Bison", "Cat", "Dog", "Eagle", "Fish", "Goat", "Hamster", "Jaguar", "Kangaroo", "Lion",
            "Monkey", "Nighthawk", "Oppossum", "Pony", "Raccoon", "Snail", "Tiger", "Whale", "Yak", "Zebra"
        };

        public Floor(int floorNum)
        {
            var numRooms = GlobalConfig.MIN_ROOMS_PER_FLOOR;// random.Next(GlobalConfig.MIN_ROOMS_PER_FLOOR, GlobalConfig.MAX_ROOMS_PER_FLOOR); // 5-8 rooms
            
            // number of goblins for floor N: 0, 2, 4, ...
            var numGoblins = (floorNum - 1) * 2; 

            while (this.Rooms.Count < numRooms)
            {
                var numMonsters = random.Next(GlobalConfig.MIN_MONSTERS_PER_ROOM, GlobalConfig.MAX_MONSTERS_PER_ROOM);

                var roomId = unusedRoomIds[0];
                unusedRoomIds.Remove(roomId);

                var room = new Room(floorNum, roomId, numMonsters, numGoblins);
                numGoblins -= room.Monsters.Count(m => m.Name.ToUpperInvariant().Contains("GOBLIN"));

                // Guaranteed connectedness: each room connects to the next
                /*
                if (this.Rooms.Any())
                {
                    this.Rooms[this.Rooms.Count - 1].ConnectTo(room);
                }
                */
                this.Rooms.Add(room);
            }

            /*
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
            */

            // Connect every room to every other room
            foreach (var source in this.Rooms)
            {
                foreach (var target in this.Rooms)
                {
                    if (source != target)
                    {
                        source.ConnectTo(target);
                    }
                }
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

        internal void SpawnItems(params AbstractItem[] items)
        {
            var allMonsters = new List<Monster>();
            this.Rooms.Where(r => !r.IsLocked).ToList().ForEach(r => allMonsters.AddRange(r.Monsters));

            var itemHolders = allMonsters.OrderBy(a => random.Next()).Take(items.Length).ToList();            
            for (var i = 0; i < items.Length; i++)
            {
                itemHolders[i].Item = items[i];
            }
        }

        internal void AddWorkBenchToRandomRoom()
        {
            var room = this.GetRandomEmptyRoom();
            room.CreateWorkBench();
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

        internal void CreateKeyAndLockFinalRoom()
        {
            var roomToLock = this.Rooms.Last();
            roomToLock.IsLocked = true;

            var allMonsters = new List<Monster>();
            this.Rooms.Where(r => r != roomToLock).ToList().ForEach(r => allMonsters.AddRange(r.Monsters.Where(m => m.Item == null)));

            var whichMonster = random.Next(allMonsters.Count);
            var monster = allMonsters[whichMonster];
            monster.Item = new DoorKey();
        }
        
        internal void SealRandomRoom()
        {
            var room = this.GetRandomRoom();
            room.IsSealed = true;
        }

        internal void CreateMachineRoom()
        {
            var room = this.GetRandomEmptyRoom();
            room.ConnectTo(new MachineRoom(room));
        }

        internal Room GetRandomRoom()
        {
            var roomIndex = 0;
            
            // Don't seal the starting/final rooms (with stairs)
            while (roomIndex == 0 || this.Rooms[roomIndex].Stairs != StairsType.NONE || this.Rooms[roomIndex].IsLocked)
            {
                roomIndex = random.Next(this.Rooms.Count - 1);
            }

            return this.Rooms[roomIndex];
        }

        // Room with no bench, machine, or socket
        private Room GetRandomEmptyRoom()
        {
            var room = this.Rooms[0];

            // Not 100% necessary. Socket can coexist, since you just PUT stuff in it.
            while (room.WorkBench != null || room.Socket != null || room.HasMachine() || room.Stairs == StairsType.NEXT_FLOOR)
            {
                var roomIndex = random.Next(this.Rooms.Count - 1);
                room = this.Rooms[roomIndex];
            }

            return room;
        }
    }
}
