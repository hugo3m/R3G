using System;
using System.Collections.Generic;
using Recognizer;
using UnityEngine;

namespace Menus.CreateGesture.Model
{
    public class CreateGestureModel
    {
        public Dictionary<string, List<Tuple<GestureData, Texture2D>>> Pictures = new Dictionary<string, List<Tuple<GestureData, Texture2D>>>();
        
        private string classSelected;
        private GestureData selectedData;
        public Dictionary<string, Func<List<GestureData>>> AllClassesAndDataLazy;
        public Dictionary<string, List<GestureData>> AllClassesAndData = new Dictionary<string, List<GestureData>>();


        public GestureData GetSelectedData()
        {
            return selectedData;
        }
        public void SetSelectedData(GestureData data)
        {
            selectedData = data;
        }
        public void SetSelectedClass(string classe)
        {
            classSelected = classe;
            Debug.Log(classSelected);
        }

        public string GetSelectedClass()
        {
            return classSelected;
        }
        
        public void SetDataClass(Dictionary<string,Func<List<GestureData>>> classes)
        {
            AllClassesAndDataLazy = classes;
        }
        
        public void UpdatePreviousDataOfSelected()
        {
            if (classSelected == null)
            {
                return;
            }
            
            foreach (var gestureData in AllClassesAndData[classSelected])
            {
                if (gestureData == selectedData && AllClassesAndData[classSelected].IndexOf(gestureData) != 0)
                {
                    selectedData = AllClassesAndData[classSelected][AllClassesAndData[classSelected].IndexOf(gestureData) - 1];
                    break;
                } 
            }
        }
        
        public void UpdateNextDataOfSelected()
        {
            if (classSelected == null)
            {
                return;
            }
            
            foreach (var gestureData in AllClassesAndData[classSelected])
            {
                if (gestureData == selectedData && AllClassesAndData[classSelected].IndexOf(gestureData) != AllClassesAndData[classSelected].Count-1)
                {
                    selectedData = AllClassesAndData[classSelected][AllClassesAndData[classSelected].IndexOf(gestureData) + 1];
                    break;
                }      
            }
        }
        
       

      
    }
}