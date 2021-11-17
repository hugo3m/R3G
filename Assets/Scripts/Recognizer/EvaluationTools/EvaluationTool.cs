using System;
using System.Collections.Generic;
using System.Linq;

namespace Recognizer.EvaluationTools
{
    public abstract class EvaluationTool
    {
        protected List<GestureData> _data;
        protected IClassifierManager _classifierManager;

        protected EvaluationTool(List<GestureData> data, IClassifierManager classifierManager)
        {
            _data = data;
            _classifierManager = classifierManager;
        }

        public abstract EvaluationResult Evaluate();

        protected static EvaluationResult GetEvaluationResultFromConfusionMatrix(Dictionary<string,Dictionary<string,float>> confusionMatrix)
        {
            
            //recall = correctlyPredicted / totalOfExampleWhichBelongToClass
            Dictionary<string,float> recallPerClass = new Dictionary<string, float>();
            
            //precision = correctlyPredicted / totalOfExampleAttributedToClass;
            Dictionary<string,float> precisionPerClass = new Dictionary<string, float>();

            //fmesure = 2 * (precision * recall) / (precision + recall);
            float fmesure = 0;
            
            //the proportion of patterns that have been incorrectly classified by a decision model.
            float errorRate=0f;
            
            float totalExamples=0f;

            
            foreach (KeyValuePair<string,Dictionary<string,float>> pair in confusionMatrix)
            {
                string classe = pair.Key;
                float correctlyPredicted = pair.Value[classe];
                float totalOfExampleWhichBelongToClass = pair.Value.Aggregate(0f,(acc,x)=>acc+x.Value);//total de la ligne
                recallPerClass[classe] = correctlyPredicted / totalOfExampleWhichBelongToClass;

                errorRate += Math.Abs(correctlyPredicted - totalOfExampleWhichBelongToClass);
                totalExamples += totalOfExampleWhichBelongToClass;
                
                
                float totalOfExampleAttributedToClass = confusionMatrix.Aggregate(0f,(acc,x)=>acc+x.Value[classe]);//total de la colonne
                precisionPerClass[classe] = correctlyPredicted / totalOfExampleAttributedToClass;
            }


            float recall = recallPerClass.Aggregate(0f, (acc, x) => acc + x.Value) / confusionMatrix.Count;
            float precision = precisionPerClass.Aggregate(0f, (acc, x) => acc + x.Value) / confusionMatrix.Count;
            
            
            if (precision + recall > 0.01)
            {
                fmesure = 2 * (precision * recall) / (precision + recall);
            }

            errorRate = errorRate / totalExamples;

            return new EvaluationResult(fmesure,recall,precision,errorRate,confusionMatrix);
        }

    }
}