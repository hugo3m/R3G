using System;
using System.Collections.Generic;
using Menus.TestAndSetting;
using Recognizer;
using Recognizer.EvaluationTools;

namespace Menus.ClassifierEvaluation.Controller
{
    /// <summary>
    /// cf. diagramme "ClassifierEvaluation"
    /// </summary>
    public class ClassifierEvaluationController : MenuController<ClassifierEvaluationApplication>
    {
       
        protected override void Start()
        {
            base.Start();
            RecoManager.GetInstance().StopRecognizer();
            
            app.model.AppClassesLearned = RecoManager.GetInstance().ParamRecoManager.GetAppClassesLearnt();
            Dictionary<string,Func<List<GestureData>>> dataClassesLazyAndHeaderDergOnly = DataManager.GetInstance().GetDataClassesLAZYAndHeaderDergOnly();
            foreach (KeyValuePair<string,Func<List<GestureData>>> valuePair in dataClassesLazyAndHeaderDergOnly)
            {
                app.model.DataPerClass.Add(valuePair.Key,valuePair.Value());
            }
            app.view.InitView();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            ClassifierEvaluationNotification.HoldOut += HoldOut;
            ClassifierEvaluationNotification.CrossValidation += KCrossValidation;
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
            ClassifierEvaluationNotification.HoldOut -= HoldOut;
            ClassifierEvaluationNotification.CrossValidation -= KCrossValidation;        }

        /// <summary>
        /// interroge le data manager  et le leapReco pour récupérer les données utiles pour l'évaluation
        /// </summary>
        /// <returns>la liste des Data dont on a besoin</returns>
        private List<GestureData> NeededData()
        {
            List<string> appClassesLearned = app.model.AppClassesLearned;
            Dictionary<string,List<GestureData>> dataPerClass = app.model.DataPerClass;
            
            List<GestureData> neededData = new List<GestureData>();
            foreach (KeyValuePair<string,List<GestureData>> pair in dataPerClass)
            {
                if(appClassesLearned.Contains(pair.Key))
                    neededData.AddRange(pair.Value);
            }

            return neededData;
        }

        /// <summary>
        /// Do the HoldOut evaluation
        /// </summary>
        /// <param name="dataProporion">The proportion of data to use for learning</param>
        public void HoldOut(float dataProporion)
        {
            RecoManager.GetInstance().StopRecognizer();
            
            List<GestureData> neededData = NeededData();
            HoldOutEvaluation holdOutEvaluation = new HoldOutEvaluation(dataProporion, neededData, RecoManager.GetInstance().ClassifierManager);
            EvaluationResult evaluationResult = holdOutEvaluation.Evaluate();
            app.model.ResEval = evaluationResult;

        }

        /// <summary>
        /// Do the KCross Validation evaluation
        /// </summary>
        /// <param name="k"></param>
        public void KCrossValidation(int k)
        {
            RecoManager.GetInstance().StopRecognizer();
            List<GestureData> neededData = NeededData();
            try
            {
                KCrossValidationEvaluation kCrossValidationEvaluation= new KCrossValidationEvaluation(k, neededData, RecoManager.GetInstance().ClassifierManager);
                EvaluationResult evaluationResult = kCrossValidationEvaluation.Evaluate();
                app.model.ResEval = evaluationResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
    
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


      
    }
}