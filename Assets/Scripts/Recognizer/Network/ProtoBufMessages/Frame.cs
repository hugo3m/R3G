using ProtoBuf;

namespace RecognitionServer.Shared.ProtoBufMessages
{
    [ProtoContract]
    public class Frame
    {
        [ProtoMember(1)]
        public double[] Data { get; set; }

        [ProtoMember(2)]
        public string Id { get; set; }
    }
}
