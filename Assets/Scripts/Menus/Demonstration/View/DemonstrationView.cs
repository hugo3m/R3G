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
        public UserButton UserButton;
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
        public Button Recap;

        public GameObject RecapPanel;
        public RectTransform PredictedGesturesContent;
        public AnimationFileReader fileReader;
        public RawImage fileReaderRawImage;
        public PredictedGesture PredictedGesture;
        public Text currentRecapGesture;
        public Text currentTimeRecap;


        public bool RecordPlaying = false;
        public bool GesturesInitialized = false;

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
                UserButton newButton = Instantiate(UserButton, ConfigContent.transform, false);
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
            StartStopButtonText.text = "STOP";
            _delayedStart = DelayedRecording(0);
            StartCoroutine(_delayedStart);
        }

        private void StopRecording()
        {
            DemonstrationNotification.Record(false);
            RecordPlaying = false;
            DemonstrationNotification.StartStopTimer();
            StartStopButtonText.text = "START";
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
                UserButton newButton = Instantiate(UserButton, ConfigContent.transform, false);
                newButton.gameObject.name = device.ToString();
                newButton.text.text = device.ToString();
                newButton.button.onClick.AddListener(() => { DemonstrationNotification.OnSelectedDevice(device); });
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
                    UserButton newButton = Instantiate(UserButton, ConfigContent.transform, false);
                    newButton.gameObject.name = db.Name;
                    newButton.text.text = db.Name;
                    newButton.button.onClick.AddListener(() => { DemonstrationNotification.OnSelectedDb(db); });
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

        public void ActiveDemonstrationPanel()
        {
            DemonstrationNotification.ActiveDemonstrationPanel();
        }

        public void ActiveRecapPanel()
        {
            DemonstrationNotification.ActiveRecapPanel();
        }

        public void StartFileReader()
        {
            DemonstrationNotification.StartFileReader();
        }

        public void StopFileReader()
        {
            DemonstrationNotification.StopFileReader();
        }

        public void ChangeToSceneUsersManagement()
        {
            InformationContainer.INCOMMING_SCENE = "Demonstration";
            SceneManager.LoadScene("UsersManagement");
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
                GestureTexts.Add(Instantiate(GestureText, GestureContent.transform, false));
                GestureTexts[i].gameObject.name = list[i].Classe;
                GestureTexts[i].text.text = list[i].Classe;
                GestureTexts[i].background.GetComponent<Image>().color = Color.HSVToRGB(0.3f, 0.1f, 0.7f);
                GestureTexts[i].gesture = list[i];
                GesturesInitialized = true;
                Texture2D texture = new Texture2D(1, 1);
                try
                {
                    Byte[] file = System.IO.File.ReadAllBytes(app.model.GetDataBase().GetFullPathDataClasses() + "/" +
                                                              list[i].Classe + ".png");
                    texture.LoadImage(file);
                    GestureTexts[i].image.texture = texture;
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        "Demonstration need a PNG Image associate to each class : format is 'ClassName.png'");
                }
            }
        }

        public void RefreshGestures()
        {
            if (!GesturesInitialized) return;
            List<GestureData> list = app.model.GetAvailableGestures();
            int predictedGesture = app.model.GetAIResult().PredictedGesture();
            for (int i = 0; i < list.Count; i++)
            {
                if (predictedGesture == i && i != 0)
                {
                    GestureTexts[predictedGesture].background.GetComponent<Image>().color = Color.HSVToRGB(0.4f, 1f, 1f);
                    float lastTime = 0;
                    foreach (float time in app.model.GetPredictedGestures().Keys)
                    {
                        if (time > lastTime)
                        {
                            lastTime = time;
                        }
                    }
                    if (app.model.GetPredictedGestures().Count == 0 || 
                        GestureTexts[predictedGesture].gesture.Classe != app.model.GetPredictedGestures()[lastTime].Classe ||
                        (Time.time - lastTime > 3000 && GestureTexts[predictedGesture].gesture.Classe == app.model.GetPredictedGestures()[lastTime].Classe))
                    {
                        app.model.AddPredictedGestures(Time.time - app.model.GetStartTime(),GestureTexts[predictedGesture].gesture);
                    }

                    Vector3 position = GestureTexts[predictedGesture].transform.localPosition;
                    position.z -= 5;
                    position.z = Mathf.Max(-150, position.z);
                    GestureTexts[predictedGesture].transform.localPosition = position;
                }
                else
                {
                    if (predictedGesture > -1)
                    {
                        if (GestureTexts[i].transform.localPosition.z <= 0)
                        {
                            Vector3 position = GestureTexts[i].transform.localPosition;
                            position.z = 0;
                            GestureTexts[i].transform.localPosition = position;
                        }

                        GestureTexts[i].background.GetComponent<Image>().color = Color.HSVToRGB(
                            0.1f, 0.2f,
                            0.65f);
                    }
                    else
                    {
                        if (GestureTexts[i].transform.localPosition.z <= 0)
                        {
                            Vector3 position = GestureTexts[i].transform.localPosition;
                            position.z = 0;
                            GestureTexts[i].transform.localPosition = position;
                        }

                        GestureTexts[i].background.GetComponent<Image>().color = Color.HSVToRGB(
                            0.1f + Convert.ToSingle(app.model.GetAIResult().scores[i]) / 3.0f,
                            0.2f + Convert.ToSingle(app.model.GetAIResult().scores[i]) / 3.0f,
                            0.65f + Convert.ToSingle(app.model.GetAIResult().scores[i]) / 3.0f);
                    }

                    /*Vector3 position = GestureTexts[i].transform.localPosition;
                    position.z -= 5;
                    GestureTexts[predictedGesture].transform.localPosition = position;*/
                }
            }
        }

        public void DisplayPredictedGesture()
        {
            foreach (Transform child in PredictedGesturesContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            Dictionary<float,GestureData> dic = app.model.GetPredictedGestures();
            foreach (float time in dic.Keys)
            {
                float minutes = ((int)time / 60);
                float seconds = (time % 60);
                PredictedGesture p = Instantiate(PredictedGesture, PredictedGesturesContent.transform, false);
                p.text.text = dic[time].Classe + Environment.NewLine + string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }
}