using Prototype.Game.Models;
using Prototype.TextToSpeech;
using System;
using System.Collections.Generic;

namespace Prototype.Game
{
    class GoldenGarbonzo
    {
        private ISpeaker speaker;
        private Room currentRoom;

        public void Run()
        {
            this.speaker = new MicrosoftSpeaker();            

            var dungeon = new Dungeon();
            this.currentRoom = dungeon.Rooms[0];

            SpeakAndPrint($"Welcome to the dungeon! {this.currentRoom.GetContents()}");
            SpeakAndPrint("Type something and press enter. Type 'help' for help, or 'quit' to quit.");

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
                this.speaker.StopAndClearQueue();
                SpeakAndPrint($"You typed: {input}");
                this.ProcessInput(input);
            }

            this.SpeakAndPrint("Bye!");
        }

        private void ProcessInput(string input)
        {
            switch (input.ToUpperInvariant())
            {
                case "HELP":
                    SpeakAndPrint("Type 'quit' to quit, 'list' or 'l' to list the current room");
                    break;
                case "LIST":
                case "L":
                    SpeakAndPrint(this.currentRoom.GetContents());
                    break;
                case "QUIT":
                    return;
                default:
                    SpeakAndPrint($"Not sure how to {input}");
                    break;
            }
        }

        private void SpeakAndPrint(string text)
        {
            Console.WriteLine(text);
            this.speaker.Speak(text);
        }
    }
}
