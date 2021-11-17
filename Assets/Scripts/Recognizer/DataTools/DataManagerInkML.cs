using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Recognizer.DataTools
{
    public class DataManagerInkML : DataManager
    {
        private static DataManagerInkML INSTANCE = new DataManagerInkML();

        public new static DataManagerInkML GetInstance(string path)
        {
            _pathDataBase = path;
            return INSTANCE;
        }

        public bool AddDataInkML(GestureDataInkML data)
        {
            string path = System.IO.Path.GetFullPath(_pathDataBase + "/" +
                                                     data.DataName + ".inkml");  
            data.Path = path;
            File.WriteAllText(path, data.ExtractData());
            return true;
        }

        public new string getPathDataBase()
        {
            return _pathDataBase;
        }

        public new void setPathDataBase(string path)
        {
            _pathDataBase = path;
        }

        public void DeleteDataAndInkML(GestureData data)
        {
            string path1 = Path.GetFullPath(_pathDataBase + "/Inkml/" + data.DataName.Replace(".txt",".inkml"));
            string path2 = Path.GetFullPath(_pathDataBase + "/Data/" + data.DataName);
            File.Delete(path1);
            File.Delete(path2);
        }
    }
}