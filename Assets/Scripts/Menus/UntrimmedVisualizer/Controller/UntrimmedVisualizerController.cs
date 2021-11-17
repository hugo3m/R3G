using System;
using System.Collections.Generic;
using Menus.ReaffectAction;
using Recognizer;
using Recognizer.DataTools;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Menus.UntrimmedVisualizer.Controller
{
    public class UntrimmedVisualizerController : MenuController<UntrimmedVisualizerApplication>
    {
        protected override void Start()
        {
            base.Start();


            if (RecoManager.GetInstance().State != RecoManager.StateCurrentReco.NotWorking)
            {
                RecoManager.GetInstance().StopRecognizer();
            }
            UntrimmedDataManager dataManager = new PKUMMDv1DataManager();
            // UntrimmedDataManager dataManager = new MSRC12DataManager();
            // UntrimmedDataManager dataManager = new ChalearnDataManager();
            // UntrimmedDataManager dataManager = new OADDataManager();
            Dictionary<string,Func<List<GestureData>>> classes = dataManager.GetDataClassesLAZYAndHeaderDergOnly();

            app.model.SetDataClass(classes);

            app.view.InitView(classes,null);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UntrimmedVisualizerNotification.SequenceSelected += OnSequenceSelected;
            UntrimmedVisualizerNotification.DataSelected += OnDataSelected;
            UntrimmedVisualizerNotification.ReplayClicked += OnReplayClicked;
            UntrimmedVisualizerNotification.UpdatePosLecture += OnUpdatePosLecture;

            try
            {
                RecoManager.GetInstance().StopRecognizer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                app.model.pauseToggle = true;
            }
            
            if (Input.GetKeyDown(KeyCode.V))
            {
                app.view.DrawVoxelsInTime();
            }
        }

        private void OnUpdatePosLecture(float obj)
        {
            app.view.UpdatePosLecture(obj);
        }

        /// <summary>
        /// Met à jour le model en fonction de la classe selectionné
        /// </summary>
        /// <param name="classe">la classe</param>
        private void OnSequenceSelected(string seq)
        {
            app.model.SequenceSelected = seq;
        }

        /// <summary>
        /// Met à jour le model en fonction de la donnée selectionné
        /// </summary>
        /// <param name="data">la donnée</param>
        private void OnDataSelected(GestureData data)
        {
            app.model.selectedData =data;
        }
        /// <summary>
        /// lance le replay de la data selectionnée
        /// </summary>
        private void OnReplayClicked()
        {
            GestureData data = app.model.selectedData;
            app.view.PlayFile(data);
        }
        
        /// <summary>
        /// lance le replay de la data selectionnée
        /// </summary>
        public void OnVoxelizedClicked(bool activated)
        {
            app.view.ShowVoxels(activated);
        }

        protected override void MoveRight()
        {
            throw new NotImplementedException();
        }

        protected override void MoveLeft()
        {
            throw new NotImplementedException();
        }

        protected override void Enter()
        {
            throw new NotImplementedException();
        }
    }
}