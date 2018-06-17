using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace ZeusAssistant.Model
{
    public static class JinglePlayer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void PlaySound()
        {
            try
            {
                var playTaskAsync = new System.Threading.Tasks.Task(() =>
                {
                    using (var reader = new WaveFileReader("apert.wav"))
                    using (var waveOut = new WaveOutEvent())
                    {
                        waveOut.Init(reader);
                        waveOut.Play();
                        System.Threading.Thread.Sleep(500);
                    }
                });
                playTaskAsync.Start();
            }
            catch (Exception ex)
            {

                logger.Error(ex.Message, "Error is playing Jingle");
            }
        }
    }
}
