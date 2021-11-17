using System;
using System.Collections.Generic;
using System.Linq;

namespace Recognizer.EvaluationTools
{
    public class HoldOutEvaluation : EvaluationTool
    {
        private List<GestureData> testData;
        private List<GestureData> learnData;
        
        /// <summary>
        /// Split data into 2 parts, 'dataProportion' of each class will be used to train the classifier
        /// </summary>
        /// <param name="dataProportion"></param>
        /// <param name="data"></param>
        /// <param name="classifierManager"></param>
        public HoldOutEvaluation(float dataProportion, List<GestureData> data, IClassifierManager classifierManager) : base(data, classifierManager)
        {
            if(dataProportion<0 || dataProportion>100)
                throw  new Exception("Incorrect proportion purcentage");

            Dictionary<string, List<GestureData>> dataPerClass = new Dictionary<string, List<GestureData>>();


            foreach (GestureData gestureData in data)
            {
                if(!dataPerClass.ContainsKey(gestureData.Classe))
                    dataPerClass.Add(gestureData.Classe,new List<GestureData>());
                dataPerClass[gestureData.Classe].Add(gestureData);
            }

            testData = new List<GestureData>();
            learnData = new List<GestureData>();
            
            foreach (KeyValuePair<string,List<GestureData>> pair in dataPerClass)
            {
                int nb = pair.Value.Count;
                int nbToLearn = (int)(dataProportion / 100 * nb);
                List<GestureData> shuffled = pair.Value.Shuffle();
                learnData.AddRange(shuffled.GetRange(0, nbToLearn));
                testData.AddRange(shuffled.GetRange(nbToLearn, nb - nbToLearn));
            }

            testData=testData.Shuffle();
            learnData=learnData.Shuffle();
        }

        public override EvaluationResult Evaluate()
        {
            Dictionary<string,Dictionary<string,float>> confusionMatrix = this._classifierManager.LearnAndClassify(learnData, testData);

            return EvaluationTool.GetEvaluationResultFromConfusionMatrix(confusionMatrix);
        }
    }
}