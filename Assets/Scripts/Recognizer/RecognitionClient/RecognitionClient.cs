using RecognitionServer.Shared.Network;
using RecognitionServer.Shared.ProtoBufMessages;
using System;
using System.Linq;
using System.Net.Sockets;

namespace RecognitionClient
{
    public class RecognitionClientC
    {
        private string _address;
        private int _port;
        private NetworkClient _networkClient;

        public delegate void OnErrorDelegate(string errorMessage);
        public event OnErrorDelegate OnError;

        //public delegate void OnFrameRecognitionResultDelegate(string id, string recognisedClass, double confidence, bool scoreCanBeUsed);
        public delegate void OnFrameRecognitionResultDelegate(string id, System.Collections.Generic.Dictionary<string, double> allMap, bool scoreCanBeUsed); // MODIF AVEC LUDO
        public event OnFrameRecognitionResultDelegate OnFrameRecognitionResult;

        public RecognitionClientC(string address, int port)
        {
            this._address = address;
            this._port = port;

            _networkClient = new NetworkClient(this._address, this._port, "RecognitionClientMainThread");
        }
        
        public void Connect()
        {
            bool _connectionSuccessfull = false;

            _networkClient.OnMessageError += _networkClient_OnMessageError;
            _networkClient.OnFrameRecognitionResult += _networkClient_OnFrameRecognitionResult;
            while (!_connectionSuccessfull)
            {
                try
                {
                    if (!_connectionSuccessfull)
                        _networkClient.Connect();

                    Console.WriteLine("Client connected");
                    UnityEngine.Debug.Log("Client Connected");
                    _connectionSuccessfull = true;
                }
                catch (SocketException e )
                {
                    Console.WriteLine("Unable to connect to server");
                    UnityEngine.Debug.Log("Unable to connect to server");
                }
            }
        }



        public void Disconnect()
        {

             Console.WriteLine("Disconnecting client");
            UnityEngine.Debug.Log("Disconnecting client");
            _networkClient.OnMessageError -= _networkClient_OnMessageError;
            _networkClient.Disconnect();
        }

        private void _networkClient_OnMessageError(ErrorMessage errorMessage)
        {
            OnError?.Invoke(errorMessage.Text);
        }


        private void _networkClient_OnFrameRecognitionResult(FrameRecognitionResult recoResult)
        {
            if (recoResult.Scores == null || recoResult.Scores.Count() == 0)
                return;


            /*const string scorePrefix = "Distance1.";

            var scoreToUse = recoResult.Scores.Where(l => l.Key.StartsWith(scorePrefix)).Select(l => new {
                ScoreName = l.Key.Substring(scorePrefix.Length),
                Score = l.Value
            }).OrderByDescending(a => a.Score).FirstOrDefault();*/


            if (recoResult.Scores != null)
            {
                OnFrameRecognitionResult?.Invoke(recoResult.Id, recoResult.Scores, /*scoreToUse.ScoreName, scoreToUse.Score, */recoResult.ScoreCanBeUsed);
            }
        }

        public void SendFrame(string id, double[] frame)
        {
            Header header = new Header(new Frame() { Data = frame, Id = id}, eType.eFrame);
            _networkClient.Send(header);
            
        }

        public void SendMsg(string msg)
        {
            Header header = new Header(new ErrorMessage() {Text = msg}, eType.eError);
            _networkClient.Send(header);
        }



    }
}
