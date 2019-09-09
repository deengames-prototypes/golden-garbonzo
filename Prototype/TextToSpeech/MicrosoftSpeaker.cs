using System;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;

namespace Prototype.TextToSpeech
{
    /// <summary>
    /// Text-to-speech via the proprietary System.Speech libararies (works on Windows only)
    /// TODO: extract this to a separate reference DLL so we can compile cross-platform stuff
    /// </summary>
    public class MicrosoftSpeaker : ISpeaker
    {
        private SpeechSynthesizer synthesizer;
        private Prompt lastSpoken;

        public MicrosoftSpeaker()
        {
            // Initialize a new instance of the SpeechSynthesizer.  
            this.synthesizer = new SpeechSynthesizer();
            // Configure the audio output. 
            this.synthesizer.SetOutputToDefaultAudioDevice();
            this.synthesizer.SelectVoiceByHints(VoiceGender.Female);
        }

        public void Speak(string text, bool isSynchronous = false)
        {
            if (this.lastSpoken != null && !this.lastSpoken.IsCompleted)
            {
                synthesizer.SpeakAsyncCancel(this.lastSpoken);
            }

            if (isSynchronous)
            {
                synthesizer.Speak(text);
            }
            else
            {
                this.lastSpoken = synthesizer.SpeakAsync(text);
            }
        }


        static void PrintAllVoices()
        {
            // Initialize a new instance of the SpeechSynthesizer.  
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {

                // Output information about all of the installed voices.   
                Console.WriteLine("Installed voices -");
                foreach (InstalledVoice voice in synth.GetInstalledVoices())
                {
                    VoiceInfo info = voice.VoiceInfo;
                    string AudioFormats = "";
                    foreach (SpeechAudioFormatInfo fmt in info.SupportedAudioFormats)
                    {
                        AudioFormats += String.Format("{0}\n",
                            fmt.EncodingFormat.ToString());
                    }

                    Console.WriteLine(" Name:          " + info.Name);
                    Console.WriteLine(" Culture:       " + info.Culture);
                    Console.WriteLine(" Age:           " + info.Age);
                    Console.WriteLine(" Gender:        " + info.Gender);
                    Console.WriteLine(" Description:   " + info.Description);
                    Console.WriteLine(" ID:            " + info.Id);
                    Console.WriteLine(" Enabled:       " + voice.Enabled);
                    if (info.SupportedAudioFormats.Count != 0)
                    {
                        Console.WriteLine(" Audio formats: " + AudioFormats);
                    }
                    else
                    {
                        Console.WriteLine(" No supported audio formats found");
                    }

                    string AdditionalInfo = "";
                    foreach (string key in info.AdditionalInfo.Keys)
                    {
                        AdditionalInfo += String.Format("  {0}: {1}\n", key, info.AdditionalInfo[key]);
                    }

                    Console.WriteLine(" Additional Info - " + AdditionalInfo);
                    Console.WriteLine();
                }
            }
        }

        public void FinishSpeaking()
        {
            while (this.lastSpoken != null && !this.lastSpoken.IsCompleted)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
