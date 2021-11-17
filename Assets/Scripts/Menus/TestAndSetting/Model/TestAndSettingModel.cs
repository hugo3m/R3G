using System.Collections.Generic;
using Recognizer;

namespace Menus.TestAndSetting.Model
{
    
    public class TestAndSettingModel
    {
        private string _classRecognized;
        public List<string> Classes { get; set; }
        public Dictionary<string, Dictionary<string, double>> Histograms { get; set; }
        public Dictionary<string, Dictionary<string, double>> ClassifiersScores { get; set; }

        public string ClassRecognized
        {
            get { return _classRecognized; }
            set
            {
                LastHisto = Histograms;
                LastScores = ClassifiersScores;
                Reco = true;
                _classRecognized = value; 
            }
        }

        public Dictionary<string, Dictionary<string, double>> LastHisto; 
        public Dictionary<string, Dictionary<string, double>> LastScores; 

        public bool Reco { get; set; }

        
        public RecoManager.StatusReco IsRecoWorking { get; set; } = RecoManager.StatusReco.NotWorkingAndTrying;
    }
}