using System;
using System.Collections.Generic;
using System.Threading;

namespace Prototype.TextToSpeech
{
    public class DummySpeaker: ISpeaker
    {
        public void Speak(string text)
        {
        }

        public void FinishSpeaking()
        {
        }

        public void StopAndClearQueue()
        {
        }
    }
}
