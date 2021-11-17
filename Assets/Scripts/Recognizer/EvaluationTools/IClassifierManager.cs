using System.Collections.Generic;

namespace Recognizer.EvaluationTools
{
    public interface IClassifierManager
    {
        Dictionary<string,Dictionary<string,float>> LearnAndClassify(List<GestureData> datasForLearn, List<GestureData> dataToClassify);
    }
}