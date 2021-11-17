using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Menus.CreateGesture.Model;
using Recognizer;
using UnityEngine;
using System.Reflection;
using Menus.Acquisition.Model;
using RecognitionServer.Shared.Network;
using Recognizer.DataTools;
using UnityEngine.UI;
using Unity.UNetWeaver;

namespace Menus.Acquisition.Controller
{
    /// <summary>
    /// cf. diagramme de classe "Users"
    /// </summary>
    public class AcquisitionController : MenuController<AcquisitionApplication>
    {
        private bool _recordPlaying = false;
        private Double[] _deviceFrame;
        private Double[] _frameWithTime;
        private StringBuilder _outData;
        private List<StringBuilder> _outDataJoints;
        private StringBuilder _outDataInkML;
        private List<List<string[]>> _jointsPositionTimeList;
        private StringBuilder _outDataTimeStamped;
        private float _timeRemaining = 10.0f;

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
            AcquisitionNotification.Record += Record;
            AcquisitionNotification.Validate += Validate;
            AcquisitionNotification.StartStopTimer += StartStopTimer;
            AcquisitionNotification.ActiveReplayPanel += ActiveReplayPanel;
            AcquisitionNotification.ActiveAcquisitionPanel += ActiveAcquisitionPanel;
            AcquisitionNotification.PlayFileReader += PlayFileReader;
            AcquisitionNotification.StartFileReader += StartFileReader;
            AcquisitionNotification.StopFileReader += StopFileReader;
            AcquisitionNotification.ActiveMenuPanel += ActiveMenuPanel;
            AcquisitionNotification.ActiveGestureClassesPanel += ActiveGesturePanel;
            AcquisitionNotification.ActiveConfigPanel += ActiveConfigPanel;
            AcquisitionNotification.OnSelectedUser += OnUser;
            AcquisitionNotification.OnSelectedDevice += OnDevice;
            AcquisitionNotification.OnSelectedDb += OnDataBase;
            AcquisitionNotification.ActiveCreateClassPanel += ActiveCreateClassPanel;
            AcquisitionNotification.SetClassName += SetClassName;
            AcquisitionNotification.OnSelectedGesture += OnSelectedGesture;
            AcquisitionNotification.OnAddClass += OnAddClass;
            AcquisitionNotification.OnDeleteClass += OnDeleteClass;
            AcquisitionNotification.ActiveDirectivesPanel += ActiveDirectivesPanel;
            AcquisitionNotification.OnRandomClasses += OnRandomClasses;
            AcquisitionNotification.ValidateGesturesToDo += ValidateGesturesToDo;
            AcquisitionNotification.CleanFileReader += CleanFileReader;
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
            AcquisitionNotification.Record -= Record;
            AcquisitionNotification.Validate -= Validate;
            AcquisitionNotification.StartStopTimer -= StartStopTimer;
            AcquisitionNotification.ActiveReplayPanel -= ActiveReplayPanel;
            AcquisitionNotification.ActiveAcquisitionPanel -= ActiveAcquisitionPanel;
            AcquisitionNotification.PlayFileReader -= PlayFileReader;
            AcquisitionNotification.StartFileReader -= StartFileReader;
            AcquisitionNotification.StopFileReader -= StopFileReader;
            AcquisitionNotification.ActiveMenuPanel -= ActiveMenuPanel;
            AcquisitionNotification.ActiveGestureClassesPanel -= ActiveGesturePanel;
            AcquisitionNotification.ActiveConfigPanel -= ActiveConfigPanel;
            AcquisitionNotification.OnSelectedUser -= OnUser;
            AcquisitionNotification.OnSelectedDevice -= OnDevice;
            AcquisitionNotification.OnSelectedDb -= OnDataBase;
            AcquisitionNotification.ActiveCreateClassPanel -= ActiveCreateClassPanel;
            AcquisitionNotification.SetClassName -= SetClassName;
            AcquisitionNotification.OnSelectedGesture -= OnSelectedGesture;
            AcquisitionNotification.OnAddClass -= OnAddClass;
            AcquisitionNotification.ActiveDirectivesPanel -= ActiveDirectivesPanel;
            AcquisitionNotification.OnRandomClasses -= OnRandomClasses;
            AcquisitionNotification.ValidateGesturesToDo -= ValidateGesturesToDo;
            AcquisitionNotification.CleanFileReader -= CleanFileReader;
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
            while (true)
            {
                if (_recordPlaying)
                {

                    StringBuilder frame = new StringBuilder("");
                    long timestamp = watch.ElapsedMilliseconds;
                    RecoManager.GetInstance().DetectAndFillPositionDeviceInkML(ref _frameWithTime, timestamp);
                    RecoManager.GetInstance().DetectAndFillPositionDevice(ref _deviceFrame);

                    int i = 3;
                    for (int j = 0; j < _outDataJoints.Count; j++)
                    {
                        _outDataJoints[j].Append(_frameWithTime[i - 3].ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(" ")
                            .Append(_frameWithTime[i - 2].ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(" ")
                            .Append(_frameWithTime[i - 1].ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(" ")
                            .Append(_frameWithTime[i].ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(", ");
                        i += 4;
                    }
                    
                    /*int i = 3;
                    for (int j = 0; j < _jointsPositionTimeList.Count; j++)
                    {
                        _jointsPositionTimeList[j].Add(new string[]
                        {
                            _frameWithTime[i - 3].ToString(System.Globalization.CultureInfo.InvariantCulture),
                            _frameWithTime[i - 2].ToString(System.Globalization.CultureInfo.InvariantCulture),
                            _frameWithTime[i - 1].ToString(System.Globalization.CultureInfo.InvariantCulture),
                            _frameWithTime[i].ToString(System.Globalization.CultureInfo.InvariantCulture)
                        });
                        i += 4;
                    }*/

                    for (int k = 0; k < _deviceFrame.Length; k++)
                     {
                         _frameWithTime[k] = _deviceFrame[k];
                         frame.Append(_deviceFrame[k].ToString(System.Globalization.CultureInfo.InvariantCulture))
                             .Append(" ");
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

        private void Record(bool start, bool standardAcquisition)
        {
            if (!start)
            {
                app.view.ClassNameToDo.text = "Class";
                GestureData[] gestures = StopRecord(standardAcquisition);
                if (standardAcquisition)
                {
                    DataManagerInkML.GetInstance(app.model.GetDataBase().GetFullPathData())
                        .AddData((GestureData)gestures[0]);
                   DataManagerInkML.GetInstance(app.model.GetDataBase().GetFullPathInkml())
                        .AddDataInkML((GestureDataInkML)gestures[1]);
                }
                else
                {
                    DataManager.GetInstance(app.model.GetDataBase().GetFullPathDataClasses())
                            .AddDataClassNameInActionsCSV(app.view.ClassName.text);
                    DataManagerInkML.GetInstance(app.model.GetDataBase().GetFullPathDataClasses())
                        .AddData((GestureData)gestures[0]);
                    
                    app.model.SetDataClassesAndAvailableGestures(DataManager.GetInstance(app.model.GetDataBase().GetFullPathDataClasses()).GetDataClassesLAZYAndCSVOnly());
                    app.view.DisplayDataClassesName();

                }
                
            }
            else
            {
                _timeRemaining = float.Parse(standardAcquisition ? app.view.DurationField[0].text : app.view.DurationField[1].text);
                if (_timeRemaining == 0) _timeRemaining = 9999;
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
            _outDataTimeStamped = new StringBuilder();
            _outDataJoints = new List<StringBuilder>();
            _outDataInkML = new StringBuilder();
            _jointsPositionTimeList = new List<List<string[]>>();
            for (int  i = 0;  i < _frameWithTime.Length/4;  i++)
            {
                _outDataJoints.Add(new StringBuilder().Append("\n\t\t<trace>\n\t\t"));
            }
            StartCoroutine(Recording());
        }

        /// <summary>
        ///  Stop l'enregistrement
        /// </summary>
        /// <returns> l'enregistrement</returns>
        private GestureData[] StopRecord(bool standardAcquisition)
        {
            app.view.SoundNotification.Play();
            _recordPlaying = false;
            string fileName = GetUniqueFileName();
            foreach (var sb in _outDataJoints)
            {
                sb.Remove(sb.Length-2, 2).Append("\n\t\t</trace>");
                _outDataInkML.Append(sb.ToString());
            }
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            if (standardAcquisition)
            {
                GestureData data = new GestureData(null, _outData.ToString(), "undefined", fileName);
                GestureDataInkML dataInkML = new GestureDataInkML(null, _outDataInkML.ToString(), "undefined",
                    fileName,
                    app.model.GetCurrentUser(), app.model.GetGesturesToDo(), _jointsPositionTimeList);
                return new[] { data, dataInkML };
            }
            else
            {
                GestureData data = new GestureData(null, _outData.ToString(), app.view.ClassName.text, app.view.ClassName.text);
                GestureDataInkML dataInkML = new GestureDataInkML(null, _outDataInkML.ToString(), app.view.ClassName.text,
                    fileName,
                    app.model.GetCurrentUser(), app.model.GetGesturesToDo(), _jointsPositionTimeList);
                return new[] { data, dataInkML };
            }
            
            
        }

        private string GetUniqueFileName()
        {
            return app.model.GetCurrentUser().LastName + app.model.GetCurrentUser().FirstName +
                   DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        }

        protected override void Enter()
        {
            app.view.StartStopRecord(true);
        }

        protected override void Update()
        {
            base.Update();
            if (_recordPlaying)
            {
                _timeRemaining -= Time.deltaTime;
                if (_timeRemaining < 0)
                {
                    app.view.StartStopRecord(app.view.AcquisitionPanel.activeInHierarchy);
                    return;
                }
                float t = Time.time - app.model.GetStartTime();
                string minutes = ((int)t / 60).ToString();
                string seconds = (t % 60).ToString("f0");
                foreach (Text timer in app.view.timerText)
                {
                    timer.text = "TEMPS EFFECTUE : " + minutes + "min " + seconds + "s";
                }
                
            }
            
            
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
                        AcquisitionNotification.OnSelectedDevice(DevicesInfos.Device.LeapMotion);
                    }
                    break;
                case "db":
                    app.view.CleanConfigPanel();
                    app.view.SelectDb();
                    break;
                case "end":
                    if(app.model.GetDataBase() != null)
                    {
                        app.view.CleanConfigPanel();
                        app.view.User.text = app.model.GetCurrentUser().FirstName + " " + app.model.GetCurrentUser().LastName;
                        app.view.Device.text = app.model.GetDevice().ToString();
                        app.view.Database.text = app.model.GetDataBase().Name;
                        DBManager.GetInstance().SetCurrentDB(app.model.GetDataBase());                        
                        RecoManager.GetInstance().SetInfos(app.model.GetDevice());
                        _deviceFrame = new double[RecoManager.GetInstance().DeviceInfo.FrameSize];
                        _frameWithTime = new double[RecoManager.GetInstance().DeviceInfo.FrameSize + RecoManager.GetInstance().DeviceInfo.FrameSize/3];
                        base.lateInstanciateProvider();
                        AcquisitionNotification.ActiveMenuPanel();
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
            app.view.ReplayPanel.SetActive(false);
            app.view.ConfigPanel.SetActive(false);
            app.view.AcquisitionPanel.SetActive(false);
            app.view.MenuPanel.SetActive(false);
            app.view.CreateClassPanel.SetActive(false);
            app.view.DirectivesPanel.SetActive(false);
        }

        public void ActiveReplayPanel()
        {
            this.ClosePanels();
            app.model.SetCurrentGesture(null);
            app.view.Delete.interactable = false;
            app.view.AddGesture.gameObject.SetActive(false);
            app.view.Back.onClick.RemoveAllListeners();
            app.view.Back.onClick.AddListener(() => this.ActiveAcquisitionPanel());
            app.view.Delete.onClick.RemoveAllListeners();
            app.view.Delete.onClick.AddListener(() => this.DeleteGesture());
            app.view.DisplayData();
            app.view.ReplayPanel.SetActive(true);
        }

        public void ActiveGesturePanel()
        {
            this.ClosePanels();
            app.model.SetCurrentGesture(null);
            app.view.Delete.interactable = false;
            app.model.SetDataClassesAndAvailableGestures(DataManager.GetInstance(app.model.GetDataBase().GetFullPath()).GetDataClassesLAZYAndCSVOnly());
            app.view.AddGesture.gameObject.SetActive(true);
            app.view.Back.onClick.RemoveAllListeners();
            app.view.Back.onClick.AddListener(() => this.ActiveMenuPanel());
            app.view.Delete.onClick.RemoveAllListeners();
            app.view.Delete.onClick.AddListener(() => this.DeleteClassGesture());
            app.view.DisplayDataClasses();
            app.view.ReplayPanel.SetActive(true);
        }

        public void ActiveAcquisitionPanel()
        {
            this.ClosePanels();
            if(app.model.GetGesturesToDo().Count > 0)
            {
                app.view.GestureToDoScrollView.SetActive(true);
            }
            else
            {
                app.view.GestureToDoScrollView.SetActive(false);
            }
            app.view.AcquisitionPanel.SetActive(true);
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
                app.view.fileReader.Play();
            }
        }

        public void StopFileReader()
        {
            app.view.fileReader.Pause();
        }

        public void CleanFileReader()
        {
            if (app.view.fileReader.isLoaded())
            {
                app.view.fileReader.Pause();
                app.view.fileReader.Clear();
            }
        }

        public void ActiveMenuPanel()
        {
            this.ClosePanels();
            app.view.DurationField[0].text = (app.model.GetGesturesToDo().Count * 5).ToString(); //Permet de mettre une proposition de temps d'acquisition en fonction du nombre de gestes à effectuer
            app.view.MenuPanel.SetActive(true);
        }

        public void ActiveConfigPanel()
        {
            this.ClosePanels();
            app.view.ConfigPanel.SetActive(true);
            IDeviceProvider[] devicesProvider = GameObject.FindObjectsOfType<IDeviceProvider>();
            foreach(IDeviceProvider device in devicesProvider)
            {
                GameObject.Destroy(device.gameObject);
            }
            AcquisitionNotification.Validate("user");
        }

        public void ActiveCreateClassPanel()
        {
            this.ClosePanels();
            app.model.SetDataClassesAndAvailableGestures(DataManager.GetInstance(app.model.GetDataBase().GetFullPath()).GetDataClassesLAZYAndCSVOnly());
            app.view.DisplayDataClassesName();
            app.view.CreateClassPanel.SetActive(true);
        }

        public void SetClassName(string name)
        {
            app.view.ClassName.text = name;
        }

        public void OnSelectedGesture(GestureData gesture)
        {
            app.model.SetCurrentGesture(gesture);
        }

        public void OnAddClass()
        {
            app.model.GetGesturesToDo().Add(app.model.GetCurrentGesture());
            app.view.RefreshGesturesToDo();
        }

        public void OnDeleteClass()
        {
            app.model.GetGesturesToDo().Remove(app.model.GetCurrentGesture());
            app.view.RefreshGesturesToDo();
        }

        public void ActiveDirectivesPanel()
        {
            this.ClosePanels();
            app.model.SetCurrentGesture(null);
            app.model.SetDataClassesAndAvailableGestures(DataManager.GetInstance(app.model.GetDataBase().GetFullPathDataClasses()).GetDataClassesLAZYAndCSVOnly());
            app.model.SetNbGesturesToDo(0);
            app.view.SelectAvailableClass();
            app.view.RefreshGesturesToDo();
            app.view.DirectivesPanel.SetActive(true);
        }

        public void OnRandomClasses()
        {
            if (app.view.NbofGestures.text != "")
            {
                app.model.SetNbGesturesToDo(int.Parse(app.view.NbofGestures.text));
                app.model.SetGesturesToDo();
                app.model.SetRandomlyGesturesToDo(app.model.GetNbGesturesToDo());
                app.view.RefreshGesturesToDo();
            }
        }

        public void ValidateGesturesToDo()
        {
            foreach (Transform child in app.view.GestureToDoContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            List<GestureData> list = app.model.GetGesturesToDo();
            foreach (GestureData g in list)
            {
                GestureButton newButton = Instantiate(app.view.buttonGesture, app.view.GestureToDoContent.transform, true);
                newButton.gameObject.name = g.Classe;
                newButton.text.text = g.Classe;
                newButton.playButton.onClick.AddListener(() =>
                {
                    app.view.ReplayRenderAcquisition.SetActive(true);
                    app.view.ClassNameToDo.text = g.Classe;
                    AcquisitionNotification.PlayFileReader(g);
                });
            }
        }

        public void DeleteClassGesture()
        {
            this.ActiveGesturePanel();
        }

        public void DeleteGesture()
        {
            DataManagerInkML.GetInstance(app.model.GetDataBase().GetFullPath()).DeleteDataAndInkML(app.model.GetCurrentGesture());
            this.ActiveReplayPanel();
        }

        /*public IEnumerator ShowMessage(string message, float delay)
        {
            
        }*/
    }
}