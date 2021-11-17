using System;
using System.Collections.Generic;
using Menus.MainMenu.View;
using Recognizer;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.MainMenu.Controller
{
    public class MainMenuController : MenuController<MainMenuApplication>
    {
        //Prend en paramètre tous les prefabs des sous-menus.
        private Selectable selectedButton;
        private Selectable destination;
        private bool _enterIn;
        private bool _isInSubMenu = false;
        private MainMenuView _view;
        private bool _doExit = false;

        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            app.Draw();

            _view = app.view;
            _view.InitView();
            
            if (RecoManager.GetInstance().State != RecoManager.StateCurrentReco.MenuWorking)
            {
                RecoManager.GetInstance().StopRecognizer();
                RecoManager.GetInstance().StartMenuRecognizer();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
          /*  try
            {
                if (RecoManager.GetInstance().State != RecoManager.StateCurrentReco.MenuWorking)
                {
                    RecoManager.GetInstance().StopRecognizer();
                    RecoManager.GetInstance().StartMenuRecognizer();
                }
            }
            catch (Exception) //the first time, no problem
            {
                // ignored
            }*/
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            if (_view.isActiveAndEnabled)
            {
                _isInSubMenu = false;
                if (RecoManager.GetInstance().State != RecoManager.StateCurrentReco.MenuWorking)
                {
                    RecoManager.GetInstance().StopRecognizer();
                    RecoManager.GetInstance().StartMenuRecognizer();
                }
            }
            
            if(_isInSubMenu)
                return;
            
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Z))
            {
                MoveRight();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveLeft();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Enter();
            }

            if (_enterIn)
            {
                _isInSubMenu = true;
                app.Undraw();//to do immediately
                //appeler event a la place
                AppMenuItem selected = app.model.appMenuItems[app.model.currentSelectedItem];
                _enterIn = false;
                MenuNotificationGeneral.OnNavigationRequired(selected);
            }

            if (_doExit)
            {
                _doExit = false;
                MenuNotificationGeneral.OnNavigationRequired(null);
            }
        }

        protected override void MoveRight()
        { 
                _view.MoveRight();
        }

        protected override void MoveLeft()
        {
                _view.MoveLeft();
        }

        protected override void Enter()
        {
                _enterIn = true;
        }

        protected override void Exit()
        {
            _doExit = true;
        }
    }
}