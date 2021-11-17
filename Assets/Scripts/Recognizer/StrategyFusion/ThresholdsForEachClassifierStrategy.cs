using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Recognizer;
using Recognizer.StrategyFusion;

namespace AppTrans.Scripts.Recognizer.StrategyFusion
{
    /// <summary>
    /// Will return a complete ResultStrategy
    /// The histograms are calculated using the Eric Anquetil's AMRG course, and the decision is taken considering its
    /// NOT Compatible with the old version of the RecognizerServer
    /// </summary>
    public class ThresholdsForEachClassifierStrategy : StrategyRecognizer,ISpecificPsiTheta
    {
        private Dictionary<string, Dictionary<string, double>> histogrammes;
        private List<string> _appClassesLearned;
        private bool _resetedHisto;

        public List<double> THETA = new List<double>(){10,10,8,8,6,6};
        public List<double> PSI = new List<double>(){20,20,16,16,12,12};
        private RecoManager.StateCurrentReco _status;

        public ThresholdsForEachClassifierStrategy()
        {
            histogrammes = new Dictionary<string, Dictionary<string, double>>();
        }
        
        

        public  override ResultStrategy OnFrameRecognitionResult(Dictionary<string, double> brutResult)
        {
            if (_appClassesLearned == null || RecoManager.GetInstance().State != _status)
            {
                _appClassesLearned = RecoManager.GetInstance().ParamRecoManager.GetAppClassesLearnt();
                _resetedHisto = false;
                ResetHistogram();
                _status = RecoManager.GetInstance().State;
            }


            Dictionary<string, Dictionary<string, double>>
                scores = new Dictionary<string, Dictionary<string, double>>();

            
            bool haveResult = false;
            string patternAll = @"^all_C(\d+)\.(.+)";
            foreach (KeyValuePair<string, double> p in brutResult)
            {
                Regex regex = new Regex(patternAll);
                MatchCollection matchesAll = regex.Matches(p.Key);
                foreach (Match matched in matchesAll)
                {
                 
                    haveResult = true;
                    int cid = int.Parse(matched.Groups[1].Value) - 1;
                    if (!scores.ContainsKey(_appClassesLearned[cid]))
                        scores.Add(_appClassesLearned[cid], new Dictionary<string, double>());

                    string classScored = matched.Groups[2].Value;
                    double score = p.Value;
                    scores[_appClassesLearned[cid]].Add(classScored, score);
                }
            }

            if (!haveResult) //pas de valeurs renvoyés -> pas encore de score ou perte de tracking du leap
            {
                ResetHistogram();
                _resetedHisto = true;
            }
            
            
        
            bool decisionMade = false;
            string bestClass = null;
            double higherDiff = 0;

            foreach (KeyValuePair<string, Dictionary<string, double>> score in scores) //score.Key = Ci
            {
                _resetedHisto = false;
                if (histogrammes[score.Key].All((couple) => couple.Value == 0)) //for the init
                    histogrammes[score.Key] = score.Value;
                else
                {
                    var orderByScore = score.Value.OrderByDescending(couple => couple.Value).ToList();

                    // beta equals to the difference between
                    //the score of the currently predicted class (predicted_i)
                    //and the score of the secondly ranked predicted class by the classifier Ci
                    double beta = orderByScore[0].Value - orderByScore[1].Value;

                    foreach (KeyValuePair<string, double> scoClassJ in score.Value) //scoClassJ.Key = Cj
                    {
                        if (scoClassJ.Key == orderByScore[0].Key)
                        {
                            histogrammes[score.Key][scoClassJ.Key] += beta;
                        }
                        else
                        {
                            //gamma corresponds to the difference between
                            //the score of predicted_i
                            //the score of the jth class corresponding the jth entry of the histogram
                            double gamma = orderByScore[0].Value - scoClassJ.Value;
                            histogrammes[score.Key][scoClassJ.Key] -= gamma;
                            if (histogrammes[score.Key][scoClassJ.Key] < 0)
                                histogrammes[score.Key][scoClassJ.Key] = 0;
                        }

                        //check if it is higher than threshold
                        double seuil =PSI[ _appClassesLearned.IndexOf(score.Key)];
                        if (scoClassJ.Key == score.Key) //check with theta
                            seuil = THETA[ _appClassesLearned.IndexOf(score.Key)];

                        if (histogrammes[score.Key][scoClassJ.Key] >= seuil)
                        {
                            double diff = histogrammes[score.Key][scoClassJ.Key] - seuil;
                            if ((!decisionMade) || diff >= higherDiff)
                            {
                                decisionMade = true;
                                higherDiff = diff;
                                bestClass = scoClassJ.Key;
                            }
                        }
                    }
                }
            }


            Dictionary<string, Dictionary<string, double>> histograms = histogrammes;

            if (decisionMade)
            {
                //on demande au moteur de réinitialiser la distance curviligne car on a pris une décision
                RecoManager.GetInstance().ResetCurDi();
                histograms = new Dictionary<string, Dictionary<string, double>>(histogrammes); //make a copy
                ResetHistogram();
                _resetedHisto = true;
            }

            return new ResultStrategy(decisionMade, bestClass, scores, histograms);
        }


        private void ResetHistogram()
        {
            if(_resetedHisto)
                return;
            histogrammes.Clear();
            foreach (string g1 in _appClassesLearned)
            {
                histogrammes.Add(g1, new Dictionary<string, double>());
                foreach (string g2 in _appClassesLearned)
                {
                    histogrammes[g1].Add(g2, 0);
                }
            }
        }
        public override void NewGesturesLearned()
        {
            _appClassesLearned = null;
        }
        public double GetTheta()
        {
            return THETA[0];
        }

        public double GetPsi()
        {
            return PSI[0];
        }
    }
}