using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Recognizer;
using Recognizer.StrategyFusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Menus.TestAndSetting.View
{
    /// <summary>
    /// lance les events :
    /// ThetaModification
    /// CurvilineDistanceModification
    /// HandJointsModification
    /// </summary>
    public class TestAndSettingView : TestAndSettingElement
    {

        public HistogramView histogramPrefab;
        public GameObject linePrefab;
        public GameObject histogramsContainer;
        public TextMeshProUGUI classRecoLabel;
        public InputField NoiseValue;
        public InputField CurvilineValue;
        public Image RecoStatus;
        public TextMeshProUGUI StrategyNameText;

        public List<Toggle> OrderedJointsType;
        public Toggle SeuilsToggle;
        public Toggle PauseToggle;
        public GameObject PanelSelectionJointure;
        public GameObject SeuilsParent;

        [NonSerialized]
        public bool Pause;

        private List<HistogramView> histograms;
        private static float SPACE_BETWEEN_HISTO=3;
        
        
        private static float TIME_REFRESHING_HISTO=0.1f;
        private static float TIME_HIGHLIGH_RECO_CLASS = 0.7f;
        private float _nextRefresh = 0;
        private float _timeToBackNormalColor = 0;
        private string _lastReco="-1";
        private bool backNormal = true;
        private GameObject _lineTheta;
        private GameObject _linePsi;
        private RecoManager.StatusReco currentStatus = RecoManager.StatusReco.NotWorkingAndTrying;
        private StrategyRecognizer _strategy;
        private bool _lastStatusSeuil;

        /// <summary>
        /// Initialise la vue :
        /// Soit N le nombre de classe
        /// N histogrammes (un par classifeur "spécialiste") qui contient N batons (un par classe)
        /// On peut filtrer les classifieurs dont on veut voir l'histogramme
        /// Les seuils de detection ne sont modifiables que si le reconnaisseur
        /// utilise la stratégie par défaut (isDefaultStrategy).
        /// </summary>
        /// <param name="strategy">Le nom de la stratégie de fusion</param>
        /// <param name="theta">Phi : le noise tolerance</param>
        /// <param name="curvi">Le pourventage de distance curviligne</param>
        /// <param name="jointsStatus">Le status des différentes jointures de doigts</param>
        public void InitView(StrategyRecognizer strategy,string theta, float curvi, List<Tuple<LeapMotionJointType,bool>> jointsStatus)
        {
            this._strategy = strategy;
            if (!(_strategy is ISpecificPsiTheta))
                SeuilsToggle.interactable = false;
            
            PanelSelectionJointure.SetActive(RecoManager.GetInstance().Device==DevicesInfos.Device.LeapMotion);
            _lastStatusSeuil = SeuilsToggle.isOn;
            
            InitHistograms();
            InitThetaPsi(theta,false);
            InitCurvi(curvi);
            InitJoints(jointsStatus);
            StrategyNameText.text = strategy.GetType().Name;
            UpdateStatus();

        }

        private void UpdateStatus()
        {
            if (currentStatus != app.model.IsRecoWorking)
            {
                currentStatus = app.model.IsRecoWorking;
                RecoStatus.color = currentStatus == RecoManager.StatusReco.Work ? Color.green : Color.red;
            }
        }


        public void ApplyNoiseTolerance()
        {
            TestAndSettingNotification.ThetaModification(NoiseValue.text);
            InitThetaPsi(NoiseValue.text,false);
        }
        
        public void ApplyCurviTolerance()
        {
            TestAndSettingNotification.CurvilineDistanceModification(float.Parse(CurvilineValue.text));            
        }
        
        public void ApplyJoints()
        {
            List<Tuple<LeapMotionJointType,bool>> jointsStatus = new List<Tuple<LeapMotionJointType, bool>>();
            for (int i = 0; i < OrderedJointsType.Count; i++)
            {
                jointsStatus.Add(new Tuple<LeapMotionJointType, bool>((LeapMotionJointType) i,OrderedJointsType[i].isOn));
            }

            TestAndSettingNotification.HandJointsModification(jointsStatus);

        }
        
        public void AskRelearn()
        {
            TestAndSettingNotification.RelearnAsk();            
        }

        private void InitCurvi(float curvi)
        {
            CurvilineValue.text = curvi.ToString(CultureInfo.InvariantCulture);
        }

        private void InitThetaPsi(string theta,  bool useStrategy)
        {
            float posTheta;
            float posPsi;
            if (useStrategy && (_strategy is ISpecificPsiTheta) )
            {
                float realTheta = (float)(_strategy as ISpecificPsiTheta).GetTheta();
                float realPsi  =(float)(_strategy as ISpecificPsiTheta).GetPsi();
                posTheta = realTheta*HistogramView.ScaleOneUnitY;
                posPsi = realPsi*HistogramView.ScaleOneUnitY;
            }
            else
            {
                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";
                try
                {
                    float thetaF =  float.Parse(theta,NumberStyles.Any,ci);  
                    posTheta = thetaF*HistogramView.ScaleOneUnitY;
                    posPsi = thetaF*2*HistogramView.ScaleOneUnitY;
                }
                catch (FormatException e)
                {
                    posTheta = 0;
                    posPsi = 0;
                    Console.WriteLine("Impossible d'afficher les seuils");
                }
                
            }
         
            if (_lineTheta == null)
            {
                _lineTheta = Instantiate(linePrefab,SeuilsParent.transform);
                RectTransform rectTransform = _lineTheta.GetComponent<RectTransform>();

                rectTransform.sizeDelta = new Vector2(histogramsContainer.GetComponent<RectTransform>().rect.width,rectTransform.sizeDelta.y);
                _linePsi = Instantiate(linePrefab,SeuilsParent.transform);
                _linePsi.GetComponent<Image>().color=Color.yellow;
                rectTransform = _linePsi.GetComponent<RectTransform>();
            
                rectTransform.sizeDelta = new Vector2(histogramsContainer.GetComponent<RectTransform>().rect.width,rectTransform.sizeDelta.y);

            }
            RectTransform rect = _lineTheta.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(0,posTheta,rect.localPosition.z);
            
            rect = _linePsi.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(0,posPsi,rect.localPosition.z);
            NoiseValue.text = theta.ToString(CultureInfo.InvariantCulture);
        }

        private void InitJoints(List<Tuple<LeapMotionJointType, bool>> jointsStatus)
        {
            for (int i = 0; i < jointsStatus.Count; i++)
            {
                if( (int)jointsStatus[i].Item1 != i )
                    throw new Exception("Incorrect order of joints status");
                OrderedJointsType[i].isOn = jointsStatus[i].Item2;
            }
        }
        
        private void InitHistograms()
        {
            List<string> classes = app.model.Classes;
            Vector2 sizeCanvas = new Vector2(histogramsContainer.GetComponent<RectTransform>().rect.width,histogramsContainer.GetComponent<RectTransform>().rect.height);
            float posX = -sizeCanvas.x/2;

            
            histograms = new List<HistogramView>();

            float sizeXOfAnHistogram = (sizeCanvas.x- SPACE_BETWEEN_HISTO * (classes.Count))/(classes.Count);
            for (var index = 0; index < classes.Count; index++)
            {
                HistogramView histo = Instantiate(histogramPrefab,histogramsContainer.transform);
                RectTransform transRec = histo.GetComponent<RectTransform>();
                histo.gameObject.transform.localScale=Vector3.one;
       
                
                List<float> init = classes.Select(_ =>0f).ToList();
                
                transRec.sizeDelta = new Vector2(sizeXOfAnHistogram,transRec.sizeDelta.y);//'y' sera set dans le histogram view
                histo.SetVals(init,classes);
                
                Vector3 po = transRec.localPosition;
                histo.ColorOne(index,Color.blue);
                
                transRec.localPosition = new Vector3(posX+(index==0?transRec.sizeDelta.x/2:0), 0, po.z);
                posX += transRec.sizeDelta.x + SPACE_BETWEEN_HISTO+(index==0?transRec.sizeDelta.x/2:0);
                histograms.Add(histo);
            }
        }


        /// <summary>
        /// Met à jour les histogrammes
        /// </summary>
        /// <param name="histogrammesVal">les valeurs des histogrammes</param>
        public void UpdateScoreHisto(Dictionary<string, Dictionary<string, double>> histogrammesVal)
        {

            try
            {
                foreach (KeyValuePair<string, Dictionary<string, double>> gest in histogrammesVal)
                {
                    int index = app.model.Classes.IndexOf(gest.Key);
                    HistogramView histo = histograms[index];
                    foreach (KeyValuePair<string,double> scoreClass in gest.Value)
                    {
                        int indexC = app.model.Classes.IndexOf(scoreClass.Key);
                        histo.SetValue(indexC,(float)scoreClass.Value);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // en cas de concurrent modification, mais pas important de louper quelques informations
            }
        }
        /// <summary>
        /// met à jour la vue en fonction de la classe reconnue
        /// </summary>
        /// <param name="classReco">la classe reconnue par le classifeur</param>
        public void UpdateGestureReco(string classReco)
        {
            if (app.model.Reco)
            {
                classRecoLabel.text = classReco;
                classRecoLabel.color = Color.red;
                _timeToBackNormalColor = Time.time + TIME_HIGHLIGH_RECO_CLASS;
                backNormal = false;
                app.model.Reco = false;

                if (PauseToggle.isOn)
                {
                    Pause = true;
                    RecoManager.GetInstance().Pause(true);
                    UpdateScoreHisto(app.model.LastHisto);
                    UpdateScoreClassifier(app.model.LastScores);
                }
                
            }
            else if (!backNormal && Time.time > _timeToBackNormalColor)
            {
                    classRecoLabel.color = Color.black;
                    backNormal = true;
            }
        }

        private void UpdateScoreClassifier(Dictionary<string,Dictionary<string,double>> classifiersScores)
        {
            if(histograms.Count==0)
                return;
            foreach (var scores in classifiersScores)
            {
                int index = app.model.Classes.IndexOf(scores.Key);

                foreach (var score in scores.Value)
                {
                    int index2 = app.model.Classes.IndexOf(score.Key);
                    histograms[index].SetScore(index2,(float)score.Value);
                }
            }
        }
        
        private void CleanScoreClassifier()
        {
            foreach (var histo in histograms)
            {
                histo.CleanScores();
            } 
        }

        /* public void AddThingToDo(Action a)
        {
            _todosInUpdate.Enqueue(a);
        }*/

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return))
            {
                Pause = false;
                RecoManager.GetInstance().Pause(false);

                RecoManager.GetInstance().ResetCurDi();
                CleanScoreClassifier();
            }

            if (_lastStatusSeuil!=SeuilsToggle.isOn)
            {
                _lastStatusSeuil = SeuilsToggle.isOn;
                NoiseValue.interactable = !SeuilsToggle.isOn;
                InitThetaPsi(NoiseValue.text,_lastStatusSeuil);
            }
            
            if(Pause)
                return;
            
            if (Time.time > _nextRefresh ) {
                _nextRefresh = Time.time+TIME_REFRESHING_HISTO;
                if(app.model.ClassRecognized!=null)
                    UpdateGestureReco(app.model.ClassRecognized);
                
                if(app.model.Histograms!=null && !Pause)
                    UpdateScoreHisto(app.model.Histograms);
               
                UpdateStatus();
            }
        }
    }
}