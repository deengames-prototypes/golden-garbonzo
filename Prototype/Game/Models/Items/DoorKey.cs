using System;

namespace Prototype.Game.Models.Items
{
    // TODO: pairs to a specific door
    class DoorKey : AbstractItem
    {
        public override string Description
        {
            get
            {
                return "A key that opens a locked door.";
            }
        }
    }
}
