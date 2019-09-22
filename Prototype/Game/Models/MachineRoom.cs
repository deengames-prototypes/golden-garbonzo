using Prototype.Game.Models.Items.Assemblable;
using System;
using System.Text;

namespace Prototype.Game.Models
{
    class MachineRoom : Room
    {
        public Room ContainingRoom { get; private set;  }
        internal const string HelpText = "You stand in front of a huge machine. Type 'put' to put an item in the alcove. Type 'switch' and then a number to flip a switch. Type 'leave' to leave.";
        private PowerCube powerCube = null;
        private Random random = new Random();
        private bool[] switches = new bool[3];
        private bool[] expectedSwitches = new bool[3];

        public MachineRoom(Room containingRoom) : base(0, "MACHINE", 0)
        {
            this.ContainingRoom = containingRoom;

            for (var i = 0; i < expectedSwitches.Length; i++)
            {
                expectedSwitches[i] = random.Next(100) <= 50;
            }
        }

        protected override string DetailedGetContents()
        {
            var builder = new StringBuilder();
            var cubeText = powerCube == null ? "" : "The power cube thrums from within.";
            builder.Append($"You are standing at a huge, room-sized machine. You notice a cube-shaped alcove near you. {cubeText}");
            builder.Append($"You see three switches that control the energy flow. They are {getSwitches()}. ");
            return builder.ToString();
        }

        protected override string ShortGetContents()
        {
            var builder = new StringBuilder();
            builder.Append($"You are standing at a huge machine. The machine is {(powerCube == null ? "off" : "on")}. ");
            builder.Append($"You see three switches: {getSwitches()}. ");
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
