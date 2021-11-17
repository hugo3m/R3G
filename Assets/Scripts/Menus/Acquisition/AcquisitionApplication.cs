using System;
using Menus.Acquisition.Controller;
using Menus.Acquisition.Model;
using Menus.Acquisition.View;
using UnityEngine;

namespace Menus.Acquisition
{
    public class AcquisitionElement : MenuElement<AcquisitionApplication>
    {
    }
    public class AcquisitionApplication : AppMenuItem
    {
        [NonSerialized]
        public AcquisitionModel model;
        public AcquisitionView view;

        private void Awake()
        {
            model = new AcquisitionModel();
        }



        public override string GetTitle()
        {
            return "Acquisition";
        }
    }
}