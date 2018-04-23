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
using ZeusAssistant.Model.Weather;

namespace ZeusAssistant.ViewModel
{
    class ZeusMainViewModel:INotifyPropertyChanged
    {
        //private SpeechMicrosoft _microsoftSpeech;
        private SpeechWitAi _witAi;
        private VoiceRecorder _recorder;
        private Creditentials _credits;
        private WeatherApi _weatherApi;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public HttpClient HttpClient { get; set; }

        #region buttons
        public CommandBinding StartRecording { get { return new CommandBinding(async () => await RunSpeechRecognition(), () => true); }}
        public CommandBinding StopRecording { get { return new CommandBinding(async () => await DoTest(), () => true); } }
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
            try
            {
                _credits = new Creditentials();
                _credits.Load();
                HttpClient = new HttpClient();
                //_microsoftSpeech = new SpeechMicrosoft();
                //_microsoftSpeech.Recognized += _microsoftSpeech_Recognized;
                _witAi = new SpeechWitAi(HttpClient,
                    _credits.Credits.Where((x) => x.Provider == ApiProvider.WitAi).First().Path,
                    _credits.Credits.Where((x) => x.Provider == ApiProvider.WitAi).First().Token);
                _recorder = new VoiceRecorder(0.05f, 1000);
                _recorder.DataAvailable += Recorder_DataAvailable;
                _recorder.RecordingStopped += async(s,e) => await Recorder_RecordingStopped(s,e);
                _weatherApi = new WeatherApi(HttpClient,
                    _credits.Credits.Where((x) => x.Provider == ApiProvider.OpenWeatherMap).First().Path,
                    _credits.Credits.Where((x) => x.Provider == ApiProvider.OpenWeatherMap).First().Token);
            }
            catch (Exception ex)
            {

                logger.Error(ex, "Failed to create ZeusMainViewModel");
            }
        }

        private async Task Recorder_RecordingStopped(object sender, EventArgs e)
        {
            var result = _witAi.StopPostChunked();
            if (result == null)
                return;
            logger.Info(result.MessageIntent.ToString());
            await DoActions(result);
         }

        private void _microsoftSpeech_Recognized(object sender, string e)
        {
            _witAi.StartPostChunked();
            _recorder.Start();
        }

        private void Recorder_DataAvailable(object sender, VoiceRecorderEventArgs e)
        {
            _witAi.SendPostChunked(e.Data);
        }

        #region functions
        public async Task RunSpeechRecognition()
        {
            //await _microsoftSpeech.Run();
            await Task.Delay(100);
            _microsoftSpeech_Recognized(this, "");
        }

        private async Task<string> DoActions(Model.Messages.Message action)
        {
            switch (action.MessageIntent)
            {
                case Model.Messages.IntentEnum.Weather:
                    var weatherAction = action as Model.Messages.MessageWeather;
                    return await _weatherApi.GetForecastAsync(weatherAction.Location, weatherAction.When);
                case Model.Messages.IntentEnum.Time:
                    return "";
                case Model.Messages.IntentEnum.Alarm:
                    return "";
                case Model.Messages.IntentEnum.Note:
                    return "";
                default:
                    return "";
            }
        }

        public async Task DoTest()
        {
            var message = Test.CreateMessage("weather", 1.0, "cork", 1.0, new DateTime(2018,04,18), 1.0);
            var actionMessage = await DoActions(message);
            TextToSpeech.Speak(actionMessage);
            await JobScheduler.RunProgram();
        }

        #endregion
    }
}
