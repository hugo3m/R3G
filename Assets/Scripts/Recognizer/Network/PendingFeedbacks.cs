using RecognitionServer.Shared.ProtoBufMessages;
using System;
using System.Collections.Concurrent;

namespace RecognitionServer.Shared.Network
{
    public class PendingFeedbacks
    {
        private readonly ConcurrentDictionary<int, Header> _Messages = new ConcurrentDictionary<int, Header>();

        public int Count { get { return _Messages.Count; } }

        public void Add(Header xHeader)
        {
            if (xHeader == null) throw new Exception("cannot add a null header");

            if (!_Messages.TryAdd(xHeader.serialMessageId, xHeader))
            {
                throw new Exception("there must be a programming error somewhere");
            }
        } //

        public void Remove(Header xHeader)
        {
            Header lHeader;
            if (!_Messages.TryRemove(xHeader.serialMessageId, out lHeader))
            {
                throw new Exception("there must be a programming error somewhere");
            }

            switch (xHeader.objectType)
            {
                case eType.eError:
                   // Console.WriteLine("error: " + ((ErrorMessage)xHeader.data).Text);
                   // Console.WriteLine("the message that was sent out was: " + lHeader.objectType + " with serial id " + lHeader.serialMessageId);
                   // Console.WriteLine("please check the log files" + Environment.NewLine);
                    break;
                case eType.eFeedback:
                    // all ok !
                    break;
                default:
                    Console.WriteLine("warning: This message type was not expected.");
                    break;
            }
        } //
    }
}
