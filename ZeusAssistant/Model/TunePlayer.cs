using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace ZeusAssistant.Model
{
    public static class TunePlayer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static object locker = new object();
        /// <summary>
        /// Plays sound
        /// </summary>
        public static void PlaySound()
        {
            try
            {
                using (var reader = new WaveFileReader("Speech On.wav"))
                using (var waveOut = new WaveOutEvent())
                {
                    waveOut.Init(reader);
                    waveOut.Play();
                    System.Threading.Thread.Sleep(reader.TotalTime.Milliseconds);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error playing Tune");
            }
        }

        /// <summary>
        /// Plays sound asynchronously - doesnt work!
        /// </summary>
        public static async Task PlaySoundAsync()
        {
            try
            {
                int delay = 0;
                var task1 = new Task(() => 
                {
                    lock (locker)
                    {
                        using (var reader = new WaveFileReader("Alarm.wav"))
                        using (var waveOut = new WaveOutEvent())
                        {
                            waveOut.Init(reader);
                            waveOut.Play();
                            delay = 1000;
                            logger.Info("plays inside");
                        }
                    }

                });
                task1.Start();
                await Task.Delay(delay);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error playing Tune");
            }
        }
    }
}
