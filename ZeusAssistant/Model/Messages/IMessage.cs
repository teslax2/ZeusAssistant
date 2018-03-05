using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    public interface IMessage
    {
        MessageIntent MessageIntent { get; set; }
        string RawMessage { get; set; }
        DateTime Created { get; set; }
        string Text { get; set; }
    }

}
