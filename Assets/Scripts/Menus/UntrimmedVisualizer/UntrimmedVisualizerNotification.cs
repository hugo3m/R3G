using System;
using Recognizer;

namespace Menus.UntrimmedVisualizer
{
    public class UntrimmedVisualizerNotification
    {
        public static Action<string> SequenceSelected;

        public static Action<GestureData> DataSelected;
        public static Action ReplayClicked;
        public static Action<float> UpdatePosLecture;
    }
}