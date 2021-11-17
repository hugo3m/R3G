using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Recognizer.EvaluationTools;
using UnityEngine;

namespace Recognizer
{
    public class DataManager
    {
        private static DataManager INSTANCE = new DataManager();
        protected static string _pathDataBase;

        public static DataManager GetInstance(string path)
        {
            _pathDataBase = path;
            return INSTANCE;
        }

        public static DataManager GetInstance()
        {
            _pathDataBase = "./RecognitionServer/AppRecognizer/" + RecoManager.GetInstance().DeviceInfo.PathRaw;
            return INSTANCE;
        }

        /// <summary>
        /// Donne toutes les données enregistrées
        /// </summary>
        public Dictionary<string, List<GestureData>> GetDataClasses()
        {
            Dictionary<string, List<GestureData>> data = new Dictionary<string, List<GestureData>>();

            string path = System.IO.Path.GetFullPath(_pathDataBase);

            DirectoryInfo d = new DirectoryInfo(@path); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            string str = "";
            foreach (FileInfo file in Files)
            {
                string[] dataLines = File.ReadAllLines(file.FullName);
                string first = dataLines.First();
                if (!first.Contains("="))
                    continue;
                string classe = first.Substring(first.IndexOf('=') + 1, first.IndexOf('>') - first.IndexOf('=') - 1);

                if (!data.ContainsKey(classe))
                {
                    data.Add(classe, new List<GestureData>());
                }

                data[classe].Add(new GestureData(file.FullName, dataLines, classe, file.Name));
            }

            return data;
        }

        public List<GestureData> GetDataLAZY()
        {
            List<GestureData> data = new List<GestureData>();

            string path = System.IO.Path.GetFullPath(_pathDataBase);

            DirectoryInfo d = new DirectoryInfo(@path); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            string str = "";
            foreach (FileInfo file in Files)
            {
                string[] dataLines = File.ReadAllLines(file.FullName);
                string first = dataLines.First();
                if (!first.Contains("="))
                    continue;

                string classe = first.Substring(first.IndexOf('=') + 1, first.IndexOf('>') - first.IndexOf('=') - 1);

                data.Add(new GestureData(file.FullName, dataLines, classe, file.Name));
            }

            return data;
        }

        /// <summary>
        /// Donne toutes les données enregistrées : + opti
        /// </summary>
        public Dictionary<string, Func<List<GestureData>>> GetDataClassesLAZYAndHeaderDergOnly()
        {
            Dictionary<string, Func<List<GestureData>>> data = new Dictionary<string, Func<List<GestureData>>>();


            List<string> classeName = GetAllDataClasseName();

            foreach (string classe in classeName)
            {
                Func<List<GestureData>> a = () => GetListOfGestureDataForClasse(classe);
                data.Add(classe, a);
            }

            return data;
        }
        
        /// <summary>
        /// Donne toutes les données enregistrées à partir du fichier Actions.csv: + opti
        /// </summary>
        public Dictionary<string, Func<List<GestureData>>> GetDataClassesLAZYAndCSVOnly()
        {
            Dictionary<string, Func<List<GestureData>>> data = new Dictionary<string, Func<List<GestureData>>>();


            List<string> classeName = GetAllDataClasseNameCSV();

            foreach (string classe in classeName)
            {
                Func<List<GestureData>> a = () => GetListOfGestureDataForClasse(classe);
                data.Add(classe, a);
            }

            return data;
        }

        private List<GestureData> GetListOfGestureDataForClasse(string classe)
        {
            string path = System.IO.Path.GetFullPath(_pathDataBase + "/DataClasses");
            List<GestureData> data = new List<GestureData>();

            DirectoryInfo d = new DirectoryInfo(@path);
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            string str = "";
            foreach (FileInfo file in Files)
            {
                IEnumerable<string> dataLines = File.ReadLines(file.FullName);
                string s = dataLines.First();
                if (!s.Contains("="))
                    continue;

                string classeNameInFile = s.Substring(s.IndexOf('=') + 1, s.IndexOf('>') - s.IndexOf('=') - 1);

                if (classeNameInFile != classe)
                    continue;


                Action<StringBuilder> lambda = (StringBuilder strB) => ReadOneInString(strB, dataLines);
                data.Add(new GestureData(file.FullName, lambda, classe, file.Name));
            }

            return data;
        }

        protected void ReadOneInString(StringBuilder stringBuilder, IEnumerable<string> data)
        {
            foreach (string s in data)
            {
                stringBuilder.Append(s).AppendLine();
            }
        }

        /// <summary>
        /// Donne GestureData (random) d'un geste choisi
        /// </summary>
        /// <param name="classe"> Le nom du geste dont on veut la GestureData </param>
        /// <returns></returns>
        public GestureData GetGestureDataFromClassName(string classe)
        {
            string path = System.IO.Path.GetFullPath(_pathDataBase);

            DirectoryInfo d = new DirectoryInfo(@path);
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files

            //On shuffle les fichiers :
            //-Pas le même geste à chaque fois
            //-Plus rapide en moyenne (pas tous les exemples d'un même geste à la suite)
            List<FileInfo> filesList = Files.Shuffle();

            string str = "";
            foreach (FileInfo file in filesList)
            {
                IEnumerable<string> dataLines = File.ReadLines(file.FullName);
                string first = dataLines.First();
                if (!first.Contains("="))
                    continue;
                string classeName =
                    first.Substring(first.IndexOf('=') + 1, first.IndexOf('>') - first.IndexOf('=') - 1);

                if (classeName == classe)
                {
                    return new GestureData(file.FullName, File.ReadAllLines(file.FullName), classe, file.Name);
                }
            }

            return null;
        }


        /// <summary>
        /// Donne une texture représentant la projection en 2D des trajectoires du geste passé en paramètre
        /// </summary>
        /// <param name="gestureData"></param>
        /// <returns></returns>
        public Texture2D GetImageFromDataGesture(GestureData gestureData)
        {
            if ((!Directory.Exists(Application.dataPath + "/../gestePicture")) ||
                (!File.Exists(Application.dataPath + "/../gestePicture/" + gestureData.DataName + ".png")))
                ImageGenerator.saveImage(gestureData.Path, gestureData.DataName, 1000, 1000);

            byte[] fileData =
                File.ReadAllBytes(Application.dataPath + "/../gestePicture/" + gestureData.DataName + ".png");
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            return tex;
        }

        /// <summary>
        /// remove a gesture data from all data (action irreversible), utilise le path
        /// </summary>
        /// <param name="data"></param>
        public void Delete(GestureData data)
        {
            File.Delete(data.Path);
        }

        /// <summary>
        /// Give all avaible classes depending of files
        /// </summary>
        public List<string> GetAllDataClasseName()
        {
            try
            {
                string filePath =
                    System.IO.Path.GetFullPath(
                        _pathDataBase + "/HeaderDERG.txt");
                string[] lines = System.IO.File.ReadAllLines(filePath);
                int i;
                for (i = 0; i < lines.Length; i++)
                    if (lines[i].StartsWith("#"))
                        break;
                List<string> classes = lines[i].Split('#').Where(x => x.Length != 0).ToList();
                return classes;
            }
            catch (Exception e)
            {
                Exception ex =
                    new Exception(
                        "You need to create a file HeaderDERG.txt into the RAW folder and specify the available gestures separated with '#'");
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// Give all avaible classes depending of CSV files
        /// </summary>
        public List<string> GetAllDataClasseNameCSV()
        {
            List<string> resList = new List<string>();
            try
            {
                string fullPath = Path.GetFullPath(_pathDataBase + "/Actions.csv");
                var dict = File.ReadLines(fullPath).Select(line => line.Split(';'))
                    .ToDictionary(line => int.Parse(line[0]), line => line[1]);
                resList = dict.Values.ToList();
            }
            catch(Exception e)
            {
                Exception ex =
                    new Exception(
                        "You need to create a file Actions.csv into the RAW folder and specify the available gestures separated with ';'");
                Console.WriteLine(ex.Message);
                throw ex;
            }

            return resList;
        }

        protected void AddDataClassNameInHeaderDerg(string classe)
        {
            try
            {
                string filePath =
                    System.IO.Path.GetFullPath(
                        _pathDataBase + "/HeaderDERG.txt");

                string[] lines = System.IO.File.ReadAllLines(filePath);
                int i;
                for (i = 0; i < lines.Length; i++)
                    if (lines[i].StartsWith("#"))
                        break;

                lines[i] = lines[i] + "#" + classe;
                File.WriteAllLines(filePath, lines);
            }
            catch (Exception e)
            {
                Exception ex =
                    new Exception(
                        "You need to create a file HeaderDERG.txt into the RAW folder and specify the available gestures separated with '#'");
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
        
        public void AddDataClassNameInActionsCSV(string className)

        {
            try
            {
                string filePath = System.IO.Path.GetFullPath(_pathDataBase +"/Actions.csv");
                int lineCount = File.ReadLines(filePath).Count();
                if (!this.GetAllDataClasseNameCSV().Contains(className))
                {
                    File.AppendAllText(filePath, lineCount + ";" + className + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                Exception ex =
                    new Exception(
                        "You need to create a file Actions.csv into the RAW folder and specify the available gestures separated with ';'");
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Donne le lieu d'enregistrement des données
        /// </summary>
        /// <returns></returns>
        public string GetPathForRecording()
        {
            return Path.GetFullPath(_pathDataBase + "/");
        }

        /// <summary>
        /// Add a data, utilise l'attribut data (pas le path)
        /// </summary>
        /// <param name="data"></param>
        public bool AddData(GestureData data)
        {
            string path = System.IO.Path.GetFullPath(_pathDataBase + "/" +
                                                     data.DataName + ".txt");
            //if (System.IO.File.Exists(path))
            //   return false;
            data.Path = path;
            File.WriteAllText(path, data.ExtractData());
            return true;
        }

        /*public bool DeleteClass(string className)
        {
            
        }*/

        public string getPathDataBase()
        {
            return _pathDataBase;
        }

        public void setPathDataBase(string pathRaw)
        {
            _pathDataBase = pathRaw;
        }
    }
}