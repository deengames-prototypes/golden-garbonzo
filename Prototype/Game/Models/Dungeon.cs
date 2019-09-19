﻿using Prototype.Game.Models.Items;
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
            for (var i = 1; i <= numFloors; i++)
            {
                this.Floors.Add(new Floor(i));
            }

            // TODO: this should probably be more random. For now, it's a progression of difficulty, amirite?
            this.Floors.ForEach(f => f.SealRandomRoom());
            this.Floors[0].CreateKeyAndLockFinalRoom();
        }
    }
}
