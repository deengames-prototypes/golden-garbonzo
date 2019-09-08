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

            SpeakAndPrint("Hello. Type something and press enter, and I will say it.");
            SpeakAndPrint("What should I say? Type QUIT to quit.");

            do
            {
                input = Console.ReadLine();
                SpeakAndPrint(input);
                SpeakAndPrint("Now? (QUIT to quit.)");
                Console.Write("> ");
            }
            while (input.Trim().ToUpper() != "QUIT");

            SpeakAndPrint("BYE!");
        }

        private void SpeakAndPrint(string text)
        {
            Console.WriteLine(text);
            this.speaker.Speak(text);
        }
    }
}
