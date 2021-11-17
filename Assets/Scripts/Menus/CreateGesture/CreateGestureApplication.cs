using System;
using Menus.CreateGesture.Controller;
using Menus.CreateGesture.Model;
using Menus.CreateGesture.View;
using Menus.MainMenu.Controller;
using Menus.MainMenu.Model;
using Menus.MainMenu.View;
using UnityEngine;

namespace Menus.CreateGesture
{
    public class CreateGestureElement : MenuElement<CreateGestureApplication>
    {
    }
    public class CreateGestureApplication: AppMenuItem
    {
        [NonSerialized]
        public CreateGestureModel model;
        public CreateGestureView view;

        private void Awake()
        {
            model = new CreateGestureModel();
        }

 

        public override string GetTitle()
        {
            return "Création de geste";
        }
    }   
}