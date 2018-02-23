using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeusAssistant.Model;
using System.ComponentModel;
using System.IO;

namespace ZeusAssistant.ViewModel
{
    class ZeusMainViewModel:INotifyPropertyChanged
    {
        public MicrosoftSpeech _microsoftSpeech;
        public ApiHandler Api = new ApiHandler();
#region buttons
        //public CommandBinding StartRecording { get { return new CommandBinding(async () => await RunSpeechRecognition(), () => true); }}
        public CommandBinding StartRecording { get { return new CommandBinding(() => RunTest(), () => true); } }
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
            _microsoftSpeech = new MicrosoftSpeech();
        }

        #region functions
        public async Task RunSpeechRecognition()
        {
            //await _microsoftSpeech.Run();
            try
            {
                var file = File.ReadAllBytes(@"C:\Users\wiesi_000\Desktop\sample.mp3");
                var size = file.Length;
                int i = 0;
                byte[] dupa = new byte[5120];
                int bufferSize = 5120;
                while (bufferSize>0)
                {
                    Buffer.BlockCopy(file, i, dupa, 0, bufferSize);
                    i += bufferSize;
                    if (i + bufferSize > size)
                        bufferSize = size - i;
                    var response = await Api.Post(dupa);
                    System.Diagnostics.Debug.WriteLine(response);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

        }

        public void RunTest()
        {
            var response = Api.PostChunked2();
            System.Diagnostics.Debug.WriteLine(response);
        }
#endregion
    }
}
