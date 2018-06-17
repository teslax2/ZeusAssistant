using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;
//using System.Speech.Synthesis;
using System.Globalization;
using System.Threading;

namespace ZeusAssistant.Model
{
    class SpeechMicrosoft
    {
        public event EventHandler<string> Recognized;
        //private SpeechSynthesizer _synthesizer;
        private SpeechRecognitionEngine _recognizer;
        private CultureInfo _cultureInfo;
        private Choices _choices;
        private GrammarBuilder _grammarBuilder;
        private Grammar _grammar;
        private bool _stop;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public SpeechMicrosoft()
        {
            _cultureInfo = new CultureInfo("pl-PL");
            Init();
        }

        public SpeechMicrosoft(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
            Init();
        }

        private void Init()
        {
            try
            {
                _recognizer = new SpeechRecognitionEngine(_cultureInfo);
                _recognizer.SetInputToDefaultAudioDevice();
                _choices = new Choices();
                _choices.Add("dupa");
                _choices.Add("pimpuś");
                _grammarBuilder = new GrammarBuilder();
                _grammarBuilder.Append(_choices);
                _grammarBuilder.Culture = _cultureInfo;
                _grammar = new Grammar(_grammarBuilder);
                _recognizer.LoadGrammar(_grammar);
                _recognizer.SpeechRecognized += _recognizer_SpeechRecognized;

                //_synthesizer = new SpeechSynthesizer();
                //_synthesizer.SelectVoiceByHints(VoiceGender.Female);
                //_synthesizer.SetOutputToDefaultAudioDevice();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Microsoft speech engine error");
            }
        }

        public void OnSpeechRecognized(string message)
        {
            EventHandler<string> recognized = Recognized;
            if (recognized != null)
                recognized(this, message);
        }

        private void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Stop();
            logger.Debug("Recognized! {0}: {1}", e.Result.Text, e.Result.Confidence);
            TunePlayer.PlaySound();
            OnSpeechRecognized(e.Result.ToString());
        }

        public async Task Run()
        {
            if (_recognizer == null) return;
            _stop = false;
            await Task.Run(() =>
            {
                logger.Debug("Started!");
                while (!_stop)
                {
                    _recognizer.Recognize();
                }
            });
        }

        public void Stop()
        {
            _stop = true;
        }

        public void Speak(string content)
        {
            //_synthesizer.SpeakAsync(content);
        }
    }
}
