using System;
using System.Collections.Generic;
using System.Linq;
using Menus.ReaffectAction.View;
using Recognizer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Menus.ReaffectAction.Controller
{
    /// <summary>
    /// cf. diagramme "Reaffect" et "Reaffect_suite"
    /// </summary>
    public class ReaffectActionController : MenuController<ReaffectActionApplication>
    {
        public IActionProvider provider;
        private ReaffectActionView _view;
        private ReaffectActionModel _model;

        
      
        
        protected override void Start()
        {
            base.Start();
          
            _view  = app.view;
            _model = app.model;
            Init();
          
            if (RecoManager.GetInstance().State != RecoManager.StateCurrentReco.MenuWorking)
            {
                RecoManager.GetInstance().StopRecognizer();
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            ReaffectActionNotification.Change += Change;
            ReaffectActionNotification.AddChange += AddChange;
            ReaffectActionNotification.Replay += Replay;
            ReaffectActionNotification.Deselect += Deselect;
            ReaffectActionNotification.AddToLearn += AddToLearn;
            ReaffectActionNotification.RemoveGestureToLearn += RemoveGestureToLearn;
            ReaffectActionNotification.Init += Init;
            //ReaffectActionNotification.ShowToLearn += ShowToLearn;
            try
            {
                RecoManager.GetInstance().StopRecognizer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private void Init()
        {
            List<string> learned = RecoManager.GetInstance().ParamRecoManager.GetAppClassesLearnt();

            List<Tuple<string,string>> gestActions = provider.GetMapAction();
            List<string> gestesWithActions = gestActions.Select(x => x.Item1).Where(x=>x!="").ToList();
            
            learned.RemoveAll(x=>gestesWithActions.Contains(x));
            app.model.ToLearnClasses = learned;
            
            app.model.InitToLearnClassesTotal = new List<string>(learned);
            app.model.InitToLearnClassesTotal.AddRange(gestesWithActions);
            app.model.Changes = gestActions;

            _view.InitView(gestActions,learned);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            ReaffectActionNotification.Change -= Change;
            ReaffectActionNotification.AddChange -= AddChange;
            ReaffectActionNotification.Replay -= Replay;
            ReaffectActionNotification.Deselect -= Deselect;
            ReaffectActionNotification.AddToLearn -= AddToLearn;
            ReaffectActionNotification.RemoveGestureToLearn -= RemoveGestureToLearn;
            ReaffectActionNotification.Init -= Init;

        }

       

        protected override void Update()
        {
            base.Update();
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveRight();
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveLeft();
            }
            if(Input.GetKeyDown(KeyCode.Return))
            {
                Enter();
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
            }
            
        }

        /// <summary>
        /// Traitement lors de la demande du replay du geste
        /// </summary>
        /// <param name="geste"></param>
        public void Replay(string geste)
        {
            GestureData data = DataManager.GetInstance().GetGestureDataFromClassName(geste);
            _view.Play(data);
        }
        
        private void Deselect(string action, Selectable selectedButton)
        {
            _model.DeselectGesture(action, selectedButton);
            _model.AvailabilityClasses = GetAvailabilityClasses(provider.GetMapAction(),
                DataManager.GetInstance().GetAllDataClasseName());
            _view.HideScrollPanel(_view.availableGestPanel);
            _view.DisplayRelarn(_model.HasChanges());
        }
        private void RemoveGestureToLearn(string classe, Selectable selectedButton)
        {
            _model.ToLearnClasses.Remove(classe);
      
            _model.AvailabilityClasses = GetAvailabilityClasses(provider.GetMapAction(),
                DataManager.GetInstance().GetAllDataClasseName());
            _view.HideScrollPanel(_view.availableGestPanel);
            _view.DisplayRelarn(_model.HasChanges());
        }
        /// <summary>
        /// Traitement lors de la demande d'un changement d'un geste
        /// </summary>
        private void Change(bool justGest)
        {
            //Dictionary<string, string> usedGests = provider.GetMapAction();
            List<Tuple<string, string>> usedGests = _model.GetChanges();
            List<string> allKnownClassK = DataManager.GetInstance().GetAllDataClasseName();
            _model.AvailabilityClasses = GetAvailabilityClasses(usedGests, allKnownClassK);
            _view.ShowAvailableClasses(_model.AvailabilityClasses,justGest);
        }

//        private void ShowToLearn()
//        {
//            //_model.AddToLearn(geste);
//            _model.ToLearnClasses = DataManager.GetInstance().GetAllDataClasseName();
//            _view.ShowToLearnClasses(_model.ToLearnClasses);
//        }

        private void AddToLearn(string old, string newGeste)
        {
            if (old != "")
                _model.ToLearnClasses.Remove(old);
            _model.ToLearnClasses.Add(newGeste);
            _view.HideScrollPanel(_view.availableGestPanel);
            _view.DisplayRelarn(_model.HasChanges());
        }


        /// <summary>
        /// donne des infos sur la disponiblité
        /// </summary>
        /// <param name="gestUsed"></param>
        /// <param name="allClasses"></param>
        /// <returns>Liste de triplet <nomClasse,Donnée Représentative, avaible></returns>
        public List<Tuple<string, GestureData, bool>> GetAvailabilityClasses(List<Tuple<string, string>> gestUsed,
            List<string> allClasses)
        {
            List<Tuple<string, GestureData, bool>> availabilityGestList = new List<Tuple<string, GestureData, bool>>();
            foreach (string gest in allClasses)
            {
                bool usedInActions = gestUsed.Contains(gestUsed.Find(x => x.Item1 == gest));
                bool usedInApp = app.model.ToLearnClasses.Contains(gest);
                availabilityGestList.Add(new Tuple<string, GestureData, bool>(gest, null, !usedInActions && !usedInApp));
            }

            return availabilityGestList;

        }

        /// <summary>
        /// Ajout d'un changement
        /// </summary>
        /// <param name="gest"></param>
        /// <param name="action"></param>
        /// <param name="selectedButton"></param>
        public void AddChange(string gest, string action, Selectable selectedButton)
        {
            _model.AddChange(gest, action, selectedButton);
            //selectedButton.GetComponentsInChildren<Button>()[1].interactable = false; //Deactivate Replay button
            _view.HideScrollPanel(_view.availableGestPanel);
            _view.DisplayRelarn(_model.HasChanges());

        }
        
        /// <summary>
        /// Setter de l'action provider
        /// </summary>
        /// <param name="provider"></param>
        public void SetActionProvider(IActionProvider provider)
        {
            this.provider = provider;
        }
        
        protected override void MoveRight()
        {
            _view.DoMoveRight();
        }

        protected override void MoveLeft()
        {
            _view.DoMoveLeft();
        }

        protected override void Enter()
        {
            _view.DoEnter();
        }

        protected override void Exit()
        {
            _view.DoExit();
        }

        private void OnApplicationQuit()
        {
            /*if (app.model.HasChanges())
            {
                _model.DoChange();
            }*/
        }
    }
}