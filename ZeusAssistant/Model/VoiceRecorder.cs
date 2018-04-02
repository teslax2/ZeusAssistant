using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.IO;
using NAudio.Lame;

namespace ZeusAssistant.Model
{
    public class VoiceRecorder
    {
        private WaveInEvent _wave;
        private LameMP3FileWriter _lameWriter;
        public MemoryStream _lameStream;
        public event EventHandler<VoiceRecorderEventArgs> DataAvailable;
        public event EventHandler<SilenceDetectedEventArgs> SilenceDetected;
        public event EventHandler RecordingStopped;
        public float VoiceDetectionLevel { get; set; }
        public TimeSpan SilenceDuration { get; set; }
        public DateTime RecordingStarted { get; set; }
        private DateTime silenceStartTime;
        private bool recording;
        private void OnDataAvailable(byte[] data, int lenght)
        {
            EventHandler<VoiceRecorderEventArgs> dataAvailable = DataAvailable;
            if (dataAvailable != null)
                dataAvailable(this, new VoiceRecorderEventArgs(data,lenght));
        }
        private void OnSilenceDetected(TimeSpan time)
        {
            var silenceDetected = SilenceDetected;
            if (silenceDetected != null)
                silenceDetected(this, new SilenceDetectedEventArgs(time));
        }
        private void OnRecordingStopped()
        {
            var recordingStopped = RecordingStopped;
            if (recordingStopped != null)
                recordingStopped(this, new EventArgs());
        }

        public VoiceRecorder(float voiceDetectionLevel, int silenceDuration)
        {
            VoiceDetectionLevel = voiceDetectionLevel;
            SilenceDuration = TimeSpan.FromMilliseconds(silenceDuration);
            _lameStream = new MemoryStream();
        }

        public void Start()
        {
            silenceStartTime = DateTime.Now;
            _wave = new WaveInEvent();
            _wave.BufferMilliseconds = 100;
            _wave.DataAvailable += _wave_DataAvailable;
            _wave.RecordingStopped += _wave_RecordingStopped;
            _wave.StartRecording();
            recording = true;
            RecordingStarted = DateTime.Now;
            _lameWriter = new LameMP3FileWriter(_lameStream, _wave.WaveFormat, 64);
        }

        public void Stop()
        {
            if (_wave != null & recording)
            {
                _wave.StopRecording();
                recording = false;
            }
        }

        private void _wave_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (true)
            {
                var dataChunk = EncodeToMp3(e.Buffer);
                OnDataAvailable(dataChunk,dataChunk.Length);
                CheckSoundLevel(e);
                CheckSilenceDuration();
            }
        }
        /// <summary>
        /// Stream copied to mp3 encoder
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] EncodeToMp3(byte[] data)
        {
             var positionStart = (int)_lameStream.Position;
            _lameWriter.Write(data, 0, data.Length);
            var positionStop = (int)_lameStream.Position;
            var lenght = positionStop - positionStart;
            byte[] encodedBytes = new byte[lenght];
            _lameStream.Seek(positionStart, SeekOrigin.Begin);
            _lameStream.Read(encodedBytes, 0, lenght);
            _lameStream.Seek(positionStop, SeekOrigin.Begin);
            return encodedBytes;
        }

        private void _wave_RecordingStopped(object sender, StoppedEventArgs e)
        {
            OnRecordingStopped();
            if (_wave != null)
                _wave.Dispose();
            if (_lameStream != null)
                _lameWriter.Dispose();

        }

        /// <summary>
        /// Check voice level to start silence timer
        /// </summary>
        /// <param name="e"></param>
        private void CheckSoundLevel(WaveInEventArgs e)
        {
            float max = 0;

            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                short sample = (short)(e.Buffer[i + 1] << 8 | e.Buffer[i]);
                var sample32 = sample / 32768f;
                if (sample32 < 0) sample32 = -sample32;
                // is this the max value?
                if (sample32 > max) max = sample32;
            }
            if (max > VoiceDetectionLevel)
            {
                silenceStartTime = DateTime.Now;
            }
            System.Diagnostics.Debug.WriteLine(recording.ToString());
        }
        /// <summary>
        /// Check if silence takes longer than silenceTime and after 2 secs from begining
        /// </summary>
        private void CheckSilenceDuration()
        {
            var now = DateTime.Now;
            var silenceTime = now - silenceStartTime;
            var sinceStarted = now - RecordingStarted;
            System.Diagnostics.Debug.WriteLine("Silence time: " + silenceTime.ToString());
            if (silenceTime > SilenceDuration & sinceStarted.Seconds > 1)
            {
                Stop();
                OnSilenceDetected(silenceTime);
            }
        }
    }
}
