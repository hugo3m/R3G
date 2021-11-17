using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Menus.CreateGesture.Model;
using Recognizer;
using Recognizer.DataTools;
using UnityEngine;

namespace Menus.CreateGesture.Controller
{
    /// <summary>
    /// cf. diagramme de classe "Create"
    /// </summary>
    public class CreateGestureController : MenuController<CreateGestureApplication>
    {
        private bool recordPlaying = false;
        private Double[] _leapFrame;
        private StringBuilder _outData;
        private CreateGestureModel _model;

        protected override void Start()
        {
            base.Start();
            _leapFrame = new double[RecoManager.GetInstance().DeviceInfo.FrameSize];
            RecoManager.GetInstance().StopRecognizer();
            Dictionary<string, Func<List<GestureData>>> classes = DataManager.GetInstance().GetDataClassesLAZYAndHeaderDergOnly();
            app.model.SetDataClass(classes);
            app.view.InitView(classes, null);
        }

      

        protected override void OnEnable()
        {
            base.OnEnable();
               
            CreateGestureNotification.ClassSelected += OnClasseSelected;
            CreateGestureNotification.DataSelected += OnDataSelected;
            CreateGestureNotification.ReplayClicked += OnReplayClicked;
            CreateGestureNotification.DeleteData += OnDeleteClicked;
            CreateGestureNotification.Record += Record;
            try
            {
                RecoManager.GetInstance().StopRecognizer();
            }
            catch (Exception e)
            {
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CreateGestureNotification.ClassSelected -= OnClasseSelected;
            CreateGestureNotification.DataSelected -= OnDataSelected;
            CreateGestureNotification.ReplayClicked -= OnReplayClicked;
            CreateGestureNotification.DeleteData -= OnDeleteClicked;
            CreateGestureNotification.Record -= Record;
        }

        protected override void MoveRight()
        {
            app.model.UpdateNextDataOfSelected();
        }

        protected override void MoveLeft()
        {
           app.model.UpdatePreviousDataOfSelected();
        }

        /// <summary>
        /// Start and stop l'enregistrement
        /// </summary>
        protected override void Enter()
        {
           app.view.StartStopRecord();
        }


        protected override void Update()
        {
            base.Update();
            
           
            
        }

        private IEnumerator Recording()
        
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            while (true)
            {
                if (recordPlaying)
                {
                    RecoManager.GetInstance().DetectAndFillPositionDevice(ref _leapFrame);
                    StringBuilder frame = new StringBuilder("");
                    for (int i = 0; i < _leapFrame.Length; i++)
                        frame .Append( _leapFrame[i].ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(" ");

                    _outData.Append(frame).Append("\n");
                }
                else
                {
                    yield break;
                }
                yield return new WaitForSeconds(RecoManager.FREQUENCY_RECORD_SEND_DATA);
            }
        }
        
        /// <summary>
        /// Called by the view to start/stop recording
        /// </summary>
        /// <param name="start"></param>
        private void Record(bool start)
        {
            if (!start)
            {
                GestureData data = StopRecord();
                DataManager.GetInstance().AddData(data);
                if(!app.model.Pictures.ContainsKey(data.Classe))
                    app.model.Pictures.Add(data.Classe, new List<Tuple<GestureData, Texture2D>>());
                Texture2D imageFromDataGesture = DataManager.GetInstance().GetImageFromDataGesture(data);
                Tuple<GestureData,Texture2D> dataTexture = new Tuple<GestureData, Texture2D>(data, imageFromDataGesture);
                app.model.Pictures[data.Classe]
                    .Add(dataTexture);
                app.view.AddSingleImageOfClass(data.Classe, dataTexture);
                //app.view.InitView(DataManager.GetInstance().GetDataClasses(), data.Classe);
            }
            else
            {
                StartRecord();
            }
        }

        /// <summary>
        /// Démarre l'enregistrement
        /// </summary>
        private void StartRecord()
        {
            recordPlaying = true;
            _outData = new StringBuilder();
            StartCoroutine(Recording());
        }

        
        /// <summary>
        ///  Stop l'enregistrement
        /// </summary>
        /// <returns> l'enregistrement</returns>
        private GestureData StopRecord()
        {
            recordPlaying = false;
            string fileName;
            if(app.view.GetTag() == "")
            {
                fileName = GetUniqueFileName(app.view.GetClassName() + ".txt");
            }
            else
            {
                fileName = GetUniqueFileName(app.view.GetClassName() + "_" + app.view.GetTag() + ".txt");
            }

            Debug.Log(_outData.ToString());
            return new GestureData(null, _outData.ToString(), app.view.GetClassName(), fileName);
        }
        
        
        static public string GetUniqueFileName(string filename)
        {
            string result;
            string pathR = DataManager.GetInstance().GetPathForRecording();
            string fName = Path.GetFileNameWithoutExtension(filename);
            string extension = Path.GetExtension(filename);
            int idx = 0;

            if (!Directory.Exists(pathR))
                Directory.CreateDirectory(pathR);

            do
            {
                result = pathR + Path.DirectorySeparatorChar + fName + "_" + idx + extension;
                idx++;
            }
            while (File.Exists(result));

            return fName + "_" + (idx-1) + extension;
        }
        
        
        /// <summary>
        /// Met à jour le model en fonction de la donnée selectionné
        /// </summary>
        /// <param name="data">la donnée</param>
        private void OnDataSelected(GestureData data)
        {
            app.model.SetSelectedData(data);
        }
        /// <summary>
        /// lance le replay de la data selectionnée
        /// </summary>
        private void OnReplayClicked()
        {
            GestureData data = app.model.GetSelectedData();
            app.view.PlayFile(data);
        }
        /// <summary>
        /// Supprime la donnée selectionné
        /// </summary>
        private void OnDeleteClicked()
        {            
            DataManager.GetInstance().Delete(app.model.GetSelectedData());
            app.model.Pictures[app.model.GetSelectedData().Classe]
                .Remove(app.model.Pictures[app.model.GetSelectedData().Classe]
                    .Find(img => img.Item1 == app.model.GetSelectedData()));
            app.view.DisplayImageOfClass(app.model.GetSelectedData().Classe, true);
        }
        /// <summary>
        /// Met à jour le model en fonction de la classe selectionné
        /// </summary>
        /// <param name="classe">la classe</param>
        private void OnClasseSelected(string classe)
        {
            app.model.SetSelectedClass(classe);
        }

    
    }
}