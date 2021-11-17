using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Recognizer;
using Recognizer.DataTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Menus.UntrimmedVisualizer.View
{
    public class UntrimmedVisualizerView : UntrimmedVisualizerElement
    {
        private List<string> _gests = new List<string>();
        public Button gestureButton;
        public RectTransform scrollView;
        public Button replay;
        private Button _oldSelectedGesture = null;
        public GameObject gestureContainer;
        public GameObject imageContainer;
        public Image loading;
        private int _imageCounter;
        private Button _oldSelectedImage;
        public RectTransform PrefabCuDi;
        public RectTransform PrefabCuDi2;
        public RectTransform PrefabLabelText;
        public RectTransform PrefabClassifText;
        public RectTransform LecturesBarsAndLabels;
        public RectTransform LabelBar;
        public RectTransform LabelPredBar;
        public RectTransform SplitCudiBar;
        public TMP_InputField inputFiledTrail;
        public TMP_InputField inputSampled;
        public TMP_InputField inputSampledPred;
        public TMP_InputField fpsVisualisation;

        public RawImage fileReaderRawImage;
        public AnimationFileReader fileReader;
        public RawImage fileReaderNormRawImage;
        public AnimationFileReader fileReaderForNormalized;
        public Image lectureProgressBar;
        private int _nbFrame;
        private List<RectTransform> currentCuDi = new List<RectTransform>();
        private List<RectTransform> currentLabelsText = new List<RectTransform>();
        private List<int> _predictionsRegress;
        private List<RectTransform> _currentPrediction = new List<RectTransform>();
        private List<string> _predictionClassif;
        private List<RectTransform> _currentPredictionClassAll = new List<RectTransform>();

        public TextMeshProUGUI classifPrediction;
        private float _oldFrequency = -1f;

        public GameObject AnchorBoxTimes;


        public GameObject AnchorBox;
        public GameObject WhiteCube;
        public Material BlackCube;
        public Material WhiteCubeMat;
        public float CubeSize = 1f;
        public float CubeSpace = 0.5f;
        public float CubeSpaceX = 0.5f;
        private List<List<List<Renderer>>> _theBox;
        private bool _showVoxel;
        private List<Renderer> _theBlacks = new List<Renderer>();
        private GestureData _currentData;

        public List<Material> materialsForValues;
        private static readonly int Color = Shader.PropertyToID("_Color");


        public VideoPlayer VideoPlayer;

        public void InitView(Dictionary<string, Func<List<GestureData>>> classes, string selectedGestureButton)
        {
            HideGestures();
            int counter = 0;
            foreach (KeyValuePair<string, Func<List<GestureData>>> gestAction in classes)
            {
                counter++;

                AddClassNameWithListener(gestAction.Key);
            }

            _oldFrequency = RecoManager.FREQUENCY_RECORD_SEND_DATA;
            fpsVisualisation.text = "" + ((int) (1 / RecoManager.FREQUENCY_RECORD_SEND_DATA));

            scrollView.GetComponent<RectTransform>().sizeDelta =
                new Vector2(scrollView.GetComponent<RectTransform>().sizeDelta.x, 40 * counter);

            replay.interactable = false;
            replay.onClick.AddListener(() => UntrimmedVisualizerNotification.ReplayClicked());
        }

        private void AddClassNameWithListener(string gest, bool select = false)
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
            UntrimmedVisualizerNotification.SequenceSelected(button.name);
        }

        public void DisplayImageOfClass(string classe, bool withCounter) // A utiliser quand on clique sur le class text
        {
            object[] parms = new object[2] {classe, withCounter};
            StopCoroutine("DisplayImageOfClassCoroutine");
            StartCoroutine("DisplayImageOfClassCoroutine", parms);
        }

        private IEnumerator DisplayImageOfClassCoroutine(object[] parms)
        {
            loading.gameObject.SetActive(true);
            string classe = (string) parms[0];
            bool withCounter = (bool) parms[1];

            HideImages();

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
                loading.gameObject.SetActive(false);
                yield return null;
            }

            if (withCounter)
                imageContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    imageContainer.GetComponent<RectTransform>().sizeDelta.x,
                    (_imageCounter / 3 - 1) * imageContainer.GetComponent<GridLayoutGroup>().cellSize.y);
            loading.gameObject.SetActive(false);
        }

        private void HideGestures()
        {
            if (gestureContainer.GetComponentsInChildren<RectTransform>().Length != 0)
            {
                foreach (RectTransform gesture in gestureContainer.transform)
                    Destroy(gesture.gameObject);
            }
        }

        private void HideImages()
        {
            if (imageContainer.GetComponentsInChildren<RectTransform>().Length != 0)
            {
                foreach (RectTransform image in imageContainer.transform)
                    Destroy(image.gameObject);
            }
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

        public void SelectData(GestureData data, Button button)
        {
            UntrimmedVisualizerNotification.DataSelected(data);
            replay.interactable = true;

            ColorBlock colorVal = button.colors;
            colorVal.normalColor = new Color(1f, 1f, 1f, 1f);
            if (_oldSelectedImage)
                _oldSelectedImage.colors = colorVal;
            colorVal.normalColor = new Color(1f, 0.5f, 0.5f, 1f);
            button.colors = colorVal;
            _oldSelectedImage = button;
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

            Action<int> informer = i => InformerOnUpdate(i);
            fileReader.SetEventInformProgression(informer);
            if (data is LabeledUntrimedGestureData) //should be
            {
                LabeledUntrimedGestureData d = (LabeledUntrimedGestureData) data;
                if (d.PathNormalizedData != null)
                {
                    fileReaderNormRawImage.gameObject.SetActive(true);
                    fileReaderForNormalized.LoadNewGeste(d.PathNormalizedData);
                    fileReaderForNormalized.Play();
                }

                if (d.PathVideoRGB != null)
                {
                    VideoPlayer.url = "File://" + d.PathVideoRGB;
                    VideoPlayer.Play();
                    VideoPlayer.playbackSpeed = int.Parse(inputSampled.text) *
                                                (int.Parse(fpsVisualisation.text) / VideoPlayer.frameRate);
                }
            }

            Invoke(nameof(UpdateValueSample), 0.6f);
            UpdateValueTrail();
            UpdateFPSSpeed();
            _nbFrame = fileReader.Play();
            _currentData = data;
            DrawBlocsLabelsAndSetInformations(data);
            DrawBlocsCuDi(data);
            DrawPredictionClassesForAllSequence();
        }


        public void Voxelize(GestureData data, int frame)
        {
            if (data is LabeledUntrimedGestureData) //should be
            {
                LabeledUntrimedGestureData d = (LabeledUntrimedGestureData) data;
                List<List<List<List<float>>>> boxes = d.ExtractVoxel();
                if (boxes == null)
                    return;
                int cpt = 0;

                List<List<List<float>>> theBox = boxes[frame];
                DrawTheBox(theBox.Count, theBox[0].Count, theBox[0][0].Count);

                for (int i = 0; i < theBox.Count; i++)
                for (int j = 0; j < theBox[i].Count; j++)
                for (int k = 0; k < theBox[i][j].Count; k++)
                    if (theBox[i][j][k] >= 0.01)
                    {
                        ColorVoxel(i, j, k, _theBox, true, theBox[i][j][k]);
                    }
            }
        }

        public void DrawVoxelsInTime()
        {
            LabeledUntrimedGestureData d = (LabeledUntrimedGestureData) app.model.selectedData;
            List<List<List<List<float>>>> boxes = d.ExtractVoxel();


            int NB_FRAM = 60;
            List<List<List<Renderer>>> theBoxToColor = GetWhiteBox(NB_FRAM, boxes[0].Count, boxes[0][0].Count,
                AnchorBoxTimes.transform);


            for (int i = 0; i < NB_FRAM; i++)
            {
                List<List<List<float>>> theBox = boxes[i];
                for (var j = 0; j < theBox.Count; j++)
                {
                    List<List<float>> xList = theBox[j];
                    for (var k = 0; k < xList.Count; k++)
                    {
                        List<float> yList = xList[k];
                        float som = yList.Aggregate((i1, i2) => i1 + i2);
                        if (som > 0)
                        {
                            ColorVoxel(i, 15 - k, j, theBoxToColor, false);
                        }
                    }
                }
            }
        }

        private void ColorVoxel(int x, int y, int z, List<List<List<Renderer>>> theBox, bool addToBlack = true,
            float value = 1f)
        {
            Renderer cube = theBox[x][y][z];
            if (value >= 1 || value < 0.001)
                cube.material = materialsForValues[(int) value];
            else
            {
                cube.material = materialsForValues[1];
                cube.material.SetColor(Color, new Color(1 - value, 1 - value, 1 - value));
            }

            if (addToBlack)
                _theBlacks.Add(cube);
        }

        private void DrawTheBox(int sizeX, int sizeY, int sizeZ)
        {
            if (_theBox != null &&
                (_theBox.Count == sizeX && _theBox[0].Count == sizeY && _theBox[0][0].Count == sizeZ))
            {
                foreach (Renderer blak in _theBlacks)
                    blak.material = WhiteCubeMat;
                _theBlacks.Clear();
                return;
            }

            if (_theBox != null)
            {
                foreach (List<List<Renderer>> list in _theBox)
                {
                    foreach (List<Renderer> renderers in list)
                    {
                        foreach (Renderer renderer1 in renderers)
                        {
                            Destroy(renderer1.gameObject);
                        }
                    }
                }

                _theBox.Clear();
                foreach (Renderer blak in _theBlacks)
                    blak.material = WhiteCubeMat;
                _theBlacks.Clear();
            }

            _theBox = GetWhiteBox(sizeX, sizeY, sizeZ, AnchorBox.transform);
        }

        private List<List<List<Renderer>>> GetWhiteBox(int sizeX, int sizeY, int sizeZ, Transform anchor)
        {
            List<List<List<Renderer>>> theBox = new List<List<List<Renderer>>>();
            for (int i = 0; i < sizeX; i++)
            {
                List<List<Renderer>> boxI = new List<List<Renderer>>();
                for (int j = 0; j < sizeY; j++)
                {
                    List<Renderer> boxJ = new List<Renderer>();
                    for (int k = 0; k < sizeZ; k++)
                    {
                        GameObject cube = Instantiate(WhiteCube, anchor);
                        cube.transform.localPosition = Vector3.zero;
                        cube.transform.localPosition += i * (CubeSize / 2f + CubeSpaceX) * anchor.right;
                        cube.transform.localPosition += j * (CubeSize / 2f + CubeSpace) * (anchor.up);
                        cube.transform.localPosition +=
                            k * (CubeSize / 2f + CubeSpace) * (anchor.forward);
                        boxJ.Add(cube.GetComponentInChildren<Renderer>());
                    }

                    boxI.Add(boxJ);
                }

                theBox.Add(boxI);
            }

            return theBox;
        }


        private void DrawBlocsLabelsAndSetInformations(GestureData data)
        {
            if (data is LabeledUntrimedGestureData) //should be
            {
                LabeledUntrimedGestureData d = (LabeledUntrimedGestureData) data;
                List<Label> labels = d.ExtractLabel();

                _predictionsRegress = d.ExtractRegress();
                _predictionClassif = d.ExtractClassif();

                foreach (RectTransform o in currentLabelsText)
                {
                    Destroy(o.gameObject);
                }

                currentLabelsText.Clear();

                foreach (Label label in labels)
                {
                    int frameLength = label.EndFrame - label.BeginFrame;
                    float rectWidth = LabelBar.rect.width;
                    float widthLabel = frameLength * rectWidth / _nbFrame;
                    float beginLen = label.BeginFrame * rectWidth / _nbFrame;
                    RectTransform newLabel = Instantiate(PrefabLabelText, LabelBar);
                    newLabel.sizeDelta = new Vector2(widthLabel, newLabel.rect.height);
                    newLabel.anchoredPosition = new Vector3(beginLen, 0, 0);

                    TextMeshProUGUI textt = newLabel.GetComponentInChildren<TextMeshProUGUI>();
                    textt.text = label.NameLabel;
                    RawImage img = newLabel.GetComponentInChildren<RawImage>();
                    int id = label.NumberLabel % materialsForValues.Count;
                    id = id == 0 ? 1 : id;
                    img.color = materialsForValues[id].color;
                    currentLabelsText.Add(newLabel);
                    if (label.ActionPoint != -1)
                    {
                        frameLength = 2;
                        rectWidth = LabelBar.rect.width;
                        widthLabel = frameLength * rectWidth / _nbFrame;
                        beginLen = label.ActionPoint * rectWidth / _nbFrame;
                        newLabel = Instantiate(PrefabLabelText, LabelBar);
                        newLabel.sizeDelta = new Vector2(widthLabel, newLabel.rect.height);
                        newLabel.anchoredPosition = new Vector3(beginLen, 3, 0);
                        //
                        textt = newLabel.GetComponentInChildren<TextMeshProUGUI>();
                        textt.text = "PA";
                        img = newLabel.GetComponentInChildren<RawImage>();
                        img.color = UnityEngine.Color.white;
                        currentLabelsText.Add(newLabel);
                    }
                    
                }
            }
            else
            {
                Debug.Log("Warning: WRONG TYPE OF DATA PASSED (should be LabeledUntrimedGestureData)");
            }
        }


        private void DrawBlocsCuDi(GestureData data)
        {
            if (data is LabeledUntrimedGestureData) //should be
            {
                LabeledUntrimedGestureData d = (LabeledUntrimedGestureData) data;
                List<int> labels = d.ExtractSplitCuDi();
                if (labels == null)
                {
                    print("No split CuDi for this file");
                    return;
                }

                foreach (RectTransform o in currentCuDi)
                {
                    Destroy(o.gameObject);
                }

                currentCuDi.Clear();
                int cpt = 0;
                int begin = 0;
                foreach (int repet in labels)
                {
                    int frameLength = repet * int.Parse(inputSampled.text);
                    float rectWidth = SplitCudiBar.rect.width;
                    float widthLabel = frameLength * rectWidth / _nbFrame;
                    float beginLen = begin * rectWidth / _nbFrame;
                    RectTransform newsplit = Instantiate(cpt % 2 == 0 ? PrefabCuDi : PrefabCuDi2, SplitCudiBar);
                    newsplit.sizeDelta = new Vector2(widthLabel, newsplit.rect.height);
                    newsplit.anchoredPosition = new Vector3(beginLen, 0, 0);

                    currentCuDi.Add(newsplit);
                    begin += frameLength;
                    cpt += 1;
                }
            }
            else
            {
                Debug.Log("Warning: WRONG TYPE OF DATA PASSED (should be LabeledUntrimedGestureData)");
            }
        }

        private void Update()
        {
            if (app.model.pauseToggle)
            {
                app.model.pauseToggle = false;
                if (fileReader.isPause())
                {
                    fileReader.Play();
                    fileReaderForNormalized.Play();
                    VideoPlayer.Play();
                    UpdateValueTrail();
                }
                else
                {
                    fileReader.Pause();
                    fileReaderForNormalized.Pause();
                    VideoPlayer.Pause();
                    UpdateValueTrailFromTime(9999);
                }
            }
        }

        public void InformerOnUpdate(int currentIndex)
        {
            lectureProgressBar.fillAmount = ((float) currentIndex) / _nbFrame;
            lectureProgressBar.GetComponentInChildren<TextMeshProUGUI>().text = currentIndex+"";

            if (_predictionsRegress != null && _predictionsRegress.Count > 0)
            {
                DrawPredictionWindow(currentIndex);
                DrawClassifPrediction(currentIndex);
                VideoPlayer.frame = currentIndex;
            }

            if (_showVoxel)
            {
                Voxelize(_currentData, currentIndex);
            }
        }

        private void DrawPredictionWindow(int currentIndex)
        {
            try
            {
                int prediction = _predictionsRegress[currentIndex / int.Parse(inputSampledPred.text)];
                foreach (RectTransform o in _currentPrediction)
                {
                    Destroy(o.gameObject);
                }

                _currentPrediction.Clear();
                int frameLength = prediction;
                float rectWidth = LecturesBarsAndLabels.rect.width;
                float widthLabel = frameLength * rectWidth / _nbFrame;
                float beginLen = currentIndex * rectWidth / _nbFrame - widthLabel;
                RectTransform newLabel = Instantiate(PrefabClassifText, LecturesBarsAndLabels);
                newLabel.sizeDelta = new Vector2(widthLabel, newLabel.rect.height);
                newLabel.anchoredPosition = new Vector3(beginLen, 0, 0);

                //TextMeshProUGUI textt = newLabel.GetComponentInChildren<TextMeshProUGUI>();
                //textt.text = label.NameLabel;

                _currentPrediction.Add(newLabel);
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log("WARNING : NO regression prediction for index " + currentIndex);
            }
        }

        private void DrawPredictionClassesForAllSequence()
        {
            if(_predictionClassif==null)
                return;
                
            try
            {
                foreach (RectTransform o in _currentPredictionClassAll)
                {
                    Destroy(o.gameObject);
                }
                _currentPredictionClassAll.Clear();
                

//                int beginIndex = 0;
                
//                Dictionary<int, string> mappingIdClassName =
//                    ((LabeledUntrimedGestureData) _currentData).MappingIdClassName;
                
//                int pred = mappingIdClassName.First(x => x.Value == _predictionClassif[0]).Key;
//                
//                List<Label> labels = new List<Label>();
//
//                for (var index = 1; index < _predictionClassif.Count; index++)
//                {
//                    string classe = _predictionClassif[index];
//
//                    int predictionID = mappingIdClassName.First(x => x.Value == classe).Key;
//            
//                    if (predictionID != pred || index==_predictionClassif.Count-1)
//                    {
//                        Label l = new Label(beginIndex,(index==_predictionClassif.Count-1?index:index-1),predictionID,classe);
//                        labels.Add(l);
//                        pred = predictionID;
//                        beginIndex = index;
//                    }
//                }

                int sample = int.Parse(inputSampledPred.text);
                int begin = 0;
                for (int i = 1; i < _nbFrame; i++)
                {
                    if (_predictionClassif[(i - 1) / sample] != _predictionClassif[i /sample] || i == _nbFrame-1)
                    {
                        
                        int id = ((LabeledUntrimedGestureData) _currentData).MappingIdClassName.First(x => x.Value == _predictionClassif[(i-1)/sample]).Key;
                        if (id != 0)
                        {
                            int frameLength = (i == _nbFrame-1?i:(i-1))- begin +1;
                            float rectWidth = LabelPredBar.rect.width;
                            float widthLabel = frameLength * rectWidth / _nbFrame;
                            float beginLen = begin * rectWidth / _nbFrame;
                            RectTransform newLabel = Instantiate(PrefabLabelText, LabelPredBar);
                            newLabel.sizeDelta = new Vector2(widthLabel, newLabel.rect.height);
                            newLabel.anchoredPosition = new Vector3(beginLen, 0, 0);
                        

                            TextMeshProUGUI textt = newLabel.GetComponentInChildren<TextMeshProUGUI>();
                            textt.text = _predictionClassif[(i-1)/sample];
                            RawImage img = newLabel.GetComponentInChildren<RawImage>();
                            int id2 = id % materialsForValues.Count;
                            id2 = id2 == 0 ? 1 : id2;
                            img.color = materialsForValues[id2].color;

                            _currentPredictionClassAll.Add(newLabel);
                        }
                             
                        begin = i;
                    }
                        
                }


            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log("WARNING : NO class prediction for this sequence");
            }
        }

        public IEnumerator Test(List<Label> labels)
        {
            int sample = int.Parse(inputSampledPred.text);
            foreach (Label label in labels)
            {
                if(label.NumberLabel==0)
                    continue;
                int frameLength = label.EndFrame*sample - label.BeginFrame*sample;
                float rectWidth = LabelPredBar.rect.width;
                float widthLabel = frameLength * rectWidth / _nbFrame;
                float beginLen = label.BeginFrame*sample * rectWidth / _nbFrame;
                RectTransform newLabel = Instantiate(PrefabLabelText, LabelPredBar);
                newLabel.sizeDelta = new Vector2(widthLabel, newLabel.rect.height);
                newLabel.anchoredPosition = new Vector3(beginLen, 0, 0);

                TextMeshProUGUI textt = newLabel.GetComponentInChildren<TextMeshProUGUI>();
                textt.text = label.NameLabel;
                RawImage img = newLabel.GetComponentInChildren<RawImage>();
                img.color = materialsForValues[label.NumberLabel % materialsForValues.Count].color;

                _currentPredictionClassAll.Add(newLabel);
                yield return new WaitForSeconds(1);
            }
        }

        private void DrawClassifPrediction(int currentIndex)
        {
            try
            {
                classifPrediction.text = _predictionClassif[currentIndex / int.Parse(inputSampledPred.text)];
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log("WARNING : NO classif prediction for index " + currentIndex);
            }
        }

        public void UpdatePosLecture(float f)
        {
            if (_nbFrame != 0)
            {
                int indexToGo = (int) (f * _nbFrame);
                fileReader.SetCurrentFrame(indexToGo);
                fileReaderForNormalized.SetCurrentFrame(indexToGo);
                VideoPlayer.frame = indexToGo;
            }
        }

        public void UpdateValueTrail()
        {
            float updated = float.Parse(inputFiledTrail.text.Replace(".", ","));
            UpdateValueTrailFromTime(updated);
        }

        private void UpdateValueTrailFromTime(float time)
        {
            foreach (TrailRenderer child in fileReader.gameObject.GetComponentsInChildren<TrailRenderer>())
            {
                child.time = time;
            }

            foreach (TrailRenderer child in fileReaderForNormalized.gameObject.GetComponentsInChildren<TrailRenderer>())
            {
                child.time = time;
            }
        }


        public void UpdateValueSample()
        {
            fileReader.SetToSkip(int.Parse(inputSampled.text));
            fileReaderForNormalized.SetToSkip(int.Parse(inputSampled.text));
            VideoPlayer.playbackSpeed = int.Parse(inputSampled.text) *
                                        (int.Parse(fpsVisualisation.text) / VideoPlayer.frameRate);
            DrawBlocsCuDi(_currentData);
            DrawPredictionClassesForAllSequence();
            

        }

        public void UpdateFPSSpeed()
        {
            RecoManager.FREQUENCY_RECORD_SEND_DATA = 1f / int.Parse(fpsVisualisation.text);
            VideoPlayer.playbackSpeed = int.Parse(inputSampled.text) *
                                        (int.Parse(fpsVisualisation.text) / VideoPlayer.frameRate);
        }

        private void OnDisable()
        {
            if (_oldFrequency < 0f)
                RecoManager.FREQUENCY_RECORD_SEND_DATA = _oldFrequency;
        }

        public void ShowVoxels(bool activated)
        {
            _showVoxel = activated;
        }
    }
}