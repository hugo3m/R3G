using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecognitionServer.Shared.ProtoBufMessages
{
    [ProtoContract]
    public class FrameRecognitionResult
    {
        [ProtoMember(1)]
        public Dictionary<string, double> Scores;


        [ProtoMember(2)]
        public bool ScoreCanBeUsed;

        [ProtoMember(3)]
        public string Id;

    }
}
