﻿using Prototype.Game.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Game.Models
{
    class Dungeon
    {
        internal List<Room> Rooms = new List<Room>();
        private static Random random = new Random();

        public Dungeon()
        {
            this.GenerateAndConnectRooms();
            this.RandomlyConnectRooms();
            this.CreateKey();
        }

        private void GenerateAndConnectRooms()
        {
            var numRooms = random.Next(GlobalConfig.MIN_ROOMS_PER_FLOOR, GlobalConfig.MAX_ROOMS_PER_FLOOR); // 5-8 rooms
            while (this.Rooms.Count < numRooms)
            {
                // 40% chance of an empty room
                var numMonsters = random.NextDouble() <= GlobalConfig.PROBABILITY_OF_NO_MONSTERS ?
                    0 : random.Next(GlobalConfig.MIN_MONSTERS_PER_FLOOR, GlobalConfig.MAX_MONSTERS_PER_FLOOR);

                var room = new Room(numMonsters);
                // Guaranteed connectedness: each room connects to the next
                if (this.Rooms.Any())
                {
                    this.Rooms[this.Rooms.Count - 1].ConnectTo(room);
                }

                this.Rooms.Add(room);
            }
        }

        private void RandomlyConnectRooms()
        {
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
        }

        private void CreateKey()
        {
            var allMonsters = new List<Monster>();
            this.Rooms.ForEach(r => allMonsters.AddRange(r.Monsters));

            var whichMonster = random.Next(allMonsters.Count);
            var monster = allMonsters[whichMonster];
            monster.Item = new DoorKey();
        }
    }
}
