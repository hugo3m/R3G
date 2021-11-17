using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Recognizer.StrategyFusion;

namespace Recognizer
{
    /// <summary>
    /// Will return a ResultStrategy using the default strategy of the Recognizer
    /// The histograms are not Calculated (probably faster?)
    /// The scores consider only the best class of each classifier
    /// Compatible with the old version of the RecognizerServer
    /// </summary>
    public class DefaultStrategy : StrategyRecognizer
    {
        private Dictionary<string, Dictionary<string, double>> histogrammes;
        private List<string> _appClassesLearned;
        private RecoManager.StateCurrentReco _status;

        public DefaultStrategy()
        {
            histogrammes = new Dictionary<string, Dictionary<string, double>>();
        }

        public override ResultStrategy OnFrameRecognitionResult(Dictionary<string, double> brutResult)
        {
            if (_appClassesLearned == null || RecoManager.GetInstance().State != _status)
            {
                _appClassesLearned = RecoManager.GetInstance().ParamRecoManager.GetAppClassesLearnt();
                _status = RecoManager.GetInstance().State;
            }

            Dictionary<string, int> nbClass = new Dictionary<string, int>();
            Dictionary<string, Dictionary<string,double>> scores = new Dictionary<string, Dictionary<string, double>>();
    
            string pattern = @"^C(\d+)\.(.+)";
            int nbClassifiers = 0;

            foreach (KeyValuePair<string, double> p in brutResult)
            {
       

                Regex reg = new Regex(pattern);
                MatchCollection matches = reg.Matches(p.Key);
                foreach (Match match in matches)
                {
                    string cid = match.Groups[1].Value;
                    string classReco = match.Groups[2].Value;
                    
                    nbClassifiers++;

                    if (!classReco.Equals("?"))
                    {
                        var bestReco = new Dictionary<string, double>();//on a une seule valeur par classifieur pour l'instant
                        bestReco.Add(classReco,p.Value);
                        if(cid=="0")
                            continue;
                       
                        scores.Add(_appClassesLearned[int.Parse(cid)-1],bestReco);
                        if (p.Value == 1)
                        {
                            if (!nbClass.ContainsKey(classReco))
                            {
                                nbClass[classReco] = 0;
                            }
                            nbClass[classReco]++;
                        }
                    }
                }
            }

            bool decisionMade = false;
            string classeReco = "";
            try
            {
                //si il y a un "1" pour chaque classe, le classifier a pris sa décision
                KeyValuePair<string, int> kp = nbClass.First(x => (x.Value == nbClassifiers));
                decisionMade = true;
                classeReco = kp.Key;
            }
            catch (InvalidOperationException)
            {
                decisionMade = false;
            }

            return new ResultStrategy(decisionMade,classeReco,scores,histogrammes);
        }
        public override void NewGesturesLearned()
        {
            _appClassesLearned = null;
        }
    }
}