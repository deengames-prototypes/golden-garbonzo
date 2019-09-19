using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Game.Models.Items
{
    class Gemstone : AbstractItem
    {
        private static List<string> UniqueDescriptions = new List<string>() { "shiny", "smooth", "ridged", "oblong", "round", "angular", "heavy", "bumpy", "cold", "warm", "metallic" };
        private static int nextId = 0;

        private readonly string id = "";

        static Gemstone()
        {
            var random = new Random();
            Gemstone.UniqueDescriptions = Gemstone.UniqueDescriptions.OrderBy(g => random.Next()).ToList();
        }

        public Gemstone()
        {
            this.id = Gemstone.UniqueDescriptions[Gemstone.nextId];
            Gemstone.nextId++;
        }

        public override string Description => $"A {this.id} gemstone";
    }
}
