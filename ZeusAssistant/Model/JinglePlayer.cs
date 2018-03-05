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
        public static void PlaySound()
        {
            var playTask = new System.Threading.Tasks.Task(() =>
            {
                using (var reader = new WaveFileReader("apert.wav"))
                using (var waveOut = new WaveOutEvent())
                {
                    waveOut.Init(reader);
                    waveOut.Play();
                    System.Threading.Thread.Sleep(1000);
                }
            });
            playTask.Start();
        }
    }
}
