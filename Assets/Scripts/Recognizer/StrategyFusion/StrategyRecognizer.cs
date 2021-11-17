using System.Collections.Generic;
using UnityEngine;

namespace Recognizer.StrategyFusion
{
    public abstract class StrategyRecognizer : MonoBehaviour
    {
        public abstract ResultStrategy OnFrameRecognitionResult(Dictionary<string, double> brutResult);
        public abstract void NewGesturesLearned();
    }
}