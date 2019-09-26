using Prototype.Game.Models.Items;
using Prototype.Game.Models.Items.Assemblable;
using System;
using System.Text;

namespace Prototype.Game.Models
{
    class MachineRoom : Room
    {
        internal const string HelpText = "You stand in front of a huge machine. Type 'put' to put an item in the alcove. Type 'switch' and then a number to flip a switch. Type 'leave' to leave.";

        public Room ContainingRoom { get; private set;  }

        internal bool InsertedPowerCube { get; set; } = false;
        internal Gemstone Gem { get; set; } = new Gemstone();

        private Random random = new Random();
        private bool[] switches = new bool[3];
        private bool[] expectedSwitches = new bool[3] { true, true, true };

        public MachineRoom(Room containingRoom) : base(0, "MACHINE", 0, 0)
        {
            this.ContainingRoom = containingRoom;

            // Expected configuration is NOT RANDOM. One node is off.
            // Having 2+ off nodes is too easy to solve.
            var whichConfiguration = random.Next(75);
            int whichOneIsOff = (int)Math.Floor(whichConfiguration / 25d);
            expectedSwitches[whichOneIsOff] = false;
        }

        internal bool IsSolved()
        {
            for (var i = 0; i < expectedSwitches.Length; i++)
            {
                if (expectedSwitches[i] != switches[i])
                {
                    return false;
                }
            }

            return true;
        }

        internal void Switch(int index)
        {
            this.switches[index] = !this.switches[index];
        }

        internal string GetEnergyPoint()
        {
            if (this.switches[0] != this.expectedSwitches[0])
            {
                return "stops before entering the chamber.";
            }
            else if (this.switches[1] != this.expectedSwitches[1])
            {
                return "fills the first  energy node.";
            }
            else if (this.switches[2] != this.expectedSwitches[2])
            {
                return "fills the second energy node.";
            }
            else
            {
                return "floods the chamber and disappears into the depths of the machine.";
            }
        }

        protected override string DetailedGetContents()
        {
            var builder = new StringBuilder();

            var cubeText = InsertedPowerCube ? "The power cube thrums from within." : "";
            var energyMessage = this.InsertedPowerCube ? $"The energy { this.GetEnergyPoint()}" : "The energy chamber appears empty.";

            builder.Append($"You are standing at a huge, room-sized machine. You notice a cube-shaped alcove near you. {cubeText}");
            builder.Append($"You see three switches above a clear energy chamber with three energy nodes. {energyMessage} The switches are {getSwitches()}. ");
            return builder.ToString();
        }

        protected override string ShortGetContents()
        {
            var builder = new StringBuilder();
            builder.Append($"You are standing at a huge machine. The machine is {(InsertedPowerCube ? "on" : "off")}. ");
            builder.Append($"Energy in the 3-node glass energy chamber {(InsertedPowerCube ? this.GetEnergyPoint() : "is empty")}. You see three switches: {getSwitches()}. ");
            return builder.ToString();
        }

        private string getSwitches()
        {
            return $"{getSwitch(0)}, {getSwitch(1)}, and {getSwitch(2)}";
        }

        private string getSwitch(int index)
        {
            return this.switches[index] == false ? "off" : "on";
        }
    }
}
