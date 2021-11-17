using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Recognizer;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using TMPro;
using Recognizer.DataTools;

namespace Menus.Acquisition.View
{
    public class AcquisitionView : AcquisitionElement
    {
        public UserButton userButton;
        public GestureButton buttonGesture;

        public Canvas canvas;
        
        public GameObject ConfigPanel;
        public Text titleConfigPanel;
        public RectTransform ConfigContent;
        public Text currentConfigValue;
        public Button validate;
        
        public GameObject AcquisitionPanel;
        public GameObject nextGesturePanel;
        public GameObject gesturePanel;
        public TextMeshProUGUI[] StartStopButtonsText;
        
        public GameObject MenuPanel;
        public Text User;
        public Text Device;
        public Text Database;

        public GameObject ReplayPanel;
        public AnimationFileReader fileReader;
        public RawImage fileReaderRawImage;
        public RectTransform GestureContent;
        public Button AddGesture;

        public GameObject CreateClassPanel;
        public RectTransform ClassContent;
        public TMP_InputField ClassName;

        public GameObject DirectivesPanel;
        public RectTransform ExistingClasses;
        public RectTransform AddedClasses;
        public TMP_InputField NbofGestures;
        public Button AddClasses;
        public Button DeleteClasses;
        public Button ApplyAndQuit;

        public bool RecordPlaying;
        
        public TextMeshProUGUI countdown;
        private IEnumerator _delayedStart;
        public TMP_InputField delayField;

        public RecoManager Reco;
        
        public Text timerText;
        

        public void InitView()
        {   
            AcquisitionNotification.ActiveConfigPanel();
        }

        public void SelectUser()
        {
            this.titleConfigPanel.text = "Select User";
            List<UserData> listUsers = app.model.GetUsers();
            foreach (UserData user in listUsers)
            {
                UserButton newButton = Instantiate(userButton, ConfigContent.transform, true);
                newButton.gameObject.name = user.FirstName + " " + user.LastName;
                newButton.text.text = user.FirstName + " " + user.LastName;
                newButton.user = user;
                newButton.button.onClick.AddListener(() =>
                {
                    AcquisitionNotification.OnSelectedUser(newButton.user);
                });
            }
            validate.onClick.AddListener(() => AcquisitionNotification.Validate("device"));
            
        }
        public void StartStopRecord(bool standardAcquisition)
        {
            if (!RecordPlaying)
            {
                StartRecord(standardAcquisition);
            }
            else
            {
                StopRecording(standardAcquisition);
            }
        }
        
        private void StartRecord(bool standardAcquisition)
        {
            foreach(TextMeshProUGUI t in StartStopButtonsText)
            {
                t.text = "Stop";
            }
            AcquisitionNotification.StartStopTimer();
            _delayedStart = DelayedRecording(int.Parse(delayField.text), standardAcquisition);
            StartCoroutine(_delayedStart);
        }
        
        private void StopRecording(bool standardAcquisition)
        {
            AcquisitionNotification.Record(false, standardAcquisition);
            RecordPlaying = false;
            AcquisitionNotification.StartStopTimer();
            timerText.color = Color.yellow;
            foreach (TextMeshProUGUI t in StartStopButtonsText)
            {
                t.text = "Start";
            }
            delayField.interactable = true;
            StopCoroutine(_delayedStart);
            countdown.gameObject.SetActive(false);
        }
        
        IEnumerator DelayedRecording(int delay, bool standardAcquisition)
        {
            int d = delay;
            if (d > 0)
            {
                while (d >= 0)
                {
                    countdown.text = d.ToString();
                    countdown.gameObject.SetActive(true);
                    d--;
                    yield return new WaitForSeconds(1);
                }
            }

            countdown.gameObject.SetActive(false);
            RecordPlaying = true;
            //call the controller
            AcquisitionNotification.Record(true, standardAcquisition);
        }

        public void SelectDevice()
        {
            this.titleConfigPanel.text = "Select Device";

            List<DevicesInfos.Device> listDevices = app.model.GetDeviceList();
            foreach (DevicesInfos.Device device in listDevices)
            {
                UserButton newButton = Instantiate(userButton, ConfigContent.transform, true);
                newButton.gameObject.name = device.ToString();
                newButton.text.text = device.ToString();
                newButton.button.onClick.AddListener(() =>
                {
                    AcquisitionNotification.OnSelectedDevice(device);
                });
            }
            validate.onClick.AddListener(() => AcquisitionNotification.Validate("db"));
        }

        public void SelectDb()
        {
            this.titleConfigPanel.text = "Select DataBase";
            List<DataBase> listDb = app.model.GetDataBases();
            string modelDevice = app.model.GetDevice().ToString();
            Regex r = new Regex(@"^\w+");
            foreach (DataBase db in listDb)
            {
                if (r.Match(db.DeviceType).ToString() == modelDevice)
                {
                    UserButton newButton = Instantiate(userButton, ConfigContent.transform, true);
                    newButton.gameObject.name = db.Name;
                    newButton.text.text = db.Name;
                    newButton.button.onClick.AddListener(() =>
                    {
                        AcquisitionNotification.OnSelectedDb(db);
                    });
                }
            }
            validate.onClick.AddListener(() => AcquisitionNotification.Validate("end"));
        }

        public void CleanConfigPanel()
        {
            this.currentConfigValue.text = "";
            this.titleConfigPanel.text = "";
            validate.onClick.RemoveAllListeners();
            foreach (Transform child in ConfigContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void DisplayDataClasses()
        {
            foreach (Transform child in GestureContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            Dictionary<string, Func<List<GestureData>>> dic = DataManager
                .GetInstance(app.model.GetDataBase().GetFullPathDataClasses())
                .GetDataClassesLAZYAndCSVOnly();
            foreach (Func<List<GestureData>> s in dic.Values)
            {
                if(s().Count > 0)
                {
                    GestureData g = s()[0];
                    GestureButton newButton = Instantiate(buttonGesture, GestureContent.transform, true);
                    newButton.gameObject.name = g.Classe;
                    newButton.text.text = g.Classe;
                    newButton.playButton.onClick.AddListener(() =>
                    {
                        AcquisitionNotification.PlayFileReader(g);
                    });
                }
            }
        }

        public void DisplayDataClassesName()
        {
            foreach (Transform child in ClassContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            Dictionary<string, Func<List<GestureData>>> dic = DataManager
                .GetInstance(app.model.GetDataBase().GetFullPathDataClasses())
                .GetDataClassesLAZYAndCSVOnly();
            foreach (Func<List<GestureData>> s in dic.Values)
            {
                if (s().Count > 0)
                {
                    GestureData g = s()[0];
                    GestureButton newButton = Instantiate(buttonGesture, GestureContent.transform, true);
                    newButton.gameObject.name = g.Classe;
                    newButton.text.text = g.Classe;
                    newButton.playButton.onClick.AddListener(() =>
                    {
                        AcquisitionNotification.PlayFileReader(g);
                    });
                }
            }
        }

        public void DisplayData()
        {
            foreach (Transform child in GestureContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            List<GestureData> list = DataManager
                .GetInstance(app.model.GetDataBase().GetFullPathData())
                .GetDataLAZY();
            foreach(GestureData g in list)
            {
                GestureButton newButton = Instantiate(buttonGesture, GestureContent.transform, true);
                newButton.gameObject.name = g.DataName;
                newButton.text.text = g.DataName;
                newButton.playButton.onClick.AddListener(() =>
                {
                    AcquisitionNotification.PlayFileReader(g);
                });
            }
        }

        public void ActiveReplayPanel()
        {
            AcquisitionNotification.ActiveReplayPanel();
        }

        public void ActiveAcquisitionPanel()
        {
            AcquisitionNotification.ActiveAcquisitionPanel();
        }

        public void StartFileReader()
        {
            AcquisitionNotification.StartFileReader();
        }

        public void StopFileReader()
        {
            AcquisitionNotification.StopFileReader();
        }

        public void ActiveMenuPanel()
        {
            AcquisitionNotification.ActiveMenuPanel();
        }

        public void ActiveGestureClassesPanel()
        {
            AcquisitionNotification.ActiveGestureClassesPanel();
        }

        public void ActiveConfigPanel()
        {
            AcquisitionNotification.ActiveConfigPanel();
        }

        public void ActiveCreateClassPanel()
        {
            AcquisitionNotification.ActiveCreateClassPanel();
        }

    }
}