using System.Collections.Generic;

namespace Recognizer.StrategyFusion
{
    /// <summary>
    /// Store the result of a strategy
    /// ClassifiersScores is type : Dictionary<string1,Dictionary<string2,double>>
    /// string1 is the name of the class for which the classifier is expert
    /// the double is the score of string2 classified by the classifier expert in the class string1
    /// </summary>
    public class ResultStrategy
    {
        public bool DecisionMade { get; }
        public string CurrentRecognizedClass { get; }
        public Dictionary<string,Dictionary<string,double>> ClassifiersScores { get; }
        public Dictionary<string,Dictionary<string,double>> HistogramsScores { get; }

        public ResultStrategy(bool decisionMade, string currentRecognizedClass, Dictionary<string, Dictionary<string, double>> classifiersScores, Dictionary<string, Dictionary<string, double>> histogramsScores)
        {
            this.DecisionMade = decisionMade;
            this.CurrentRecognizedClass = currentRecognizedClass;
            this.ClassifiersScores = classifiersScores;
            this.HistogramsScores = histogramsScores;
        }
    }
}