using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Recognizer.DemonstrationClient
{
    public class DemonstrationClient
    {
        private string _receivedMessage;
        private AIResult _previousResult = new AIResult();

        private int _port { get;  set; }
        private string _ipAddress { get;  set; }
        private string _threadName { get;  set; }
        private NetworkStream _NetworkStream = null;
        private TcpClient _Client = null;
        private bool _ExitLoop = true;
        private BlockingCollection<string> _Queue = new BlockingCollection<string>();
        private byte[] buffer = new byte[180];


        public DemonstrationClient(string xIpAddress, int xPort, string xThreadName)
        {
            _port = xPort;
            _ipAddress = xIpAddress;
            _threadName = xThreadName;
            _receivedMessage = String.Empty;
        } //

        public AIResult GetAIResult()
        {
             return _previousResult;
        }

        public void Connect()
        {
            if (!_ExitLoop) return; // running already
            _ExitLoop = false;

            _Client = new TcpClient();
            _Client.Connect(_ipAddress, _port);
            _NetworkStream = _Client.GetStream();

            Thread lLoopRead = new Thread(new ThreadStart(LoopRead))
            {
                IsBackground = true,
                Name = _threadName + "Read"
            };
            lLoopRead.Start();

            Thread lLoopWrite = new Thread(new ThreadStart(LoopWrite))
            {
                IsBackground = true,
                Name = _threadName + "Write"
            };
            lLoopWrite.Start();
        } //

        public void Disconnect()
        {
            _ExitLoop = true;
            _Queue.Add(null);
            _Client?.Close();
            _NetworkStream?.Close();
        } //

        public void Send(string message)
        {
            if (message == null) return;
            //_PendingFeedbacks.Add(xHeader);
            _Queue.Add(message);
        }

        public void SendFrame(string data, double timestamp)
        {
            data = timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture) + "; " + data+'\n';
            _Queue.Add(data);
        }

        private void LoopWrite()
        {
            Debug.Log("Starting write loop");
            while (!_ExitLoop)
            {
                try
                {
                    string messageToSend = _Queue.Take();

                    // send header
                    byte[] data = Encoding.UTF8.GetBytes(messageToSend);
                    _NetworkStream.Write(data, 0, data.Length);
                    //Debug.Log("Sending "+data.Length+" bytes : " + messageToSend);
                }
                catch (System.IO.IOException e)
                {
                    if (_ExitLoop)
                    {
                        Debug.Log("user requested client shutdown.");
                    }
                    else
                    {
                        Debug.Log($"Disconnected: {e.StackTrace}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }

            _ExitLoop = true;
            Debug.Log("client: writer is shutting down");
        }


        private void LoopRead()
        {
            Debug.Log("Starting read loop");
            while (!_ExitLoop)
            {
                try
                {
                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = _NetworkStream.Read(buffer, 0, buffer.Length);
                    _receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, bytes);
                    if (_receivedMessage != "")
                    {
                        _previousResult = new AIResult(_receivedMessage);
                    }
                    //Debug.Log("Received : " + _receivedMessage);
                }
                catch (SocketException)
                {
                    // if there is an error in the socket shutdown everything and ask for a complete restart
                    Debug.Log("Client disconnected. Reconnect the server...");
                    _ExitLoop = true;
                }
                catch (System.IO.IOException)
                {
                    Debug.Log(_ExitLoop ? "user requested client shutdown" : "disconnected");
                    _ExitLoop = true;
                }
            }

            Debug.Log("client: reader is shutting down");
        } //
    }
}