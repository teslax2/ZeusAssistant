using System;

namespace ZeusAssistant.Model
{
    public class SilenceDetectedEventArgs:EventArgs
    {
        public TimeSpan Time { get; set; }
        public SilenceDetectedEventArgs (TimeSpan time)
        {
            Time = time;
        }
    }
}