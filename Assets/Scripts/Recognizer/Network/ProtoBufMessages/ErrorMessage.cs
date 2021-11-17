using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionServer.Shared.ProtoBufMessages
{
    [ProtoContract]
    public class ErrorMessage
    {
        [ProtoMember(1)]
        public string Text;
    }
}
