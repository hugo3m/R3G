using System;
using System.Collections.Generic;
using Recognizer;
using UnityEngine;

namespace Menus.UntrimmedVisualizer.Model
{
    public class UntrimmedVisualizerModel
    {
        public GestureData selectedData;
        public string SequenceSelected;
        public Dictionary<string, List<Tuple<GestureData, Texture2D>>> Pictures = new Dictionary<string, List<Tuple<GestureData, Texture2D>>>();
        public Dictionary<string, Func<List<GestureData>>> AllClassesAndDataLazy;
        public Dictionary<string, List<GestureData>> AllClassesAndData = new Dictionary<string, List<GestureData>>();
        public bool pauseToggle = false;

        public void SetDataClass(Dictionary<string,Func<List<GestureData>>> classes)
        {
            AllClassesAndDataLazy = classes;
        }
    }
}