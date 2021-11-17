using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Recognizer.EvaluationTools;

namespace Recognizer
{
    public class RecoClassifierManager : IClassifierManager
    {
        public Dictionary<string, Dictionary<string, float>> LearnAndClassify(List<GestureData> datasForLearn, List<GestureData> dataToClassify)
        {
            return LearnAndClassifyStatic(datasForLearn, dataToClassify);
        }
        
        public static Dictionary<string,Dictionary<string,float>> LearnAndClassifyStatic(List<GestureData> datasForLearn, List<GestureData> dataToClassify)
        {
            string pathAppReco = "./RecognitionServer/AppRecognizer/";

            string pathRAWDevice = RecoManager.GetInstance().DeviceInfo.PathRaw;
            
            //Create a test area
            string testArea = pathAppReco;
            string area = "testArea0/";
            int i = 0;
            while (Directory.Exists(testArea+area))
            {
                area = area.Replace(i + "", (i + 1) + "");
                i++;
            }

            testArea = testArea + area;
            Directory.CreateDirectory(testArea);

            //copy RAW given in parameter
            string pathRaw = testArea + pathRAWDevice+"/";
            Directory.CreateDirectory(pathRaw);

            foreach (GestureData data in datasForLearn)
            {
                File.Copy(data.Path,pathRaw+data.DataName);
            }
            
            //copy header file
            File.Copy(pathAppReco+pathRAWDevice+"/Header.txt",pathRaw+"Header.txt");
            
            //copy test data given in parameter
            string pathTest = testArea + "SegmentedTest/";
            Directory.CreateDirectory(pathTest);

            foreach (GestureData data in dataToClassify)
            {
                File.Copy(data.Path,pathTest+data.DataName);
            }
            
            //copy the script batch
            string fileName = "RecognitionForEvaluation.exe";
            string pathBatch = pathAppReco+fileName;
            string pathTestAreaBatch = testArea + fileName;
            File.Copy(pathBatch,pathTestAreaBatch);

            string dllfileName = "Recognizer.dll";
            string pathBatchDll = pathAppReco+dllfileName;
            string pathTestAreaBatchDll = testArea + dllfileName;
            File.Copy(pathBatchDll,pathTestAreaBatchDll);
            
            
            //run the script batch

            System.Diagnostics.Process serverExe = new System.Diagnostics.Process();
            serverExe.StartInfo.WorkingDirectory = testArea;
            serverExe.StartInfo.FileName = fileName;
            serverExe.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            serverExe.StartInfo.Arguments = RecoManager.GetInstance().DeviceInfo.StartArguments+" SegmentedTest SegmentedResult.txt";
            serverExe.Start();
            
            //wait the result
            serverExe.WaitForExit();
            
            //read the file to construct the confusionMatrix
            string pathResult = testArea + "SegmentedResult.txt";
            Dictionary<string, Dictionary<string, float>> confusionMatrix = ReadFileAndExtractMatrixConfusion(pathResult);

            //clear the area
            System.IO.Directory.Delete(testArea,true);
            
            
            return confusionMatrix;
        }

        /// <summary>
        /// Read the result file to construct the confusionMatrix
        /// </summary>
        /// <param name="pathResult">the path of the file containing the result</param>
        /// <returns>the confusion Matrix</returns>
        private static Dictionary<string, Dictionary<string, float>> ReadFileAndExtractMatrixConfusion(string pathResult)
        {
            string[] lines = File.ReadAllLines(pathResult);
            int i = 0;
            while (i < lines.Length && !(lines[i].Contains("Raw results")))
            {
                i++;
            }
            
            
            while (i < lines.Length && !(lines[i].Contains("|")))
            {
                i++;
            }

            //en attendant qu'on connaisse toutes les classes
            List<Tuple<string, List<int>>> values = new List<Tuple<string, List<int>>>();
            while (i < lines.Length && lines[i].Contains("|"))
            {
                List<string> elems = lines[i].Split('|').Where(x => x != "").ToList();
                string classe = elems[elems.Count-1];
                elems.Remove(classe);
                values.Add(new Tuple<string, List<int>>(classe,elems.Select(v=>int.Parse(v)).ToList()));
                i++;
            }
            
                      //ligne           //colonne
            Dictionary<string, Dictionary<string, float>> finalRes = new Dictionary<string, Dictionary<string, float>>();

            List<string> classes = values.Select(t => t.Item1).ToList();
            foreach (Tuple<string,List<int>> value in values)
            {
                finalRes.Add(value.Item1, new Dictionary<string, float>());
                
                for (var index = 0; index < value.Item2.Count; index++)
                {
                    int v = value.Item2[index];
                    finalRes[value.Item1].Add(classes[index],v);
                }
            }

            return finalRes;
        }
    }
}