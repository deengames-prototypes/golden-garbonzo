using Prototype.Game.Models.Items;
using Prototype.Game.Models.Items.Assemblable;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Game.Models
{
    class Player : Monster
    {
        public List<AbstractItem> Inventory = new List<AbstractItem>();

        public Player() : base("Player", 50, 7, 3)
        { }

        internal void RemoveItem(string itemName)
        {
            itemName = itemName.ToUpperInvariant();
            var item = this.Inventory.FirstOrDefault(i => i.Name.ToUpperInvariant() == itemName);
            if (item != null)
            {
                this.Inventory.Remove(item);
            }
        }
    }
}
