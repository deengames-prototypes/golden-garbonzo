using Prototype.Game.Models.Items.Assemblable;
using System;
using System.Text;

namespace Prototype.Game.Models
{
    class MachineRoom : Room
    {
        public Room ContainingRoom { get; private set;  }

        internal bool InsertedPowerCube { get; set; } = false;
        internal const string HelpText = "You stand in front of a huge machine. Type 'put' to put an item in the alcove. Type 'switch' and then a number to flip a switch. Type 'leave' to leave.";

        private PowerCube powerCube = null;
        private Random random = new Random();
        private bool[] switches = new bool[3];
        private bool[] expectedSwitches = new bool[3];

        public MachineRoom(Room containingRoom) : base(0, "MACHINE", 0)
        {
            this.ContainingRoom = containingRoom;

            // Expected configuration is NOT RANDOM. 25% chance it's true/true/true, 25% chance for each node to be off.
            // Having 2+ off nodes is too easy to solve.
            var whichConfiguration = random.Next(100);

            if (whichConfiguration >= 75)
            {
                for (var i = 0; i < expectedSwitches.Length; i++)
                {
                    expectedSwitches[i] = true;
                }
            }
            else
            {
                int whichOneIsOff = (int)Math.Floor(whichConfiguration / 25d);
                expectedSwitches[whichOneIsOff] = false;
            }
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

        protected override string DetailedGetContents()
        {
            var builder = new StringBuilder();
            var cubeText = powerCube == null ? "" : "The power cube thrums from within.";
            builder.Append($"You are standing at a huge, room-sized machine. You notice a cube-shaped alcove near you. {cubeText}");
            builder.Append($"You see three switches above a clear energy chamber with three energy nodes. The energy {this.GetEnergyPoint()}. The switches are {getSwitches()}. ");
            return builder.ToString();
        }

        protected override string ShortGetContents()
        {
            var builder = new StringBuilder();
            builder.Append($"You are standing at a huge machine. The machine is {(powerCube == null ? "off" : "on")}. ");
            builder.Append($"Energy in the 3-node glass energy chamber {this.GetEnergyPoint()}. You see three switches: {getSwitches()}. ");
            return builder.ToString();
        }

        private string GetEnergyPoint()
        {
            if (this.switches[0] != this.expectedSwitches[0])
            {
                return "stops before entering the chamber";
            }
            else if (this.switches[1] != this.expectedSwitches[1])
            {
                return "fills the first  energy node";
            }
            else if (this.switches[2] != this.expectedSwitches[2])
            {
                return "fills the second energy node";
            }
            else
            {
                return "floods the chamber and disappears into the depths of the machine";
            }
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
