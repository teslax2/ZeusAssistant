using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model
{
    public class VoiceRecorderEventArgs:EventArgs
    {
        public VoiceRecorderEventArgs(byte[] data, int lenght)
        {
            Data = data;
            Lenght = lenght;
        }
        public byte[] Data { get;}
        public int Lenght { get;}
    }
}
