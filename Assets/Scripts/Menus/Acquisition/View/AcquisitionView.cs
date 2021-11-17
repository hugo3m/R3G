using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DefaultNamespace;
using Recognizer;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using TMPro;
using Recognizer.DataTools;
using UnityEngine.SceneManagement;

namespace Menus.Acquisition.View
{
    public class AcquisitionView : AcquisitionElement
    {
        public UserButton userButton;
        public GestureButton buttonGesture;
        public Button CreateOrModifyUsersButton;
        public Button CreateNewDBButton;
        public Canvas canvas;
        
        public GameObject ConfigPanel;
        public Text titleConfigPanel;
        public RectTransform ConfigContent;
        public Text currentConfigValue;
        public Button validate;
        
        public GameObject AcquisitionPanel;
        public TextMeshProUGUI[] StartStopButtonsText;
        public TextMeshProUGUI[] countdown;
        private IEnumerator _delayedStart;
        public TMP_InputField[] delayField;
        public TMP_InputField[] DurationField;
        public Text ClassNameToDo;
        public GameObject ReplayRenderAcquisition;
        public RectTransform GestureToDoContent;
        public GameObject GestureToDoScrollView;

        public GameObject MenuPanel;
        public Text User;
        public Text Device;
        public Text Database;

        public GameObject ReplayPanel;
        public AnimationFileReader fileReader;
        public RawImage fileReaderRawImage;
        public RectTransform GestureContent;
        public Button AddGesture;
        public Button Back;
        public Button Delete;

        public GameObject CreateClassPanel;
        public RectTransform ClassContent;
        public TMP_InputField ClassName;

        public GameObject DirectivesPanel;
        public RectTransform ExistingClasses;
        public RectTransform AddedClasses;
        public InputField NbofGestures;
        public Button AddClasses;
        public Button DeleteClasses;
        public Button ApplyAndQuit;

        public bool RecordPlaying;

        public AudioSource SoundNotification;

        public RecoManager Reco;
        
        public Text[] timerText;

        public Text EndAcquisitionText;
        

        public void InitView()
        {   
            AcquisitionNotification.ActiveConfigPanel();
            AddClasses.interactable = false;
            DeleteClasses.interactable = false;
            EndAcquisitionText.enabled = false;
        }

        public bool solution(string[] B)
        {
            // write your code in C# 6.0 with .NET 4.5 (Mono)
            List<List<char>> matrix = new List<List<char>>();
            for (int i = 0; i < B.Length; i++)
            {
                string s = B[i];
                matrix.Add(new List<char>());
                foreach (char c in s)
                {
                    matrix[i].Add(c);
                }
            }
            Dictionary<int[], char> guardPosition = new Dictionary<int[], char>();
            for (int i = 0; i < matrix.Count; i++)
            {
                for(int j = 0; j < matrix[i].Count; j++)
                {
                    int[] key = new int[] { i, j };
                    guardPosition.Add(key, matrix[i][j]);
                }
            }
            List<int[]> supervisedBoxes = new List<int[]>();
            foreach(KeyValuePair<int[], char> entry in guardPosition)
            {
                int i, j;
                switch (entry.Value)
                {
                    case '^':
                        i = entry.Key[0];
                        j = entry.Key[1];
                        for (int i2 = i - 1; i >= 0; i--)
                        {
                            if (matrix[i2][j] == '.')
                                supervisedBoxes.Add(new int[] { i2, j });
                            else
                                break;
                        }
                        break;
                    case 'v':
                        i = entry.Key[0];
                        j = entry.Key[1];
                        for (int i2 = i + 1; i < matrix.Count; i++)
                        {
                            if (matrix[i2][j] == '.')
                                supervisedBoxes.Add(new int[] { i2, j });
                            else
                                break;
                        }
                        break;
                    case '<':
                        i = entry.Key[0];
                        j = entry.Key[1];
                        for (int j2 = j - 1; j <= 0; j--)
                        {
                            if (matrix[i][j2] == '.')
                                supervisedBoxes.Add(new int[] { i, j2 });
                            else
                                break;
                        }
                        break;
                    case '>':
                        i = entry.Key[0];
                        j = entry.Key[1];
                        for (int j2 = j + 1; j < matrix[0].Count; j++)
                        {
                            if (matrix[i][j2] == '.')
                                supervisedBoxes.Add(new int[] { i, j2 });
                            else
                                break;
                        }
                        break;
                }
            }
            return false;

        }

        public void SelectAvailableClass()
        {
            foreach (Transform child in ExistingClasses.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (GestureData gesture in app.model.GetAvailableGestures())
            {
                GestureButton button = Instantiate(buttonGesture, ExistingClasses.transform, false);
                button.gameObject.name = gesture.Classe;
                button.text.text = gesture.Classe;
                button.playButton.onClick.AddListener(() =>
                {
                    AcquisitionNotification.OnSelectedGesture(gesture);
                    AddClasses.interactable = true;
                    DeleteClasses.interactable = false;
                });
                
            }
        }

        public void OnAddClassClicked()
        {
            AcquisitionNotification.OnAddClass();
            AddClasses.interactable = false;
        }
        
        public void RefreshGesturesToDo()
        {
            foreach (Transform child in AddedClasses.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (GestureData gesture in app.model.GetGesturesToDo())
            {
                GestureButton button = Instantiate(buttonGesture, AddedClasses.transform, false);
                button.gameObject.name = gesture.Classe;
                button.text.text = gesture.Classe;
                button.playButton.onClick.AddListener(() =>
                {
                    AcquisitionNotification.OnSelectedGesture(gesture);
                    DeleteClasses.interactable = true;
                    AddClasses.interactable = false;
                });
            }
        }

        public void OnDeleteClassClicked()
        {
            AcquisitionNotification.OnDeleteClass();
            DeleteClasses.interactable = false;
        }
        public void SelectUser()
        {
            CreateOrModifyUsersButton.gameObject.SetActive(true);
            this.titleConfigPanel.text = "Select User";
            List<UserData> listUsers = app.model.GetUsers();
            foreach (UserData user in listUsers)
            {
                UserButton newButton = Instantiate(userButton, ConfigContent.transform, false);
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
                if (standardAcquisition) StartRecord(standardAcquisition);
                if (!standardAcquisition && app.view.ClassName.text.Length > 0) StartRecord(standardAcquisition);
            }
            else
            {
                foreach (Text timer in this.timerText)
                {
                    timer.text = "00:00";
                }
                this.fileReader.Pause();
                this.fileReader.Clear();
                this.ClassNameToDo.text = "";
                StopRecording(standardAcquisition);
            }
        }
        
        private void StartRecord(bool standardAcquisition)
        {
            foreach(TextMeshProUGUI t in StartStopButtonsText)
            {
                t.text = "STOP";
            }
            foreach (Text timer in timerText)
            {
                timer.color = Color.white;
            }  
            _delayedStart = DelayedRecording(int.Parse(standardAcquisition ? delayField[0].text : delayField[1].text), standardAcquisition);
            StartCoroutine(_delayedStart);
        }
        
        private void StopRecording(bool standardAcquisition)
        {
            AcquisitionNotification.Record(false, standardAcquisition);
            RecordPlaying = false;
            AcquisitionNotification.StartStopTimer();
            foreach (Text timer in timerText)
            {
                timer.color = Color.yellow;
            }   
            foreach (TextMeshProUGUI t in StartStopButtonsText)
            {
                t.text = "START";
            }
            if (standardAcquisition)
            {
                delayField[0].interactable = true;
            }
            else
            {
                delayField[1].interactable = true;
            }
            StopCoroutine(_delayedStart);
            foreach(TextMeshProUGUI c in countdown)
            {
                c.gameObject.SetActive(false);
            }
        }
        
        IEnumerator DelayedRecording(int delay, bool standardAcquisition)
        {
            int d = delay;
            if (d > 0)
            {
                while (d >= 0)
                {
                    foreach (TextMeshProUGUI c in countdown)
                    {
                        c.text = d.ToString();
                        c.gameObject.SetActive(true);
                    }
                    
                    d--;
                    yield return new WaitForSeconds(1);
                }
            }

            foreach (TextMeshProUGUI c in countdown)
            {
                c.gameObject.SetActive(false);
            }
            RecordPlaying = true;
            //call the controller
            AcquisitionNotification.StartStopTimer();
            AcquisitionNotification.Record(true, standardAcquisition);
        }

        public void SelectDevice()
        {
            this.titleConfigPanel.text = "Select Device";
            List<DevicesInfos.Device> listDevices = app.model.GetDeviceList();
            foreach (DevicesInfos.Device device in listDevices)
            {
                UserButton newButton = Instantiate(userButton, ConfigContent.transform, false);
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
            CreateNewDBButton.gameObject.SetActive(true);
            this.titleConfigPanel.text = "Select DataBase";
            List<DataBase> listDb = app.model.GetDataBases();
            string modelDevice = app.model.GetDevice().ToString();
            Regex r = new Regex(@"^\w+");
            foreach (DataBase db in listDb)
            {
                if (r.Match(db.DeviceType).ToString() == modelDevice)
                {
                    UserButton newButton = Instantiate(userButton, ConfigContent.transform, false);
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
            CreateOrModifyUsersButton.gameObject.SetActive(false);
            CreateNewDBButton.gameObject.SetActive(false);
        }

        public void DisplayDataClasses()
        {
            foreach (Transform child in GestureContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            Dictionary<string, Func<List<GestureData>>> dic = app.model.GetDataClasses();
            foreach (string s in dic.Keys)
            {
                if (dic[s]().Count > 0)
                {
                    GestureData g = dic[s]()[0];
                    GestureButton newButton = Instantiate(buttonGesture, GestureContent.transform, false);
                    newButton.gameObject.name = g.Classe;
                    newButton.text.text = g.Classe;
                    newButton.playButton.onClick.AddListener(() =>
                    {
                        AcquisitionNotification.PlayFileReader(g);
                    });
                }
                else
                {
                    GestureButton newButton = Instantiate(buttonGesture, GestureContent.transform, false);
                    newButton.gameObject.name = s;
                    newButton.text.text = s;
                    ColorBlock colors = newButton.playButton.colors;
                    colors.normalColor = Color.red;
                    colors.highlightedColor = Color.red;
                    colors.pressedColor = Color.red;
                    newButton.playButton.colors = colors;
                }
            }
        }

        public void DisplayDataClassesName()
        {
            foreach (Transform child in ClassContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            Dictionary<string, Func<List<GestureData>>> dic = app.model.GetDataClasses();
            foreach (string s in dic.Keys)
            {
                if (dic[s]().Count > 0)
                {
                    GestureData g = dic[s]()[0];
                    GestureButton newButton = Instantiate(buttonGesture, ClassContent.transform, false);
                    newButton.gameObject.name = g.Classe;
                    newButton.text.text = g.Classe;
                    newButton.playButton.onClick.AddListener(() =>
                    {
                        AcquisitionNotification.SetClassName(g.Classe);
                    });
                }
                else
                {
                    GestureButton newButton = Instantiate(buttonGesture, ClassContent.transform, false);
                    newButton.gameObject.name = s;
                    newButton.text.text = s;
                    ColorBlock colors = newButton.playButton.colors;
                    colors.normalColor = Color.red;
                    colors.highlightedColor = Color.red;
                    colors.pressedColor = Color.red;
                    newButton.playButton.colors = colors;
                    newButton.playButton.onClick.AddListener(() =>
                    {
                        AcquisitionNotification.SetClassName(s);
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
            list.Reverse();
            foreach(GestureData g in list)
            {
                GestureButton newButton = Instantiate(buttonGesture, GestureContent.transform, false);
                newButton.gameObject.name = g.DataName;
                newButton.text.text = g.DataName;
                newButton.playButton.onClick.AddListener(() =>
                {
                    Delete.interactable = true;
                    AcquisitionNotification.OnSelectedGesture(g);
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

        public void ActiveDirectivesPanel()
        {
            AcquisitionNotification.ActiveDirectivesPanel();
        }

        public void OnRandomClasses()
        {
            AcquisitionNotification.OnRandomClasses();
        }

        public void ValidateGesturesToDo()
        {
            AcquisitionNotification.ValidateGesturesToDo();
        }

        public void CleanFileReader()
        {
            AcquisitionNotification.CleanFileReader();
        }

        public void HideFileReader()
        {
            app.view.ReplayRenderAcquisition.SetActive(false);
        }
        public void ChangeToSceneUsersManagement()
        {
            InformationContainer.INCOMMING_SCENE = "Acquisition";
            SceneManager.LoadScene ("UsersManagement");
        }

        public void ChangeToSceneDBManagement()
        {
            InformationContainer.INCOMMING_SCENE = "Acquisition";
            SceneManager.LoadScene(("DBManagement"));
        }
    }
}