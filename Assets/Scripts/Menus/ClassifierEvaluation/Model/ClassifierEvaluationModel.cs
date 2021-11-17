using System;
using System.Collections.Generic;
using Recognizer;
using Recognizer.EvaluationTools;

namespace Menus.ClassifierEvaluation.Model
{
    public class ClassifierEvaluationModel
    {
        public List<string> AppClassesLearned { get; set; }
        public Dictionary<string, List<GestureData>> DataPerClass{ get; set; } = new Dictionary<string, List<GestureData>>();

        public bool NewValue { get; set; } = false;
        private EvaluationResult _resEval;
        public EvaluationResult ResEval
        {
            get { return _resEval;}
            set
            {
                _resEval = value;
                NewValue = true;
            }
        }
    }
}