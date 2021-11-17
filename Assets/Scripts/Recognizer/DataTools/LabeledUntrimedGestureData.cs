using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Recognizer.DataTools
{
    public class LabeledUntrimedGestureData : GestureData
    {
        private readonly string _pathLabel;
        public readonly Dictionary<int, string> MappingIdClassName;
        private readonly string _pathClassif;
        private readonly string _pathVoxel;
        public string PathNormalizedData { get; set; }
        private List<List<List<List<float>>>> _voxel;
        private List<Label> labels;
        private List<string> classif;
        private List<int> regress;
        private readonly string _pathRegress;
        private readonly string _pathSplitCuDi;
        public readonly string PathVideoRGB;
        private List<int> _splitCudi;


        public LabeledUntrimedGestureData(string path, string[] data, string classe, string dataName) : base(path, data,
            classe, dataName)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="classe"></param>
        /// <param name="dataName"></param>
        /// <param name="pathLabel"></param>
        /// <param name="mappingIdClass_Name"></param>
        /// <param name="pathClassif">the path where elements are predicted, can be null if no classif</param>
        public LabeledUntrimedGestureData(string path, Action<StringBuilder> data, string classe, string dataName,
            string pathLabel, Dictionary<int, string> mappingIdClass_Name,string pathClassif) : base(path, data, classe, dataName)
        {
            _pathLabel = pathLabel;
            MappingIdClassName = mappingIdClass_Name;
            _pathClassif = pathClassif;
        }
        
        public LabeledUntrimedGestureData(string path, Action<StringBuilder> data, string classe, string dataName,
            string pathLabel, Dictionary<int, string> mappingIdClass_Name,string pathClassif,string pathRegress) : this(path, data, classe, dataName,pathLabel, mappingIdClass_Name, pathClassif)
        {
            _pathRegress = pathRegress;
        }
        
        public LabeledUntrimedGestureData(string path, Action<StringBuilder> data, string classe, string dataName,
            string pathLabel, Dictionary<int, string> mappingIdClass_Name,string pathClassif,string pathRegress,string pathVoxel) : this(path, data, classe, dataName,pathLabel, mappingIdClass_Name, pathClassif,pathRegress)
        {
            _pathVoxel = pathVoxel;
        }

        public LabeledUntrimedGestureData(string path, Action<StringBuilder> data, string classe, string dataName,
            string pathLabel, Dictionary<int, string> mappingIdClass_Name,string pathClassif,string pathRegress,string pathVoxel,string pathSplitCuDi) : this(path, data, classe, dataName,pathLabel, mappingIdClass_Name, pathClassif,pathRegress,pathVoxel)
        {
            _pathSplitCuDi = pathSplitCuDi;
        }
        
        public LabeledUntrimedGestureData(string path, Action<StringBuilder> data, string classe, string dataName,
            string pathLabel, Dictionary<int, string> mappingIdClass_Name,string pathClassif,string pathRegress,
            string pathVoxel,string pathSplitCuDi,string pathNormalizedData) : 
            this(path, data, classe, dataName,pathLabel, mappingIdClass_Name, pathClassif,pathRegress,pathVoxel,pathSplitCuDi)
        {
            if(File.Exists(pathNormalizedData))
                PathNormalizedData = pathNormalizedData;
            else 
                PathNormalizedData = null;
        }
        
        public LabeledUntrimedGestureData(string path, Action<StringBuilder> data, string classe, string dataName,
            string pathLabel, Dictionary<int, string> mappingIdClass_Name,string pathClassif,string pathRegress,
            string pathVoxel,string pathSplitCuDi,string pathNormalizedData,string pathVideoRGB) : 
            this(path, data, classe, dataName,pathLabel, mappingIdClass_Name, pathClassif,pathRegress,pathVoxel,pathSplitCuDi,pathNormalizedData)
        {
            if(File.Exists(pathVideoRGB))
                PathVideoRGB = pathVideoRGB;
            else 
                PathVideoRGB = null;
        }


        
        public LabeledUntrimedGestureData(string path, string data, string classe, string dataName) : base(path, data,
            classe, dataName)
        {
            throw new NotImplementedException();
        }

        public LabeledUntrimedGestureData(string classe, string dataName) : base(classe, dataName)
        {
            throw new NotImplementedException();
        }


        public List<Label> ExtractLabel()
        {
            if (labels != null)
                return labels;

            List<string> lines = File.ReadAllLines(_pathLabel).ToList();

            labels = lines.Where(l => l.Split(',').Length >= 3).Select(l =>
            {
                string[] vals = l.Split(',');
                List<int> valsInt = vals.Select(v => int.Parse(v)).ToList();
                if(valsInt.Count>3)
                    return new Label(valsInt[1], valsInt[2], valsInt[0], MappingIdClassName[valsInt[0]],valsInt[3]);
                else
                    return new Label(valsInt[1], valsInt[2], valsInt[0], MappingIdClassName[valsInt[0]]);

            }).ToList();
            return labels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Null if no path has been furnished</returns>
        public List<string> ExtractClassif()
        {
            if (_pathClassif == null)
                return null;
            if (classif != null)
                return classif;
            try
            {
                classif = File.ReadLines(_pathClassif).ToList()[0].Split(',').Select(s => MappingIdClassName[int.Parse(s)]).ToList();
            }
            catch (Exception e)
            {
                Debug.Log("WARNING:! no classif for this file "+_pathClassif);
            }
            return classif;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Null if no path has been furnished</returns>
        public List<int> ExtractRegress()
        {
            if (_pathRegress == null)
                return null;
            if (regress != null)
                return regress;
            try
            {
                regress = File.ReadLines(_pathRegress).ToList()[0].Split(',').Select(s => int.Parse(s)).ToList();
            }
            catch (Exception e)
            {
                Debug.Log("WARNING:! no classif for this file "+_pathRegress);
            }
            return regress;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Null if no path has been furnished</returns>
        public List<List<List<List<float>>>> ExtractVoxel()
        {
            if (_pathVoxel == null)
                return null;
            if (_voxel != null)
                return _voxel;
            try
            {
                List<int> repeat = ExtractSplitCuDi();
                
                int l = 0;
                List<string> lines = File.ReadLines(_pathVoxel).ToList();
                List<int> dim = lines[0].Split(',').Select(x => int.Parse(x)).ToList();

                List<List<List<List<float>>>> boxes = new List<List<List<List<float>>>>();//size should be one
                for (int x = 1; x < lines.Count; x+=dim[1] *dim[0]) // +32*32
                {
                    List<List<List<float>>> list3D = new List<List<List<float>>>();
                    for (int y = 0; y < dim[1]*dim[0]; y+=dim[0]) // +32
                    {
                        List<List<float>> list2D = new List<List<float>>();
                        for (int z = 0; z < dim[1]; z+=1) // +1
                        {
//                            Debug.Log("x+y+z"+(x+y+z));
                            List<float> zContent = lines[x+y+z].Split(',').Select(v=>float.Parse(v, CultureInfo.InvariantCulture)).ToList();
                            list2D.Add(zContent);
                        }
                        list3D.Add(list2D);
                    }
//                     Debug.Log("x+y+z"+(x+y+z));

                    int repeatNB = 1;
                    if (repeat != null)
                        repeatNB = repeat[l];
                    for (int i = 0; i <repeatNB; i++)
                    {
                        boxes.Add(list3D);
                        // boxes.Add(list3D);//because sample 1/2
                    }
                    
                  
                    
                    l += 1;
                }
                _voxel = boxes;
                Debug.Log("Boxes count");
                Debug.Log(boxes.Count);

            }
            catch (Exception e)
            {
                 Debug.Log("WARNING:! no voxel for this file "+_pathVoxel);
                 Debug.Log(e.Message);
                 return null;
            }
            return _voxel;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Null if no path has been furnished</returns>
        public List<int> ExtractSplitCuDi()
        {
            if (_pathSplitCuDi == null)
                return null;
            if (_splitCudi != null)
                return _splitCudi;
            try
            {
                _splitCudi = File.ReadLines(_pathSplitCuDi).ToList()[0].Split(',').Select(s => int.Parse(s)).ToList();
            }
            catch (Exception e)
            {
                Debug.Log("WARNING:! no CuDi frames for this file "+_pathClassif);
            }
            return _splitCudi;
        }
    }

    public class Label
    {
        public int ActionPoint=-1;
        public int BeginFrame;
        public int EndFrame;
        public string NameLabel;
        public int NumberLabel;

        public Label(int beginFrame, int endFrame, int numberLabel, string nameLabel)
        {
            BeginFrame = beginFrame;
            EndFrame = endFrame;
            NumberLabel = numberLabel;
            NameLabel = nameLabel;
        }
        
        public Label(int beginFrame, int endFrame, int numberLabel, string nameLabel,int actionPoint):this(beginFrame,endFrame,numberLabel,nameLabel)
        {
            ActionPoint = actionPoint;
        }
    }
}