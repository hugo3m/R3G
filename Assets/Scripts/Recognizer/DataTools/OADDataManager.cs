using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Recognizer.DataTools
{
    public class OADDataManager : UntrimmedDataManager
    {
        private string pathOAD = "C:\\workspace2\\Datasets\\OAD\\";

        protected override string PathRaw()
        {
            return pathOAD + "Data";
        }

        protected string PathLabelsFiles()
        {
            return pathOAD + "Label\\";
        }
        
        protected string PathVoxelFiles()
        {
            return pathOAD + "Voxelized\\";
        }

        protected string PathRegFiles()
        {
            return pathOAD + "Regress\\";
        }

        protected string PathClassifFiles()
        {
            return pathOAD + "Classif\\";
        }

        protected string PathMappingActionFile()
        {
            return pathOAD + "\\Split\\Actions.csv";
        }

        private string PathSplitCuDi()
        {
            return pathOAD + "CuDiSplit\\";
        }
        
        private string PathNormData()
        {
            return pathOAD + "DataNorm\\";
        }
        
        private string PathVideoRGB()
        {
            return null;
            return pathOAD + "VideoRGB\\";
        }
        
        /// <summary>
        /// retourne la liste des GestureData pour un groupe de données (défini par la méthode GetAllDataClasseName)
        /// </summary>
        /// <param name="classe"></param>
        /// <returns></returns>
        protected override List<GestureData> GetListOfGestureDataForClasse(string classe)
        {
            DirectoryInfo d = new DirectoryInfo(PathRaw());
            FileInfo[] files = d.GetFiles(classe); //Getting Text files
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




        /// <summary>
        /// Maniere de grouper les fichiers
        /// </summary>
        /// <returns></returns>
        public override List<string> GetAllDataClasseName()
        {
            DirectoryInfo d = new DirectoryInfo(PathRaw());
            FileInfo[] files = d.GetFiles("*.txt"); //Getting Text files
            List<string> names = files.Select(x => x.Name).ToList();
            return names;
        }
    }
}