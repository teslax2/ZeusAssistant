using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace ZeusAssistant.Model
{
    public class TextToSpeech
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Speak(string sentence)
        {
            if (string.IsNullOrEmpty(sentence)) return;
            try
            {
                var tts = new SpeechSynthesizer();
                tts.SetOutputToDefaultAudioDevice();
                var voices = tts.GetInstalledVoices(new System.Globalization.CultureInfo("pl-PL"));
                if (voices.Count > 0)
                {
                    tts.SelectVoice(voices[0].VoiceInfo.Name);
                }
                Task t = Task.Run(()=> { tts.Speak(sentence); });
                t.Wait();    
            }
            catch (Exception ex)
            {

                logger.Error(ex, "Failed to run text to speach service");
            }
        }
    }
}
