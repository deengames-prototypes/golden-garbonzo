using Prototype.Game.Models.Items;
using Prototype.Game.Models.Items.Assemblable.Parts;
using System;
using System.Collections.Generic;

namespace Prototype.Game.Models
{
    class Dungeon
    {
        internal List<Floor> Floors = new List<Floor>();
        private Random random = new Random();

        public Dungeon()
        {
            var numFloors = GlobalConfig.NUM_FLOORS;
            for (var i = 1; i <= numFloors; i++)
            {
                this.Floors.Add(new Floor(i));
            }

            // Stuff for every floor
            this.Floors.ForEach(f => f.SealRandomRoom());
            this.Floors.ForEach(f => f.AddWorkBenchToRandomRoom());
            
            var b1 = this.Floors[0];
            var b2 = this.Floors[1];
            var b3 = this.Floors[2];

            // Demo requirements. Not as complex as I hoped.
            // 1) Lock a room on 1F with a gem inside
            var gemRoom = b1.GetRandomRoom();
            gemRoom.AddToRandomMonster(new Gemstone());
            gemRoom.IsLocked = true;

            // 2) Key on 2F
            b2.GetRandomRoom().AddToRandomMonster(new DoorKey());

            // 3) Machine puzzle on 3F
            b3.CreateMachineRoom();

            // 4) Socket on 3F
            b3.GetRandomRoom().CreateGemSocket();

            // 5) Add constituent elements per floor
            b1.SpawnItems(new GlassBox(), new GlassLid());
            b2.SpawnItems(new PositronicLaser(), new FocusChamber());
            b3.SpawnItems(new CoilChasis(), new NeutronCell());
        }
    }
}
