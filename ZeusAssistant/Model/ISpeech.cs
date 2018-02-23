using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model
{
    interface ISpeech
    {

        Task Run();
        void Stop();
        event EventHandler<string> Recognized;
    }
}
