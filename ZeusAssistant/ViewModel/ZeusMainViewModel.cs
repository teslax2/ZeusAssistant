using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeusAssistant.Model;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using NLog;

namespace ZeusAssistant.ViewModel
{
    class ZeusMainViewModel:INotifyPropertyChanged
    {
        public SpeechMicrosoft MicrosoftSpeech;
        public HttpClient HttpClient;
        public SpeechWitAi WitAi;
        public VoiceRecorder Recorder;
        public Creditentials Credits;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region buttons
        public CommandBinding StartRecording { get { return new CommandBinding(async () => await RunSpeechRecognition(), () => true); }}
        //public CommandBinding StopRecording { get { return new CommandBinding(() => , () => true); } }
        #endregion

        #region events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        public ZeusMainViewModel()
        {
            Credits = new Creditentials();
            Credits.Load();
            HttpClient = new HttpClient();
            MicrosoftSpeech = new SpeechMicrosoft();
            MicrosoftSpeech.Recognized += _microsoftSpeech_Recognized;
            WitAi = new SpeechWitAi(HttpClient,Credits.WitAiPath,Credits.WitAiToken);
            Recorder = new VoiceRecorder(0.05f, 1000);
            Recorder.DataAvailable += Recorder_DataAvailable;
            Recorder.RecordingStopped += Recorder_RecordingStopped;
        }

        private void Recorder_RecordingStopped(object sender, EventArgs e)
        {
            var result = WitAi.StopPostChunked();
            if (result != null)
                logger.Info(result.MessageIntent.ToString());
         }

        private void _microsoftSpeech_Recognized(object sender, string e)
        {
            WitAi.StartPostChunked();
            Recorder.Start();
        }

        private void Recorder_DataAvailable(object sender, VoiceRecorderEventArgs e)
        {
            WitAi.SendPostChunked(e.Data);
        }

        #region functions
        public async Task RunSpeechRecognition()
        {
            await MicrosoftSpeech.Run();
        }

        #endregion
    }
}
