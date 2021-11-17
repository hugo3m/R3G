using System;
using System.Collections.Generic;
using Menus.TestAndSetting.Model;
using Menus.TestAndSetting.View;
using Recognizer;
using Recognizer.StrategyFusion;
using UnityEngine;

namespace Menus.TestAndSetting.Controller
{
    /// <summary>
    /// cf. diagramme "ClassifierTestAndSetting"
    /// </summary>
    public class TestAndSettingController : MenuController<TestAndSettingApplication>
    {

        private TestAndSettingModel _model;
        private TestAndSettingView _view;


        protected override void Start()
        {
            base.Start();
          
            _model = app.model;
            _view = app.view;
            
            _model.Classes = RecoManager.GetInstance().ParamRecoManager.GetAppClassesLearnt();
            
            app.view.InitView( 
                RecoManager.GetInstance().StrategyAppRecognizer,
                RecoManager.GetInstance().ParamRecoManager.GetNoiseToleranceValue(),
                RecoManager.GetInstance().ParamRecoManager.GetCurvlinearDistanceValue(),
                RecoManager.GetInstance().ParamRecoManager.GetJointsStatus());
            if (RecoManager.GetInstance().State != RecoManager.StateCurrentReco.AppWorking)
            {
                RecoManager.GetInstance().StopRecognizer();
                RecoManager.GetInstance().StartAppRecognizer();
            }
         
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MenuNotificationGeneral.ClassScore += ScoresUpdate;
            MenuNotificationGeneral.OnStatusRecoChanged += StatusChanged;

            TestAndSettingNotification.ThetaModification += OnThetaModification;
            TestAndSettingNotification.CurvilineDistanceModification += OnCurvilineDistanceModification;
            TestAndSettingNotification.HandJointsModification += OnHandJointsModification;
            TestAndSettingNotification.RelearnAsk += OnRelearnAsk;
            try
            {
                if (RecoManager.GetInstance().State != RecoManager.StateCurrentReco.AppWorking)
                {
                    RecoManager.GetInstance().StopRecognizer();
                    RecoManager.GetInstance().StartAppRecognizer();
                }
            }
            catch (Exception) //the first time, no problem
            {
                // ignored
            }
           
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MenuNotificationGeneral.ClassScore -= ScoresUpdate;
            MenuNotificationGeneral.OnStatusRecoChanged -= StatusChanged;

            TestAndSettingNotification.ThetaModification -= OnThetaModification;
            TestAndSettingNotification.CurvilineDistanceModification -= OnCurvilineDistanceModification;
            TestAndSettingNotification.HandJointsModification -= OnHandJointsModification;
            TestAndSettingNotification.RelearnAsk -= OnRelearnAsk;
        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Escape))
                MenuNotificationGeneral.OnNavigationRequired(null);
            
        }

        private void StatusChanged(RecoManager.StatusReco status)
        {
            if (_model!=null)
                _model.IsRecoWorking = status;
        }

        /// <summary>
        /// Appelé à chaque mise jour des scores, notification envoyée depuis le LeapRecoManager
        /// </summary>
        /// <param name="scores"></param>
        private void ScoresUpdate(ResultStrategy scores)
        {
            if (_view!=null && !_view.Pause)
            {
                _model.Histograms=scores.HistogramsScores;
                _model.ClassifiersScores = scores.ClassifiersScores;
                if(scores.DecisionMade)
                    _model.ClassRecognized = scores.CurrentRecognizedClass;
            }
           
        }
     
        
        /// <summary>
        /// Lorsqu'un réapprentissage est demandé
        /// </summary>
        public void OnRelearnAsk()
        {
            RecoManager.GetInstance().RelearnAndStart();
        }

        /// <summary>
        /// Appelé lors de la modification du seuil
        /// </summary>
        /// <param name="value"></param>
        public void OnThetaModification(string value)
        {
            RecoManager.GetInstance().ParamRecoManager.SetNoiseToleranceValue(value);         
        }
        
        /// <summary>
        /// Appelé lors de la modification du pourcentage de la distance curviligne
        /// </summary>
        /// <param name="value"></param>
        public void OnCurvilineDistanceModification(float value)
        {
            RecoManager.GetInstance().ParamRecoManager.SetCurvilineDistancePurcentage(value);           
        }

        /// <summary>
        /// Appelé lors de la modification de la selection des doigts de la main pour l'apprentissage
        /// </summary>
        /// <param name="jointsStatus"></param>
        public void OnHandJointsModification(List<Tuple<LeapMotionJointType, bool>> jointsStatus)
        {
            if(jointsStatus.Count==14)
                RecoManager.GetInstance().ParamRecoManager.SetJointsStatus(jointsStatus);
            else
                throw new Exception("Erreur, nombre de jointures non correct");
        }
        
      
        
        protected override void MoveRight()
        {
        }

        protected override void MoveLeft()
        {
        }

        protected override void Enter()
        {
        }
        
        protected override void Exit()
        {
        }


      
    }
}