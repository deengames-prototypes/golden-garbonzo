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
            string input = "";

            SpeakAndPrint("Hello. Type something and press enter, and I will say it. Type QUIT at any time to quit");

            while (input.Trim().ToUpper() != "QUIT")
            {
                input = Console.ReadLine();
                SpeakAndPrint(input);
                Console.Write("> ");
            }

            SpeakAndPrint("BYE!");
            System.Threading.Thread.Sleep(1500); // needed for last audio to play
        }

        private void SpeakAndPrint(string text)
        {
            Console.WriteLine(text);
            this.speaker.Speak(text);
        }
    }
}
