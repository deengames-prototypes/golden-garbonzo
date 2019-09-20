using System;
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
        public void Assemble(Type type, Player player)
        {
            var requiredParts = this.GetPartsFor(type);
            if (this.CanAssemble(type, player))
            {
                foreach (var itemName in requiredParts)
                {
                    var subPart = player.Inventory.Single(i => i.Name.ToUpperInvariant().Contains(itemName.ToUpperInvariant()));
                    player.Inventory.Remove(subPart);
                }
            }
        }

        /// <summary>
        /// Check if the player has some set of items. If he has all of them, remove all of them.
        /// </summary>
        /// <param name="player">The player whose inventory we're frisking</param>
        /// <param name="items">A list of item names to search for</param>
        /// <returns>true if the player had all of them (we remove them), false otherwise.</returns>
        public bool CanAssemble(Type type, Player player)
        {
            var inventoryItems = player.Inventory.Select(i => i.Name.ToUpperInvariant());
            var requiredItems = this.GetPartsFor(type).Select(s => s.Replace(" ", "").ToUpperInvariant()); // remove spaces for parity with type names

            foreach (var itemName in requiredItems)
            {
                if (!inventoryItems.Contains(itemName.ToUpperInvariant()))
                {
                    return false;
                }
            }

            return true;
        }

        internal string[] GetPartsFor(Type type)
        {
            string[] toReturn = new string[0];

            if (type == typeof(PowerCube))
            {
                toReturn = new string[] { "Glass Cube", "Positron Emitter", "Antimatter Coil" };
            }
            else if (type == typeof(GlassCube))
            {
                toReturn = new string[] { "Glass Box", "Glass Lid" };
            }
            else if (type == typeof(PositronEmitter))
            {
                toReturn = new string[] { "Positronic Laser", "Focus Chamber" };
            }
            else if (type == typeof(AntimatterCoil))
            {
                toReturn = new string[] { "Coil Chasis", "Neutron Cell" };
            }

            return toReturn.Select(s => s.Replace(" ", "").ToUpperInvariant()).ToArray(); // parity with type-as-name
        }
    }
}
