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
                using (var reader = new WaveFileReader("Alarm.wav"))
                using (var waveOut = new WaveOutEvent())
                {
                    var t = Task.Run(() =>
                    {
                        waveOut.Init(reader);
                        waveOut.Play();
                    });
                    t.Wait(1000);
                    await t;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error playing Tune");
            }
        }
    }
}
