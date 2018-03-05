﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;
using System.Globalization;
using System.Threading;

namespace ZeusAssistant.Model
{
    class SpeechMicrosoft
    {
        public event EventHandler<string> Recognized;
        private SpeechRecognitionEngine _recognizer;
        private CultureInfo _cultureInfo;
        private Choices _choices;
        private GrammarBuilder _grammarBuilder;
        private Grammar _grammar;
        private bool _stop;

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
            System.Diagnostics.Debug.WriteLine("Recognized! {0}: {1}",e.Result.Text, e.Result.Confidence);
            JinglePlayer.PlaySound();
            OnSpeechRecognized(e.Result.ToString());
        }

        public async Task Run()
        {
            _stop = false;
            await Task.Run(() =>
            {
                System.Diagnostics.Debug.WriteLine("Started!");
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
    }
}