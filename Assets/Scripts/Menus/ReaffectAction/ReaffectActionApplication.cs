using System;
using System.Collections.Generic;
using Menus.ReaffectAction.Controller;
using Menus.ReaffectAction.View;
using UnityEditor;
using UnityEngine;

namespace Menus.ReaffectAction
{
    public class ReaffectActionElement : MenuElement<ReaffectActionApplication>
    {
    }
    public class ReaffectActionApplication: AppMenuItem
    {
        [NonSerialized]
        public ReaffectActionModel model;
        public ReaffectActionView view;
        public ReaffectActionController controller;
        public AppMenuItem createGesture;

        public IActionProvider provider;

        private void Awake()
        {
            base.Awake();
            model = new ReaffectActionModel(provider, createGesture);
        }

        private void Start()
        {
            controller.SetActionProvider(provider);
        }

      
        public override string GetTitle()
        {
            return "Réaffectation de geste";
        }
    }
}