using System;
using System.Collections.Generic;
using System.Linq;

namespace Recognizer.EvaluationTools
{
    public class KCrossValidationEvaluation: EvaluationTool
    {
        private List<List<GestureData>> _splitedData;
        private int _k;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="data"></param>
        /// <param name="classifierManager"></param>
        /// <exception cref="Exception">If K is not cohérent k<1 || k>data.Count</exception>
        public KCrossValidationEvaluation(int k,List<GestureData> data, IClassifierManager classifierManager) : base(data, classifierManager)
        {
            this._k = k;
            if(k<1 || k>data.Count)
                throw new Exception("K non cohérent");
            Dictionary<string, List<GestureData>> dataPerClass = new Dictionary<string, List<GestureData>>();


            foreach (GestureData gestureData in data)
            {
                if(!dataPerClass.ContainsKey(gestureData.Classe))
                    dataPerClass.Add(gestureData.Classe,new List<GestureData>());
                dataPerClass[gestureData.Classe].Add(gestureData);
            }

            if (!dataPerClass.All((dico) => dico.Value.Count >= k))
                throw new Exception("Toutes les classes doivent avoir au moins K ("+k+") exemples");

            Dictionary<string, List<List<GestureData>>> splitedDataPerClasses = new Dictionary<string, List<List<GestureData>>>();

            foreach (var pair in dataPerClass)
            {
                List<List<GestureData>> splited = new List<List<GestureData>>();
                string classe = pair.Key;
                List<GestureData> gestureDatasShuffled = pair.Value.Shuffle();
                int nbGesteParSerie = gestureDatasShuffled.Count / k;
                for (int i = 0; i < k-1; i++)
                {
                    List<GestureData> serieClass = gestureDatasShuffled.GetRange(i * nbGesteParSerie, nbGesteParSerie);
                    splited.Add(serieClass);
                }
                int reste = gestureDatasShuffled.Count % k;
                List<GestureData> lastSerieClass = gestureDatasShuffled.GetRange((k-1) * nbGesteParSerie, nbGesteParSerie+reste);
                splited.Add(lastSerieClass);
                
                splitedDataPerClasses[classe] = splited;
            }

            _splitedData=new List<List<GestureData>>();
            for (int i = 0; i < k; i++)
            {
                List<GestureData> serie = new List<GestureData>();
                foreach (var dataClass in splitedDataPerClasses)
                {
                    serie.AddRange(dataClass.Value[i]);
                }

                serie = serie.Shuffle();
                _splitedData.Add(serie);
            }
        }

        public override EvaluationResult Evaluate()
        {
            Dictionary<string, Dictionary<string, float>> globalConfusionMatrix = new Dictionary<string, Dictionary<string, float>>();
            //do a test foreach series
            for (int i = 0; i < _k; i++)
            {
                List<GestureData> testData = _splitedData[i];
                
                List<GestureData> learnData = new List<GestureData>();
                for (int j = 0; j < _k; j++)
                {
                    if (j != i)
                    {
                        learnData.AddRange(_splitedData[j]);
                    }
                }

                Dictionary<string,Dictionary<string,float>> confusionMatrixI = _classifierManager.LearnAndClassify(learnData, testData);
                AddElementWiseConfusionMatrix(globalConfusionMatrix, confusionMatrixI);
            }

            //do the average
            Dictionary<string,Dictionary<string,float>> averageMatrix = DivideByNTheConfusionMatrix(globalConfusionMatrix, _k);

            return GetEvaluationResultFromConfusionMatrix(averageMatrix);
        }

        private Dictionary<string, Dictionary<string, float>> DivideByNTheConfusionMatrix(Dictionary<string, Dictionary<string, float>> globalConfusionMatrix, int n)
        {
            Dictionary<string, Dictionary<string, float>> average = new Dictionary<string, Dictionary<string, float>>();
            foreach (KeyValuePair<string,Dictionary<string,float>> pair in globalConfusionMatrix)
            {
                string cRow = pair.Key; 
                if (!average.ContainsKey(cRow))
                    average[cRow] = new Dictionary<string, float>();
                foreach (KeyValuePair<string, float> classVal in pair.Value)
                {
                    string cCol = classVal.Key;
                    average[cRow][cCol] = globalConfusionMatrix[cRow][cCol]/n;
                }
            }

            return average;
        }

        private void AddElementWiseConfusionMatrix(Dictionary<string, Dictionary<string, float>> globalConfusionMatrix, Dictionary<string, Dictionary<string, float>> confusionMatrixI)
        {
     
            foreach (KeyValuePair<string,Dictionary<string,float>> pair in confusionMatrixI)
            {
                string cRow = pair.Key;
                if (!globalConfusionMatrix.ContainsKey(cRow))
                    globalConfusionMatrix[cRow] = new Dictionary<string, float>();
                foreach (KeyValuePair<string, float> classVal in pair.Value)
                {
                    string cCol = classVal.Key;
                    if (!globalConfusionMatrix[cRow].ContainsKey(cCol))
                        globalConfusionMatrix[cRow][cCol] = 0f;
                    globalConfusionMatrix[cRow][cCol] += classVal.Value;
                }
            }
        }
    }
}