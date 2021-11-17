using RecognitionServer.Shared.ProtoBufMessages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RecognitionServer.Shared.Network
{
    public class NetworkListener
    {

        private bool _ExitLoop = true;
        private TcpListener _Listener;

        public delegate void dOnMessageFrame(TcpClient xSender, Header xHeader, Frame xFrame);
        public event dOnMessageFrame OnMessageFrame;


        private List<TcpClient> _Clients = new List<TcpClient>();

        public int Port { get; private set; }
        public string IpAddress { get; private set; }
        public string ThreadName { get; private set; }

        public NetworkListener(string xIpAddress, int xPort, string xThreadName)
        {
            Port = xPort;
            IpAddress = xIpAddress;
            ThreadName = xThreadName;
        } //

        public bool Connect()
        {
            if (!_ExitLoop)
            {
                Console.WriteLine("Listener running already");
                return false;
            }
            _ExitLoop = false;

            try
            {
                _Listener = new TcpListener(IPAddress.Parse(IpAddress), Port);
                _Listener.Start();

                Thread lThread = new Thread(new ThreadStart(LoopWaitingForClientsToConnect));
                lThread.IsBackground = true;
                lThread.Name = ThreadName + "WaitingForClients";
                lThread.Start();

                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return false;
        }

        public void Disconnect()
        {
            _ExitLoop = true;
            lock (_Clients)
            {
                foreach (TcpClient lClient in _Clients) lClient.Close();
                _Clients.Clear();
            }
        } //        

        private void LoopWaitingForClientsToConnect()
        {

            Console.WriteLine("Starting wait loop");
            try
            {
                while (!_ExitLoop)
                {
                    Console.WriteLine("waiting for a client");
                    TcpClient lClient = _Listener.AcceptTcpClient();
                    string lClientIpAddress = lClient.Client.LocalEndPoint.ToString();
                    Console.WriteLine("new client connecting: " + lClientIpAddress);
                    if (_ExitLoop) break;
                    lock (_Clients) _Clients.Add(lClient);

                    Thread lThread = new Thread(new ParameterizedThreadStart(LoopRead));
                    lThread.IsBackground = true;
                    lThread.Name = ThreadName + "CommunicatingWithClient";
                    lThread.Start(lClient);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
                _ExitLoop = true;
                if (_Listener != null) _Listener.Stop();
            }
        } // 

        private void LoopRead(object xClient)
        {
            Console.WriteLine("Starting reading thread");
            TcpClient lClient = xClient as TcpClient;
            NetworkStream lNetworkStream = lClient.GetStream();

            while (!_ExitLoop)
            {
                try
                {

                    Header lHeader = ProtoBuf.Serializer.DeserializeWithLengthPrefix<Header>(lNetworkStream, ProtoBuf.PrefixStyle.Fixed32);
                    //Console.WriteLine("Received message");

                    if (lHeader == null) break; // happens during shutdown process
                    
                    switch (lHeader.objectType)
                    {
                       
                        case eType.eFrame:
                            Frame frame = ProtoBuf.Serializer.DeserializeWithLengthPrefix<Frame>(lNetworkStream, ProtoBuf.PrefixStyle.Fixed32);
                            if (frame == null) break;
                            lHeader.data = frame; // not necessary, but nicer                            

                            OnMessageFrame?.Invoke(lClient, lHeader, frame);
                            /*dOnMessageFrame lEventFrame = OnMessageFrame;
                            if (lEventFrame != null) 
                                lEventFrame(lClient, lHeader, lBook);*/
                            break;

                        default:
                            Console.WriteLine("Error: unknown message type");
                            break;
                    }
                }
                catch (System.IO.IOException)
                {
                    if (_ExitLoop) Console.WriteLine("user requested client shutdown");
                  //  else Console.WriteLine("disconnected");
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            Console.WriteLine("server: listener is shutting down");
        } //

        public static void Send(TcpClient xClient, Header xHeader)
        {
            if (xHeader == null) return;
            if (xClient == null) return;

            lock (xClient)
            {
                try
                {
                    NetworkStream lNetworkStream = xClient.GetStream();

                    // send header (most likely a simple feedback)
                    ProtoBuf.Serializer.SerializeWithLengthPrefix<Header>(lNetworkStream, xHeader, ProtoBuf.PrefixStyle.Fixed32);

                    // send errors
                    if (xHeader.objectType == eType.eError)
                    {
                        ProtoBuf.Serializer.SerializeWithLengthPrefix<ErrorMessage>(lNetworkStream, (ErrorMessage)xHeader.data, ProtoBuf.PrefixStyle.Fixed32);
                    }
                    else if(xHeader.objectType == eType.eFrameRecognitionResult)
                    {
                        ProtoBuf.Serializer.SerializeWithLengthPrefix<FrameRecognitionResult>(lNetworkStream, (FrameRecognitionResult)xHeader.data, ProtoBuf.PrefixStyle.Fixed32);
                    }
                    
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }

        } //

    } // class
}
