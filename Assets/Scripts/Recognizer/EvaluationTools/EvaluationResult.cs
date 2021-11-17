using System.Collections.Generic;

namespace Recognizer.EvaluationTools
{
    public class EvaluationResult
    {
        public float Fscore { get; }
        public float Recall { get; }
        public float Precision { get; }
        public float ErrorRate { get; }
        public  Dictionary<string,Dictionary<string,float>> ConfusionMatrix{ get; }

        public EvaluationResult(float fscore, float recall, float precision, float errorRate, Dictionary<string, Dictionary<string, float>> confusionMatrix)
        {
            Fscore = fscore;
            Recall = recall;
            Precision = precision;
            ErrorRate = errorRate;
            ConfusionMatrix = confusionMatrix;
        }
    }
}