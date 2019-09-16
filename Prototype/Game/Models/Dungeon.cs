using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Game.Models
{
    class Dungeon
    {
        internal List<Floor> Floors = new List<Floor>();

        public Dungeon()
        {
            var numFloors = GlobalConfig.NUM_FLOORS;
            for (var i = 0; i < numFloors; i++)
            {
                this.Floors.Add(new Floor(i + 1));
            }
        }
    }
}
