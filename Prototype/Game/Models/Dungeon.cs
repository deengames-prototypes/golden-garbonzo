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
            var numRooms = random.Next(5, 9); // 5-8 rooms
            while (this.Rooms.Count < numRooms)
            {
                var room = new Room();
                // Guaranteed connectedness
                if (this.Rooms.Any())
                {
                    this.Rooms[this.Rooms.Count - 1].ConnectTo(room);
                }

                this.Rooms.Add(room);
            }

            for (var i = 0; i < this.Rooms.Count; i++)
            {
                var room = this.Rooms[i];
                var target = room;

                while (target == room)
                {
                    target = this.Rooms[random.Next(this.Rooms.Count)];
                }

                room.ConnectTo(target);
            }
        }
    }
}
