using System;
using Menus.ReaffectAction.Controller;
using Menus.ReaffectAction.View;
using Menus.UntrimmedVisualizer.Controller;
using Menus.UntrimmedVisualizer.Model;
using Menus.UntrimmedVisualizer.View;
using UnityEditor;

namespace Menus.UntrimmedVisualizer
{
  
    public class UntrimmedVisualizerElement : MenuElement<UntrimmedVisualizerApplication>
    {
    }
    public class UntrimmedVisualizerApplication: AppMenuItem
    {
        [NonSerialized]
        public UntrimmedVisualizerModel model;
        public UntrimmedVisualizerView view;


        private void Awake()
        {
            base.Awake();
            model = new UntrimmedVisualizerModel();
        }

        private void Start()
        {
        }

      
        public override string GetTitle()
        {
            return "Visualisation de gestes non-segmentés";
        }
    }
}