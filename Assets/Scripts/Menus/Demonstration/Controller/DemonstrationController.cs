using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Recognizer;
using UnityEngine;
using Recognizer.DataTools;
using Recognizer.DemonstrationClient;

namespace Menus.Demonstration.Controller
{
    /// <summary>
    /// cf. diagramme de classe "Users"
    /// </summary>
    public class DemonstrationController : MenuController<DemonstrationApplication>
    {
        private bool _recordPlaying = false;
        private bool _recapPlaying = false;
        private float _timeStartRecapPlaying;
        private float _stopRecapPlaying = 0;

        private Double[] _deviceFrame;
        private Double[] _frameWithTime;

        private StringBuilder _outData;
        private StringBuilder _outDataInkML;
        private List<StringBuilder> _outDataJoints;

        private DemonstrationClient _AIClient;
        private bool _isConnected;

        protected override void Start()
        {
            base.Start();
            app.model.SetUsers(UserManager.GetInstance().GetUsers());
            app.model.SetDeviceList(Enum.GetValues(typeof(DevicesInfos.Device)));
            app.model.SetDataBases(DBManager.GetInstance().GetDBs());
            app.view.InitView();
            RecoManager.GetInstance().StopRecognizer();
        }

        protected override void OnEnable()
        {
            base.lateOnEnable();
            DemonstrationNotification.Record += Record;
            DemonstrationNotification.Validate += Validate;
            DemonstrationNotification.StartStopTimer += StartStopTimer;
            DemonstrationNotification.ActiveConfigPanel += ActiveConfigPanel;
            DemonstrationNotification.OnSelectedUser += OnUser;
            DemonstrationNotification.OnSelectedDevice += OnDevice;
            DemonstrationNotification.OnSelectedDb += OnDataBase;
            DemonstrationNotification.PlayFileReader += PlayFileReader;
            DemonstrationNotification.StartFileReader += StartFileReader;
            DemonstrationNotification.StopFileReader += StopFileReader;
            DemonstrationNotification.ActiveDemonstrationPanel += ActiveDemonstrationPanel;
            DemonstrationNotification.ActiveRecapPanel += ActiveRecapPanel;

            try
            {
                RecoManager.GetInstance().StopRecognizer();
            }
            catch (Exception e)
            {
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DemonstrationNotification.Record -= Record;
            DemonstrationNotification.Validate -= Validate;
            DemonstrationNotification.StartStopTimer -= StartStopTimer;
            DemonstrationNotification.ActiveConfigPanel -= ActiveConfigPanel;
            DemonstrationNotification.OnSelectedUser -= OnUser;
            DemonstrationNotification.OnSelectedDevice -= OnDevice;
            DemonstrationNotification.OnSelectedDb -= OnDataBase;
            DemonstrationNotification.PlayFileReader -= PlayFileReader;
            DemonstrationNotification.StartFileReader -= StartFileReader;
            DemonstrationNotification.StopFileReader -= StopFileReader;
            DemonstrationNotification.ActiveDemonstrationPanel -= ActiveDemonstrationPanel;
            DemonstrationNotification.ActiveRecapPanel -= ActiveRecapPanel;
        }

        protected override void MoveRight()
        {
        }

        protected override void MoveLeft()
        {
        }

        private IEnumerator Recording()

        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            string line;
            StreamReader file =
                new StreamReader(
                    @"C:\Users\bapti\Documents\INSA\4INFO\R3G\Unity\derg3d_transversales-Transversales\ATAD\Assets\Scripts\Recognizer\DataTools\DataBases\Chalearn\Data\Sample00001_data.txt");
            while (true)
            {
                if (_recordPlaying)
                {
                    StringBuilder frame = new StringBuilder("");
                    double timestamp = watch.ElapsedMilliseconds / 1000.0;
                    RecoManager.GetInstance().DetectAndFillPositionDeviceInkML(ref _frameWithTime, timestamp);
                    RecoManager.GetInstance().DetectAndFillPositionDevice(ref _deviceFrame);

                    int i = 3;
                    for (int j = 0; j < _outDataJoints.Count; j++)
                    {
                        _outDataJoints[j]
                            .Append(_frameWithTime[i - 3].ToString(System.Globalization.CultureInfo.InvariantCulture))
                            .Append(" ")
                            .Append(_frameWithTime[i - 2].ToString(System.Globalization.CultureInfo.InvariantCulture))
                            .Append(" ")
                            .Append(_frameWithTime[i - 1].ToString(System.Globalization.CultureInfo.InvariantCulture))
                            .Append(" ")
                            .Append(_frameWithTime[i].ToString(System.Globalization.CultureInfo.InvariantCulture))
                            .Append(", ");
                        i += 4;
                    }

                    for (int k = 0; k < _deviceFrame.Length; k++)
                    {
                        _frameWithTime[k] = _deviceFrame[k];
                        frame.Append(_deviceFrame[k].ToString(System.Globalization.CultureInfo.InvariantCulture))
                            .Append(" ");
                    }

                    if (_isConnected)
                    {
                        /*
                            if ((line = file.ReadLine()) != null)
                            {
                                _AIClient.SendFrame(line, timestamp);
                            }*/
                        _AIClient.SendFrame(frame.ToString(), timestamp);


                        app.model.SetAIResult(_AIClient.GetAIResult());
                        app.view.RefreshGestures();
                        Debug.Log(_AIClient.GetAIResult().ToString());
                    }

                    _outData.Append(frame).Append("\n");
                }
                else
                {
                    yield break;
                }

                yield return new WaitForSeconds(RecoManager.FREQUENCY_RECORD_SEND_DATA);
            }
        }

        private void Record(bool start)
        {
            if (!start)
            {
                GestureData[] gestures = StopRecord();
                DataManagerInkML.GetInstance(app.model.GetDataBase().GetFullPathDemoData())
                    .AddData((GestureData) gestures[0]);
                DataManagerInkML.GetInstance(app.model.GetDataBase().GetFullPathDemoInkml())
                    .AddDataInkML((GestureDataInkML) gestures[1]);
                this.ActiveDemonstrationPanel();
                app.model.SetCurrentGesture(gestures[0]);
                app.view.Recap.interactable = true;
            }
            else
            {
                StartRecord();
            }
        }


        /// <summary>
        /// Démarre l'enregistrement
        /// </summary>
        private void StartRecord()
        {
            _recordPlaying = true;
            _outData = new StringBuilder();
            _outDataJoints = new List<StringBuilder>();
            _outDataInkML = new StringBuilder();
            for (int i = 0; i < _frameWithTime.Length / 4; i++)
            {
                _outDataJoints.Add(new StringBuilder().Append("\n\t\t<trace>"));
            }

            StartCoroutine(Recording());
        }

        /// <summary>
        ///  Stop l'enregistrement
        /// </summary>
        /// <returns> l'enregistrement</returns>
        private GestureData[] StopRecord()
        {
            _recordPlaying = false;
            string fileName = GetUniqueFileName();
            foreach (var sb in _outDataJoints)
            {
                sb.Remove(sb.Length - 2, 2).Append("</trace>");
                _outDataInkML.Append(sb.ToString());
            }

            GestureData data = new GestureData(null, _outData.ToString(), "undefined", fileName);
            GestureDataInkML dataInkML = new GestureDataInkML(null, fileName, _outDataInkML.ToString(), "undefined",
                app.model.GetCurrentUser(), new List<GestureData>());
            return new[] {data, dataInkML};
        }

        private string GetUniqueFileName()
        {
            return app.model.GetCurrentUser().LastName + app.model.GetCurrentUser().FirstName +
                   DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        }

        protected override void Enter()
        {
            app.view.StartStopRecord();
        }

        protected override void Update()
        {
        }

        protected void StartStopTimer()
        {
            if (app.model.GetFinished())
            {
                app.model.SetStartTime(Time.time);
                app.model.SetFinished(false);
            }
            else
            {
                app.model.SetFinished(true);
            }
        }

        protected void OnDataBase(DataBase dataBaseSelected)
        {
            app.model.SetDataBase(dataBaseSelected);
            app.view.currentConfigValue.text = app.model.GetDataBase().Name;
        }

        protected void OnUser(UserData userSelected)
        {
            app.model.SetCurrentUser(userSelected);
            app.view.currentConfigValue.text = userSelected.FirstName + " " + userSelected.LastName;
        }

        protected void OnDevice(DevicesInfos.Device device)
        {
            app.model.SetDevice(device);
            app.view.currentConfigValue.text = device.ToString();
            _deviceFrame = new double[RecoManager.GetInstance().DeviceInfo.FrameSize];
            _frameWithTime = new double[RecoManager.GetInstance().DeviceInfo.FrameSize +
                                        RecoManager.GetInstance().DeviceInfo.FrameSize / 3];
        }

        protected void Validate(string configcase)
        {
            switch (configcase)
            {
                case "user":
                    app.view.CleanConfigPanel();
                    app.view.SelectUser();
                    break;
                case "device":
                    if (app.model.GetCurrentUser() != null)
                    {
                        app.view.CleanConfigPanel();
                        app.view.SelectDevice();
                        DemonstrationNotification.OnSelectedDevice(DevicesInfos.Device.LeapMotion);
                    }

                    break;
                case "db":
                    app.view.CleanConfigPanel();
                    app.view.SelectDb();
                    break;
                case "end":
                    if (app.model.GetDataBase() != null)
                    {
                        app.view.CleanConfigPanel();
                        DBManager.GetInstance().SetCurrentDB(app.model.GetDataBase());
                        RecoManager.GetInstance().SetInfos(app.model.GetDevice());
                        _deviceFrame = new double[RecoManager.GetInstance().DeviceInfo.FrameSize];
                        _frameWithTime = new double[RecoManager.GetInstance().DeviceInfo.FrameSize +
                                                    RecoManager.GetInstance().DeviceInfo.FrameSize / 3];
                        app.model.SetDataClassesAndAvailableGestures(DataManager
                            .GetInstance(app.model.GetDataBase().GetFullPath()).GetDataClassesLAZYAndCSVOnly());

                        _AIClient = new DemonstrationClient("localhost", 80, "DemonstrationThread");
                        try
                        {
                            _AIClient.Connect();
                            _isConnected = true;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                            _isConnected = false;
                        }

                        if (_isConnected)
                        {
                            _AIClient.Send(app.model.GetDataBase().GetFullAIPath());
                            Debug.Log(app.model.GetDataBase().GetFullAIPath());
                        }
                        else
                        {
                            Debug.LogError("Serveur IA non connecté");
                        }

                        // _AIClient.Send("C:\\Users\\bapti\\Documents\\INSA\\4INFO\\R3G\\Unity\\derg3d_transversales-Transversales\\IA\\v1.1\\20210417-215716\\");
                        base.lateInstanciateProvider();
                        this.ActiveDemonstrationPanel();
                    }

                    break;
                default:
                    break;
            }
        }

        public void ClosePanels()
        {
            app.view.fileReader.Pause();
            app.view.fileReader.Clear();
            app.view.ConfigPanel.SetActive(false);
            app.view.DemonstrationPanel.SetActive(false);
            app.view.RecapPanel.SetActive(false);
        }

        public void ActiveConfigPanel()
        {
            this.ClosePanels();
            app.view.ConfigPanel.SetActive(true);
            IDeviceProvider[] devicesProvider = GameObject.FindObjectsOfType<IDeviceProvider>();
            foreach (IDeviceProvider device in devicesProvider)
            {
                GameObject.Destroy(device.gameObject);
            }

            _AIClient?.Disconnect();
            DemonstrationNotification.Validate("user");
        }

        public void ActiveDemonstrationPanel()
        {
            this.ClosePanels();
            app.view.DisplayGesture();
            app.view.DemonstrationPanel.SetActive(true);
        }

        public void ActiveRecapPanel()
        {
            this.ClosePanels();
            this.PlayFileReader(app.model.GetCurrentGesture());
            app.view.DisplayPredictedGesture();
            app.view.RecapPanel.SetActive(true);
        }

        public void PlayFileReader(GestureData data)
        {
            app.view.fileReaderRawImage.gameObject.SetActive(true);
            app.view.fileReader.LoadNewGeste(data.Path);
            
        }

        public void StartFileReader()
        {
            if (app.view.fileReader.isLoaded())
            {
                _recapPlaying = true;
                StartCoroutine(RecapPlaying());
                app.view.fileReader.Play();
            }
        }

        public void StopFileReader()
        {
            _recapPlaying = false;
            app.view.fileReader.ResetAnimWithPause();
            app.view.currentTimeRecap.text = "00:00";
        }

        public IEnumerator RecapPlaying()
        {
            while (true)
            {
                
                if (_recapPlaying)
                {
                    if (app.view.fileReader._currentFrameIndex == 1) _timeStartRecapPlaying = Time.time;
                    string currentGesture = "Aucun geste reconnu";
                    float currentTime = Time.time - _timeStartRecapPlaying;
                    float minutes = ((int)currentTime / 60);
                    float seconds = (currentTime % 60);
                    app.view.currentTimeRecap.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                    Dictionary<float, GestureData> dic = app.model.GetPredictedGestures();
                    foreach(float time in dic.Keys)
                    {
                        if (currentTime > time)
                        {
                            currentGesture = dic[time].Classe;
                        }
                    }
                    app.view.currentRecapGesture.text = currentGesture;
                }
                yield return new WaitForSeconds(RecoManager.FREQUENCY_RECORD_SEND_DATA);
            }
        }
    }
}