using Prototype.Game.Models.Items;
using System.Collections.Generic;

namespace Prototype.Game.Models
{
    class Player : Monster
    {
        public List<AbstractItem> Inventory = new List<AbstractItem>();

        public Player() : base("Player", 50, 7, 3)
        { }
    }
}
