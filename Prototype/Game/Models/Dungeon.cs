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
            var numRooms = random.Next(GlobalConfig.MIN_ROOMS_PER_FLOOR, GlobalConfig.MAX_ROOMS_PER_FLOOR); // 5-8 rooms
            while (this.Rooms.Count < numRooms)
            {
                // 40% chance of an empty room
                var numMonsters = random.NextDouble() <= GlobalConfig.PROBABILITY_OF_NO_MONSTERS ?
                    0 : random.Next(GlobalConfig.MIN_MONSTERS, GlobalConfig.MAX_MONSTERS);

                var room = new Room(numMonsters);
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
        }
    }
}
