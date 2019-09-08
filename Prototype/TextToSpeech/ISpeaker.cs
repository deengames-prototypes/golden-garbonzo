namespace Prototype.TextToSpeech
{
    interface ISpeaker
    {
        void Speak(string text);
        void FinishSpeaking();
    }
}
