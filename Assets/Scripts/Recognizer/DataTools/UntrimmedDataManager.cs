using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Recognizer.EvaluationTools;
using UnityEngine;

namespace Recognizer.DataTools
{
    public abstract class UntrimmedDataManager
    {
        protected Dictionary<int, string> _mapIdClass;
        protected abstract string PathRaw();
        

        
        
        /// <summary>
        /// Donne toutes les données enregistrées : + opti
        /// </summary>
        public Dictionary<string, Func<List<GestureData>>> GetDataClassesLAZYAndHeaderDergOnly()
        {
            Dictionary<string, Func<List<GestureData>>> data = new Dictionary<string, Func<List<GestureData>>>();


            List<string> classeName = GetAllDataClasseName();

            foreach (string classe in classeName)
            {
                Func<List<GestureData>> a = () =>GetListOfGestureDataForClasse(classe);
                data.Add(classe,a);
            }

            return data;

        }

        protected abstract List<GestureData> GetListOfGestureDataForClasse(string classe);

        protected void ReadOneInString(StringBuilder stringBuilder,IEnumerable<string> data)
        {
            foreach (string s in data)
            {
                stringBuilder.Append(s).AppendLine();
            }
        }


        /// <summary>
        /// Donne une texture représentant la projection en 2D des trajectoires du geste passé en paramètre
        /// </summary>
        /// <param name="gestureData"></param>
        /// <returns></returns>
        public Texture2D GetImageFromDataGesture(GestureData gestureData)
        {

            if ((!Directory.Exists(Application.dataPath + "/../gestePicture")) || (!File.Exists(Application.dataPath + "/../gestePicture/" + gestureData.DataName + ".png")))
                ImageGenerator.saveImage(gestureData.Path, gestureData.DataName,1000, 1000);
            
            byte[] fileData = File.ReadAllBytes(Application.dataPath + "/../gestePicture/"+gestureData.DataName+".png");
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            return tex;
        }
        
        protected Dictionary<int, string> GetMappingIdClassFromActionFile(string pathMapping)
        {
            if (_mapIdClass != null)
                return _mapIdClass;
            _mapIdClass=new Dictionary<int, string>();
            string pathMappingActions = pathMapping;
            string[] lines = File.ReadAllLines(pathMappingActions);
            lines.Where(l=>l.Split(';').Length>0).ToList().ForEach(l=>
            {
                string[] vals = l.Split(';');
                _mapIdClass.Add(int.Parse(vals[0]),vals[1]);
            });
            return _mapIdClass;
        }

        /// <summary>
        /// Give all avaible classes depending of files
        /// </summary>
        public abstract List<string> GetAllDataClasseName();



    }
}