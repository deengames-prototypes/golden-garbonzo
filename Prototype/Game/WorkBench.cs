using System.Linq;
using Prototype.Game.Models;
using Prototype.Game.Models.Items;
using Prototype.Game.Models.Items.Assemblable;

namespace Prototype.Game
{
    class WorkBench
    {
        /// <summary>
        /// Assembles an item out of constituent elements. Returns true if assembled, false if not.
        /// </summary>
        /// <param name="itemName">The thing to assemble.</param>
        public bool Assemble(AbstractItem item, Player player)
        {
            if (this.CheckForAndRemove(player, this.GetPartsFor(item)))
            {
                player.Inventory.Add(item);
                return true;
            }

            return false;
        }

        public string[] GetPartsFor(AbstractItem item)
        {
            if (item is PowerCube)
            {
                return new string[] { "Glass Cube", "Positron Emitter", "Antimatter Coil" };
            }
            else if (item is GlassCube)
            {
                return new string[] { "Glass Box", "Glass Lid" };
            }
            else if (item is PositronEmitter)
            {
                return new string[] { "Positronic Laser", "Focus Chamber" };
            }
            else if (item is AntimatterCoil)
            {
                return new string[] { "Coil Chasis", "Neutron Cell" };
            }

            return new string[0];
        }

        /// <summary>
        /// Check if the player has some set of items. If he has all of them, remove all of them.
        /// </summary>
        /// <param name="player">The player whose inventory we're frisking</param>
        /// <param name="items">A list of item names to search for</param>
        /// <returns>true if the player had all of them (we remove them), false otherwise.</returns>
        private bool CheckForAndRemove(Player player, params string[] items)
        {
            var inventoryItems = player.Inventory.Select(i => i.Name.ToUpperInvariant());

            foreach (var itemName in items)
            {
                if (!inventoryItems.Contains(itemName.ToUpperInvariant())
                {
                    return false;
                }
            }

            foreach (var itemName in items)
            {
                var item = player.Inventory.Single(i => i.Name.ToUpperInvariant().Contains(itemName.ToUpperInvariant()));
                player.Inventory.Remove(item);
            }

            return true;
        }
    }
}
