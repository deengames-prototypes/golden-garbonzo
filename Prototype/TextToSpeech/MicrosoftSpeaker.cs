using System;
using System.Collections.Generic;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Threading;

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
        private List<string> queueToSpeak = new List<string>();
        private Thread queueThread;
        private bool IsActive = true;

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

        public MicrosoftSpeaker()
        {
            // Initialize a new instance of the SpeechSynthesizer.  
            this.synthesizer = new SpeechSynthesizer();
            // Configure the audio output. 
            this.synthesizer.SetOutputToDefaultAudioDevice();
            this.synthesizer.SelectVoiceByHints(VoiceGender.Female);

            this.queueThread = new Thread(() =>
             {
                 while (this.IsActive)
                 {
                     this.OnUpdate();
                 }
             });

            this.queueThread.Start();
        }

        public void Speak(string text)
        {
            if (this.lastSpoken != null && !this.lastSpoken.IsCompleted)
            {
                queueToSpeak.Add(text);
            }
            else
            {
                this.lastSpoken = synthesizer.SpeakAsync(text);
            }
        }

        public void FinishSpeaking()
        {
            this.IsActive = false;

            while (this.lastSpoken != null && !this.lastSpoken.IsCompleted && this.queueToSpeak.Count > 0)
            {
                System.Threading.Thread.Sleep(100);
            }

            queueThread.Join();
        }

        public void StopAndClearQueue()
        {
            synthesizer.SpeakAsyncCancelAll();
            this.lastSpoken = null;
            this.queueToSpeak.Clear();
        }

        private void OnUpdate()
        {
            // Concurrent access to this.lastSpoken; sometimes this.lastSpoken is not null but immediately this.lastSpoken.IsCompleted
            // throws a null exception. Wierd.

            try
            {
                if (this.lastSpoken != null && this.lastSpoken.IsCompleted && this.queueToSpeak.Count > 0)
                {
                    var text = this.queueToSpeak[0];
                    this.queueToSpeak.RemoveAt(0);
                    this.lastSpoken = synthesizer.SpeakAsync(text);
                }
            }
            finally
            {

            }
        }
    }
}
