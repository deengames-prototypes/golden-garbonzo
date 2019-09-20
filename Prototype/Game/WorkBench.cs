using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prototype.Game.Models;
using Prototype.Game.Models.Items.Assemblable;

namespace Prototype.Game
{
    class WorkBench
    {
        /// <summary>
        /// Assembles an item out of constituent elements. Returns true if assembled, false if not.
        /// </summary>
        /// <param name="itemName">The thing to assemble.</param>
        public bool Assemble(string itemName, Player player)
        {
            itemName = itemName.ToUpperInvariant();

            if (itemName.Contains("POWER")) // power cube
            {
                return this.CheckForAndRemove(player, "Glass Cube", "Positron Emitter", "Antimatter Coil");
            }
            else if (itemName.Contains("GLASS CUBE"))
            {
                return this.CheckForAndRemove(player, "Glass Box", "Glass Lid");
            }
            else if (itemName.Contains("POSITRON")) // positron emitter
            {
                return this.CheckForAndRemove(player, "Positronic Laser", "Focus Chamber");
            }
            else if (itemName.Contains("ANTIMATTER")) // antimatter coil
            {
                return this.CheckForAndRemove(player, "Coil Chasis", "Neutron Cell");
            }

            return false; // dunno how to assemble that
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
