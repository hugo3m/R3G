using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Recognizer.DataTools
{
    public class PKUMMDv1DataManager : UntrimmedDataManager
    {

        private string pathPKUMMD = "C:\\workspace2\\Datasets\\PKUMMDv1\\";
        protected override string PathRaw()
        {
            return pathPKUMMD+"Data";
        }
        
        protected string PathLabelsFiles()
        {
            return pathPKUMMD+"Label\\";
        }
        protected string PathRegFiles()
        {
            return pathPKUMMD+"Regress\\";
        }      
        protected string PathClassifFiles()
        {
            return pathPKUMMD+"Classif\\";
        }
        
        protected string PathMappingActionFile()
        {
            return pathPKUMMD+"\\Split\\Actions.csv";
        }
        
        protected string PathVoxelFiles()
        {
            return pathPKUMMD + "Voxelized\\";
        }


        private string PathSplitCuDi()
        {
            return pathPKUMMD + "CuDiSplit\\";
        }
        
        private string PathNormData()
        {
            return pathPKUMMD + "DataNorm\\";
        }
        
        private string PathVideoRGB()
        {
            return null;
            return pathPKUMMD + "VideoRGB\\";
        }
        

        protected override List<GestureData> GetListOfGestureDataForClasse(string classe)
        {
            DirectoryInfo d = new DirectoryInfo(PathRaw());
            FileInfo[] files = d.GetFiles(classe+"-?.txt"); //Getting Text files
            List<GestureData> data = new List<GestureData>();

            foreach (FileInfo file in files)
            {
                IEnumerable<string> dataLines = File.ReadLines(file.FullName);

                string pathLabel = PathLabelsFiles() + file.Name;
                string pathClassif = null;
                string pathRegress = null;
                string pathVoxel = null;
                string pathSplitCuDi = null;
                string pathNormData = null;
                string pathVideoRGB = null;
                
                if (PathClassifFiles() != null)
                {
                    pathClassif = PathClassifFiles() + file.Name;
                }
                if (PathRegFiles() != null)
                {
                    pathRegress = PathRegFiles() + file.Name;
                }
                
                if (PathVoxelFiles() != null)
                {
                    pathVoxel = PathVoxelFiles() + file.Name;
                }
                
                if (PathVoxelFiles() != null)
                {
                    pathSplitCuDi = PathSplitCuDi() + file.Name;
                }
                
                if (PathNormData() != null)
                {
                    pathNormData = PathNormData() + file.Name;
                }
                
                if (PathVideoRGB() != null)
                {
                    pathVideoRGB = PathVideoRGB() + file.Name.Split('_')[0]+"_color.mp4";
                }
                
                Action<StringBuilder> lambda = (StringBuilder strB) => ReadOneInString(strB, dataLines);
                data.Add(new LabeledUntrimedGestureData(file.FullName, lambda, classe, file.Name, pathLabel,
                    GetMappingIdClassFromActionFile(PathMappingActionFile()), pathClassif, pathRegress,pathVoxel,pathSplitCuDi,pathNormData,pathVideoRGB));
                
            }
            return data;
        }

        public override List<string> GetAllDataClasseName()
        {
            DirectoryInfo d = new DirectoryInfo(PathRaw());
            FileInfo[] files = d.GetFiles("*.txt"); //Getting Text files
            List<string> names = new List<string>();
            
            foreach (FileInfo file in files)
            {
                string nameWithoutPosition = file.Name.Substring(0, 4);
                if(!names.Contains(nameWithoutPosition))
                    names.Add(nameWithoutPosition);
            }

            return names;
        }
    }
}