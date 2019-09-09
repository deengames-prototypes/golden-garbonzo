using Prototype.Game.Models;
using Prototype.TextToSpeech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Game
{
    class GoldenGarbonzo
    {
        private ISpeaker speaker;

        public void Run()
        {
            this.speaker = new MicrosoftSpeaker();            

            var dungeon = new Dungeon();
            var firstRoom = dungeon.Rooms[0];

            var debug = new List<Monster>();
            dungeon.Rooms.ForEach(r => debug.AddRange(r.Monsters));

            SpeakAndPrint($"Welcome to the dungeon! {firstRoom.GetContents()}", true);
            SpeakAndPrint("Type something and press enter. Type 'help' for help, or 'quit' to quit.", true);

            this.MainProcessingLoop();

            this.speaker.FinishSpeaking();
        }

        private void MainProcessingLoop()
        {
            string input = "";

            while (input.Trim().ToUpper() != "QUIT")
            {
                SpeakAndPrint("Your command? ");
                Console.Write("> ");
                input = Console.ReadLine();
                SpeakAndPrint($"You typed: {input}", true);
            }
        }

        private void SpeakAndPrint(string text, bool isSynchronous = false)
        {
            Console.WriteLine(text);
            this.speaker.Speak(text, isSynchronous);
        }
    }
}
