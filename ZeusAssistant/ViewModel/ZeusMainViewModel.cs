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
        private SpeechMicrosoft _microsoftSpeech;
        private SpeechWitAi _witAi;
        private VoiceRecorder _recorder;
        private Creditentials _credits;
        private WeatherApi _weatherApi;
        private JobScheduler _jobScheduler;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public HttpClient HttpClient { get; set; }

        #region buttons

        public CommandBinding StartRecording { get { return new CommandBinding(async () => await RunSpeechRecognition(), () => true); } }
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
            try
            {
                //load creditentials
                _credits = new Creditentials();
                _credits.Load();
                //client
                HttpClient = new HttpClient();
                //speech recognition
                _microsoftSpeech = new SpeechMicrosoft();
                _microsoftSpeech.Recognized += _microsoftSpeech_Recognized;
                //witAi
                _witAi = new SpeechWitAi(HttpClient,
                    _credits.Credits.Where((x) => x.Provider == ApiProvider.WitAi).First().Path,
                    _credits.Credits.Where((x) => x.Provider == ApiProvider.WitAi).First().Token);
                //voice recorder
                _recorder = new VoiceRecorder(0.05f, 1000);
                _recorder.DataAvailable += Recorder_DataAvailable;
                _recorder.RecordingStopped += async(s,e) => await Recorder_RecordingStopped(s,e);
                //weather api
                _weatherApi = new WeatherApi(HttpClient,
                    _credits.Credits.Where((x) => x.Provider == ApiProvider.OpenWeatherMap).First().Path,
                    _credits.Credits.Where((x) => x.Provider == ApiProvider.OpenWeatherMap).First().Token);
                //scheduler
                _jobScheduler = new JobScheduler();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to create ZeusMainViewModel");
                System.Windows.MessageBox.Show("GoodBye, check logs");
                App.Current.Shutdown();
            }
        }
        //recorder stopped
        //send wit Ai response to parser
        private async Task Recorder_RecordingStopped(object sender, EventArgs e)
        {
            var result = _witAi.StopPostChunked();
            if (result == null)
                return;
            //logger.Info(result.RawMessage);
            await DoActions(result);
         }
        //start keyword recognition
        public async Task RunSpeechRecognition()
        {
            await _microsoftSpeech.Run();
        }
        //recognized start keyword - fire recorder and wit ai
        private void _microsoftSpeech_Recognized(object sender, string e)
        {
            _witAi.StartPostChunked();
            _recorder.Start();
        }
        //recorder data available
        private void Recorder_DataAvailable(object sender, VoiceRecorderEventArgs e)
        {
            _witAi.SendPostChunked(e.Data);
        }
        #region functions
        private async Task DoActions(Model.Messages.Message action)
        {
            switch (action.MessageIntent)
            {
                case Model.Messages.IntentEnum.Weather:
                    var weatherAction = action as Model.Messages.MessageWeather;

                    var weatherString = await _weatherApi.GetForecastAsync(weatherAction.Location, weatherAction.When);
                    logger.Info(weatherString);
                    if (!string.IsNullOrEmpty(weatherString))
                        // _microsoftSpeech.Speak(weatherString);
                        ;
                    break;
                case Model.Messages.IntentEnum.Time:
                    return;
                case Model.Messages.IntentEnum.Alarm:
                    return;
                case Model.Messages.IntentEnum.Note:
                    return;
                default:
                    return;
            }
        }

        #endregion
    }
}
