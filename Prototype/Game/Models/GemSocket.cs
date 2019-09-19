using Prototype.Game.Models.Items;
using System.Collections.Generic;

namespace Prototype.Game.Models
{
    class GemSocket
    {
        // TODO: maybe I don't want just *any* ol' gems. For now, I'll take it.
        private List<Gemstone> socketedGems = new List<Gemstone>();
        public readonly int RequiredGems;

        public GemSocket(int requiredGems)
        {
            this.RequiredGems = requiredGems;
        }

        /// <summary>
        /// Socket a gem in this socket. Returns true if socketed, false if the gem was rejected.
        /// </summary>
        /// <param name="stone"></param>
        /// <returns></returns>
        public bool Socket(Gemstone stone)
        {
            if (socketedGems.Contains(stone) || socketedGems.Count >= RequiredGems)
            {
                return false;
            }

            this.socketedGems.Add(stone);
            return true;
        }

        public bool IsSolved()
        {
            return this.socketedGems.Count >= this.RequiredGems;
        }
    }
}
