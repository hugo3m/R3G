using RecognitionServer.Shared.ProtoBufMessages;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace RecognitionServer.Shared.Network
{
    public class NetworkClient
    {
        public delegate void OnMessageErrorDelegate(ErrorMessage errorMessage);

        public event OnMessageErrorDelegate OnMessageError;

        public delegate void OnFrameRecognitionResultDelegate(FrameRecognitionResult errorMessage);

        public event OnFrameRecognitionResultDelegate OnFrameRecognitionResult;

        public int Port { get; private set; }
        public string IpAddress { get; private set; }
        public string ThreadName { get; private set; }
        private NetworkStream _NetworkStream = null;
        private TcpClient _Client = null;
        private bool _ExitLoop = true;
        private BlockingCollection<Header> _Queue = new BlockingCollection<Header>();
        private readonly PendingFeedbacks _PendingFeedbacks = new PendingFeedbacks();


        public NetworkClient(string xIpAddress, int xPort, string xThreadName)
        {
            Port = xPort;
            IpAddress = xIpAddress;
            ThreadName = xThreadName;
        } //

        public void Connect()
        {
            if (!_ExitLoop) return; // running already
            _ExitLoop = false;

            _Client = new TcpClient();
            _Client.Connect(IpAddress, Port);
            _NetworkStream = _Client.GetStream();

            Thread lLoopWrite = new Thread(new ThreadStart(LoopWrite))
            {
                IsBackground = true,
                Name = ThreadName + "Write"
            };
            lLoopWrite.Start();

            Thread lLoopRead = new Thread(new ThreadStart(LoopRead))
            {
                IsBackground = true,
                Name = ThreadName + "Read"
            };
            lLoopRead.Start();
        } //

        public void Disconnect()
        {
            _ExitLoop = true;
            _Queue.Add(null);
            if (_Client != null) _Client.Close();
            //if (_NetworkStream != null) _NetworkStream.Close();
        } //

        public void Send(Header xHeader)
        {
            if (xHeader == null) return;
            //_PendingFeedbacks.Add(xHeader);
            _Queue.Add(xHeader);
        } //


        private void LoopWrite()
        {
            Console.WriteLine("Starting write loop");
            while (!_ExitLoop)
            {
                try
                {
                    Header lHeader = _Queue.Take();
                    if (lHeader == null) break;

                    // send header
                    ProtoBuf.Serializer.SerializeWithLengthPrefix<Header>(_NetworkStream, lHeader,
                        ProtoBuf.PrefixStyle.Fixed32);

                    // send data
                    switch (lHeader.objectType)
                    {
                        case eType.eFrame:
                            ProtoBuf.Serializer.SerializeWithLengthPrefix<Frame>(_NetworkStream, (Frame) lHeader.data,
                                ProtoBuf.PrefixStyle.Fixed32);
                            break;
                        case eType.eError:
                            ProtoBuf.Serializer.SerializeWithLengthPrefix<string>(_NetworkStream, (string) lHeader.data,
                                ProtoBuf.PrefixStyle.Fixed32);
                            break;
                        default:
                            break;
                    }
                }
                catch (System.IO.IOException e)
                {
                    if (_ExitLoop)
                    {
                        Console.WriteLine("user requested client shutdown.");
                    }
                    else
                    {
                        Console.WriteLine($"Disconnected: {e.StackTrace}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            _ExitLoop = true;
            Console.WriteLine("client: writer is shutting down");
        }


        private void LoopRead()
        {
            Debug.Log("Starting read loop");
            while (!_ExitLoop)
            {
                try
                {

                    Header lHeader =
                        ProtoBuf.Serializer.DeserializeWithLengthPrefix<Header>(_NetworkStream,
                            ProtoBuf.PrefixStyle.Fixed32);
                    if (lHeader == null) break;
                    if (lHeader.objectType == eType.eError)
                    {
                        ErrorMessage lErrorMessage =
                            ProtoBuf.Serializer.DeserializeWithLengthPrefix<ErrorMessage>(_NetworkStream,
                                ProtoBuf.PrefixStyle.Fixed32);
                        lHeader.data = lErrorMessage;
                        OnMessageError?.Invoke(lErrorMessage);
                    }
                    else if (lHeader.objectType == eType.eFrameRecognitionResult)
                    {
                        FrameRecognitionResult frameRecognitionResult =
                            ProtoBuf.Serializer.DeserializeWithLengthPrefix<FrameRecognitionResult>(_NetworkStream,
                                ProtoBuf.PrefixStyle.Fixed32);
                        lHeader.data = frameRecognitionResult;
                        OnFrameRecognitionResult?.Invoke(frameRecognitionResult);
                    }

                    //_PendingFeedbacks.Remove(lHeader);
                }
                catch (SocketException)
                {
                    // if there is an error in the socket shutdown everything and ask for a complete restart
                    Debug.Log("Client disconnected. Reconnect the server...");
                }
                catch (System.IO.IOException)
                {
                    if (_ExitLoop) Debug.Log("user requested client shutdown");
                    //else Console.WriteLine("disconnected");
                }
            }

            Console.WriteLine("client: reader is shutting down");
        } //
    } // class
}