using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DefaultNamespace;
using Menus.Acquisition.View;
using Recognizer;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using TMPro;
using Recognizer.DataTools;
using UnityEngine.SceneManagement;

namespace Menus.Demonstration.View
{
    public class DemonstrationView : DemonstrationElement
    {
        public UserButton userButton;
        public Button CreateOrModifyUsersButton;
        public Button CreateNewDBButton;
        public Canvas canvas;
        
        public GameObject ConfigPanel;
        public Text titleConfigPanel;
        public RectTransform ConfigContent;
        public Text currentConfigValue;
        public Button validate;
        
        public GameObject DemonstrationPanel;
        public TextMeshProUGUI StartStopButtonText;
        private IEnumerator _delayedStart;
        public RectTransform GestureContent;
        public GestureText GestureText;
        private List<GestureText> GestureTexts = new List<GestureText>();
        

        public bool RecordPlaying=false;
        
        public RecoManager Reco;
        
        

        public void InitView()
        {   
            DemonstrationNotification.ActiveConfigPanel();
        }
        
        public void SelectUser()
        {
            CreateOrModifyUsersButton.gameObject.SetActive(true);
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
                    DemonstrationNotification.OnSelectedUser(newButton.user);
                });
            }
            validate.onClick.AddListener(() => DemonstrationNotification.Validate("device"));
            
        }
        public void StartStopRecord()
        {
            if (!RecordPlaying)
            {
                StartRecord();
            }
            else
            {
                StopRecording();
            }
        }
        
        private void StartRecord()
        {
            StartStopButtonText.text = "Stop";
            _delayedStart = DelayedRecording(0);
            StartCoroutine(_delayedStart);
        }
        
        private void StopRecording()
        {
            DemonstrationNotification.Record(false);
            RecordPlaying = false;
            DemonstrationNotification.StartStopTimer();
            StartStopButtonText.text = "Start";
            StopCoroutine(_delayedStart);
        }
        
        IEnumerator DelayedRecording(int delay)
        {
            int d = delay;
            if (d > 0)
            {
                while (d >= 0)
                {
                   d--;
                    yield return new WaitForSeconds(1);
                }
            }
            
            RecordPlaying = true;
            //call the controller
            DemonstrationNotification.StartStopTimer();
            DemonstrationNotification.Record(true);
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
                    DemonstrationNotification.OnSelectedDevice(device);
                });
            }
            validate.onClick.AddListener(() => DemonstrationNotification.Validate("db"));
        }

        public void SelectDb()
        {
            CreateNewDBButton.gameObject.SetActive(true);
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
                        DemonstrationNotification.OnSelectedDb(db);
                    });
                }
            }
            validate.onClick.AddListener(() => DemonstrationNotification.Validate("end"));
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
            CreateOrModifyUsersButton.gameObject.SetActive(false);
            CreateNewDBButton.gameObject.SetActive(false);
        }
        

       
        public void ActiveConfigPanel()
        {
            DemonstrationNotification.ActiveConfigPanel();
        }
        
        
        public void ChangeToSceneUsersManagement()
        {
            InformationContainer.INCOMMING_SCENE = "Demonstration";
            SceneManager.LoadScene ("UsersManagement");
        }

        public void ChangeToSceneDBManagement()
        {
            InformationContainer.INCOMMING_SCENE = "Demonstration";
            SceneManager.LoadScene(("DBManagement"));
        }

        public void DisplayGesture()
        {
            GestureTexts.Clear();
            foreach (Transform child in GestureContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            List<GestureData> list = app.model.GetAvailableGestures();
            for (int i = 0; i < list.Count; i++)
            {
                GestureTexts.Add(Instantiate(GestureText, GestureContent.transform, true));
                GestureTexts[i].gameObject.name = list[i].Classe;
                GestureTexts[i].text.text = list[i].Classe;
                // double hue = app.model.GetAIResult().scores[i];
                // GestureTexts[i].background.color = Color.HSVToRGB((hue>1)?1:Convert.ToSingle(hue), 1, 1);
            }
        }

    }
}