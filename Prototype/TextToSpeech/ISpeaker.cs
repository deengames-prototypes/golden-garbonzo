namespace Prototype.TextToSpeech
{
    interface ISpeaker
    {
        void Speak(string text, bool isSynchronous);
        void FinishSpeaking();
    }
}
