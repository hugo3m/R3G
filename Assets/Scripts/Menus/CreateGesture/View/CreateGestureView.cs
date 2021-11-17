using System;
using System.Collections;
using System.Collections.Generic;
using Recognizer;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using TMPro;
using UnityEngine.SceneManagement;

namespace Menus.CreateGesture.View
{
    /// <summary>
    /// Lance les events :
    /// -ClassSelected(class)
    /// -DataSelected(data)
    /// -DeleteData()
    /// -Replay()
    /// -GestureRecognized(enter) (click sur un bouton "start")
    /// </summary>
    public class CreateGestureView : CreateGestureElement
    {
        public Canvas Canvas;
        public RectTransform gestureTxtFile;
        public RectTransform scrollView;
        public GameObject imageContainer;
        public GameObject gestureContainer;
        public AnimationFileReader fileReader;
        public RawImage fileReaderRawImage;
        public Button gestureButton;
        public Button replay;
        public Button delete;
        public TextMeshProUGUI startStopButtonText;
        public TMP_InputField classNameField;
        public TMP_InputField tagField;
        public TMP_InputField delayField;
        public TextMeshProUGUI countdown;
        public Font gestureFont;
        public Image Loading;

        private Button _oldSelectedGesture = null;
        private Button _oldSelectedImage;
        private IEnumerator _delayedStart;
        public bool RecordPlaying;
        private string _classeSelected;
        float positionCounter;
        private int _imageCounter;

        private List<string> _gests = new List<string>();

        /// <summary>
        /// Affiche la liste des classes disponible, initialise les listeners
        /// </summary>
        /// <param name="classes">la liste des données pour chaque classe</param>
        public void InitView(Dictionary<string, Func<List<GestureData>>> classes, string selectedGestureButton)
        {
            HideGestures();
            int counter = 0;
            positionCounter = 0f;
            foreach (KeyValuePair<string, Func<List<GestureData>>> gestAction in classes)
            {
                counter++;

                AddClassNameWithListener(gestAction.Key, positionCounter);
                positionCounter -= 30;
            }

            scrollView.GetComponent<RectTransform>().sizeDelta =
                new Vector2(scrollView.GetComponent<RectTransform>().sizeDelta.x, 40 * counter);

            replay.interactable = false;
            delete.interactable = false;

            replay.onClick.AddListener(() => CreateGestureNotification.ReplayClicked());
            delete.onClick.AddListener(() => CreateGestureNotification.DeleteData());

            startStopButtonText.text = "Start";
            delayField.interactable = true;
        }

        private void AddClassNameWithListener(string gest, float positionCounter, bool select = false)
        {
            _gests.Add(gest);
            Button gesture = Instantiate(gestureButton, scrollView.transform, false);
            /* ColorBlock colorVal = gesture.colors;
             colorVal.highlightedColor = new Color(1f, 0.5f, 0.5f, 1f);
             gesture.colors = colorVal;
             gesture.GetComponentInChildren<Text>().font = gestureFont;
             //gesture.GetComponent<RectTransform>().anchoredPosition = new Vector2(scrollView.anchoredPosition.x, positionCounter);*/
            gesture.name = gest;
            Text text = gesture.GetComponentInChildren<Text>();
            text.text = gest;
            gesture.onClick.AddListener(() => GestureButtonSelect(gesture));
            gesture.onClick.AddListener(() => DisplayImageOfClass(gesture.name, true));

            if (select)
                GestureButtonSelect(gesture);
        }

        /// <summary>
        /// Utile pour voir le geste choisi
        /// </summary>
        /// <param name="button">bouton du geste choisi</param>
        private void GestureButtonSelect(Button button)
        {
            ColorBlock colorVal = button.colors;
            colorVal.normalColor = new Color(1f, 1f, 1f, 0f);
            if (_oldSelectedGesture)
                _oldSelectedGesture.colors = colorVal;
            colorVal.normalColor = new Color(1f, 0.5f, 0.5f, 1f);
            button.colors = colorVal;
            _oldSelectedGesture = button;
            CreateGestureNotification.ClassSelected(button.name);
            classNameField.text = app.model.GetSelectedClass();
        }

        public void DisplayImageOfClass(string classe, bool withCounter) // A utiliser quand on clique sur le class text
        {
            object[] parms = new object[2] {classe, withCounter};
            StopCoroutine("DisplayImageOfClassCoroutine");
            StartCoroutine("DisplayImageOfClassCoroutine", parms);
        }

        private IEnumerator DisplayImageOfClassCoroutine(object[] parms)
        {
            Loading.gameObject.SetActive(true);
            string classe = (string) parms[0];
            bool withCounter = (bool) parms[1];

            HideImages();

            _classeSelected = classe;
            _imageCounter = 0;


            if (!app.model.Pictures.ContainsKey(classe))
            {
                app.model.Pictures.Add(classe, new List<Tuple<GestureData, Texture2D>>());

                if (!app.model.AllClassesAndData.ContainsKey(classe))
                {
                    app.model.AllClassesAndData.Add(classe, app.model.AllClassesAndDataLazy[classe]());
                }

                foreach (GestureData data in app.model.AllClassesAndData[classe])
                {
                    app.model.Pictures[classe].Add(new Tuple<GestureData, Texture2D>(data,
                        DataManager.GetInstance().GetImageFromDataGesture(data)));
                    yield return null;
                }
            }

            foreach (Tuple<GestureData, Texture2D> texture2D in app.model.Pictures[classe])
            {
                AddImageWithListeners(texture2D);
                _imageCounter++;
                Loading.gameObject.SetActive(false);
                yield return null;
            }

            if (withCounter)
                imageContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    imageContainer.GetComponent<RectTransform>().sizeDelta.x,
                    (_imageCounter / 3 - 1) * imageContainer.GetComponent<GridLayoutGroup>().cellSize.y);
            Loading.gameObject.SetActive(false);
        }

        public void AddImageWithListeners(Tuple<GestureData, Texture2D> texture2D)
        {
            GameObject image = new GameObject("image");
            image.transform.parent = imageContainer.transform;
            image.transform.localScale = new Vector3(1, 1, 1);
            RawImage rawImage = image.AddComponent<RawImage>();
            Button button = image.AddComponent<Button>();

            ColorBlock colorVal = button.colors;
            colorVal.highlightedColor = new Color(1f, 0.5f, 0.5f);
            button.colors = colorVal;
            button.onClick.AddListener(() => SelectData(texture2D.Item1, button));
            button.onClick.AddListener(() => button.Select());
            rawImage.texture = texture2D.Item2;
            texture2D.Item2.wrapMode = TextureWrapMode.Clamp;
            rawImage.uvRect = new Rect(0.3f, 0.3f, 0.4f, 0.4f);
        }

        public void AddSingleImageOfClass(string classe, Tuple<GestureData, Texture2D> dataTexture)
        {
            if (_classeSelected == classe)
            {
                AddImageWithListeners(dataTexture);
            }
            else
            {
                if (!_gests.Contains(classe))
                {
                    AddClassNameWithListener(classe, positionCounter, true);
                }

                DisplayImageOfClass(classe, true);
            }

            _imageCounter++;

            if (_imageCounter % 3 == 1)
                imageContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    imageContainer.GetComponent<RectTransform>().sizeDelta.x,
                    imageContainer.GetComponent<RectTransform>().sizeDelta.y +
                    imageContainer.GetComponent<GridLayoutGroup>().cellSize.y);
        }

        private void HideImages()
        {
            if (imageContainer.GetComponentsInChildren<RectTransform>().Length != 0)
            {
                foreach (RectTransform image in imageContainer.transform)
                    Destroy(image.gameObject);
            }
        }

        private void HideGestures()
        {
            if (gestureContainer.GetComponentsInChildren<RectTransform>().Length != 0)
            {
                foreach (RectTransform gesture in gestureContainer.transform)
                    Destroy(gesture.gameObject);
            }
        }

        public string GetClassName()
        {
            return classNameField.text;
        }

        public string GetTag()
        {
            return tagField.text;
        }

        public void SelectData(GestureData data, Button button)
        {
            CreateGestureNotification.DataSelected(data);
            replay.interactable = true;
            delete.interactable = true;

            ColorBlock colorVal = button.colors;
            colorVal.normalColor = new Color(1f, 1f, 1f, 1f);
            if (_oldSelectedImage)
                _oldSelectedImage.colors = colorVal;
            colorVal.normalColor = new Color(1f, 0.5f, 0.5f, 1f);
            button.colors = colorVal;
            _oldSelectedImage = button;
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
            startStopButtonText.text = "Stop";
            delayField.interactable = false;
            _delayedStart = DelayedRecording(int.Parse(delayField.text));
            StartCoroutine(_delayedStart);
        }

        IEnumerator DelayedRecording(int delay)
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
            CreateGestureNotification.Record(true);
        }

        private void StopRecording()
        {
            CreateGestureNotification.Record(false);
            RecordPlaying = false;
            startStopButtonText.text = "Start";
            delayField.interactable = true;
            StopCoroutine(_delayedStart);
            countdown.gameObject.SetActive(false);
        }

        /// <summary>
        /// Joue une donnée dans une grande fenêtre
        /// Est appellée en cas de Replay
        /// </summary>
        /// <param name="data"> la données à jouer</param>
        public void PlayFile(GestureData data)
        {
            //Debug.Log(data.DataName);
            //fileReader.filename = ".\\RecognitionServer\\AppRecognizer\\RAW_LeapMotionDataFolder\\" + data.DataName;
            fileReaderRawImage.gameObject.SetActive(true);
            fileReader.LoadNewGeste(data.Path);
            fileReader.Play();
        }
        
    }
}