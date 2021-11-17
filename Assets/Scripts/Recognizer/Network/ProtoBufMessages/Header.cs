using ProtoBuf;
using System.Threading;

namespace RecognitionServer.Shared.ProtoBufMessages
{
    public enum eType : byte { eError = 0, eFeedback, eFrame, eFrameRecognitionResult };

    [ProtoContract]
    public class Header
    {
        [ProtoMember(1)]
        public eType objectType;
        [ProtoMember(2)]
        public readonly int serialMessageId;

        public object data;
        private static int _HeaderSerialId = 0;

        public Header(object xData, eType xObjectType, int xSerialMessageId = 0)
        {
            data = xData;
            serialMessageId = (xSerialMessageId == 0) ? Interlocked.Increment(ref _HeaderSerialId) : xSerialMessageId;
            objectType = xObjectType; // we could use "if typeof(T) ...", but it would be slower, harder to maintain and less legible
        } // constructor

        // parameterless constructor needed for Protobuf-net
        public Header()
        {
        } // constructor

    }
}
