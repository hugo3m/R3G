using System;
using System.Collections;
using System.Collections.Generic;
using Menus;
using Menus.MainMenu.Controller;
using Menus.MainMenu.Model;
using Menus.MainMenu.View;
using UnityEngine;

namespace Menus.MainMenu
{
    public class MainMenuElement : MenuElement<MainMenuApplication>
    {
    }
    public class MainMenuApplication : AppMenuItem
    {
        [NonSerialized]
        public MainMenuModel model;
        public MainMenuView view;
        
        public List<AppMenuItem> appMenuItems;

        private void Awake()
        {
            model = new MainMenuModel(appMenuItems);
        }
        
        protected override void Update()
        {
            if (_hasToBeDraw)
            {
                view.gameObject.SetActive(true);
                _hasToBeDraw = false;
                IsDrawn = true;
            }

            if (_hasToBeUndraw)
            {
                view.gameObject.SetActive(false);
                _hasToBeUndraw = false;
                IsDrawn = false;

            }
        }

      

        public override string GetTitle()
        {
            return "Menu principal";
        }
    }
}