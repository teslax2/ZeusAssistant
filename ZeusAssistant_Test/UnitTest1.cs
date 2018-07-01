using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZeusAssistant.Model;
using System.Net.Http;
using System.Threading.Tasks;

namespace ZeusAssistantTest
{
    [TestClass]
    public class ZeusAssitantUnitTest
    {
        [TestMethod]
        public async Task TranslateAsync_WithValidData_Translates()
        {
            var httpClient = new HttpClient();
            var translator = new GoogleTranslator(httpClient);

            var returnedString = await translator.TranslateAsync("ja", "pl", "en");
            Console.WriteLine(returnedString);
            StringAssert.Contains(returnedString, "I");
        }

        [TestMethod]
        public async Task TranslateAsync_NoHttpClient_ShouldTranslate()
        {
            HttpClient httpClient = null;
            var translator = new GoogleTranslator(httpClient);

            var returnedString = await translator.TranslateAsync("ja", "pl", "en");
            Console.WriteLine(returnedString);
            StringAssert.Contains(returnedString, "I");
        }

        [TestMethod]
        public async Task TranslateAsync_WithNoValidPhrase_ShouldTranslate()
        {
            HttpClient httpClient = new HttpClient();
            var translator = new GoogleTranslator(httpClient);

            var returnedString = await translator.TranslateAsync("", "pl", "en");
            Console.WriteLine(returnedString);
            StringAssert.Contains(returnedString, "");
        }
        [TestMethod]
        public void TextToSpeechSpeach_Speak()
        {
            TextToSpeech.Speak("Maniek wstawaj szkoda dnia!");
        }

        [TestMethod]
        public void TextToSpeechSpeach_NoArguments_Speak()
        {
            TextToSpeech.Speak("");
        }

        [TestMethod]
        public void TunePlayer_PlaySound_ShouldPlay()
        {
            TunePlayer.PlaySound();
        }

        [TestMethod]
        public async Task TunePlayer_PlaySoundAsync_ShouldPlay()
        {
            await TunePlayer.PlaySoundAsync();
        }
    }
}
