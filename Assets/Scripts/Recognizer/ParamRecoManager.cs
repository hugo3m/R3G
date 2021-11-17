using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Recognizer
{
    public class ParamRecoManager
    {
        
        /// <summary>
        /// Donne les classes apprises par le classifieur
        /// </summary>
        /// <returns>la liste des classes</returns>
        /// <exception cref="Exception">lance une exception si la ligne du fichier curDiThreshold décrivant les gestes n'est pas trouvée</exception>
        public List<string> GetAppClassesLearnt()
        {
            try
            {
                string reco;
                if (RecoManager.GetInstance().State == RecoManager.StateCurrentReco.MenuWorking)
                    reco = "MenuRecognizer";
                else
                    reco = "AppRecognizer";
                
                string filePath = System.IO.Path.GetFullPath("./RecognitionServer/"+reco+"/"+RecoManager.GetInstance().DeviceInfo.PathRaw+"/Header.txt");
                string[] lines = System.IO.File.ReadAllLines(filePath);
            
                foreach (var line in lines)
                {
                    if (line.StartsWith("#"))
                    {
                        string[] brutClasses = line.Split('#');
                        IEnumerable<string> enumerable = brutClasses.Where(x => x != "");
                        List<string> classes = enumerable.ToList();
                        return classes;
                    }
                }
                throw new Exception("Classes non trouvées");
            }
            catch (Exception e)
            {
                Debug.LogWarning("Bad configuration"+e);
                return new List<string>();
            }
            
        }

        /// <summary>
        /// Modifie la configuration du moteur pour les classes sur lesquelles il doit apprendre (les fichiers data doivent correspondre)
        /// Un réapprentissage est nécessaire après cette modification
        /// </summary>
        /// <param name="classes"></param>
        public void SetAppClasses(List<string> classes)
        {
            string filePath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/"+RecoManager.GetInstance().DeviceInfo.PathRaw+"/Header.txt");

            string[] lines = System.IO.File.ReadAllLines(filePath);
            int i ;
            for(i =0;i<lines.Length;i++)
                if (lines[i].StartsWith("#"))
                    break;
            string newLine = classes.Select(x => "#" + x).Aggregate("", (acc, v) => acc + v);
            lines[i] = newLine;
            System.IO.File.WriteAllLines(filePath,lines);
        }
        
        
        /// <summary>
        /// Donne les status des jointures de la main (7 par mains)
        /// </summary>
        /// <returns></returns>
        public List<Tuple<LeapMotionJointType,bool>> GetJointsStatus()
        {
            string filePath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/"+RecoManager.GetInstance().DeviceInfo.PathModel+"/curDiThreshold.txt");
            string[] lines = System.IO.File.ReadAllLines(filePath);

            List<Tuple<LeapMotionJointType, bool>> jointsStatus = new List<Tuple<LeapMotionJointType, bool>>();
            string joints = lines[1];
            List<bool> jointsValue = joints.Split(' ').Where(str => str!="").Select(nbStr => nbStr=="1").ToList();
            for (int i = 0; i < jointsValue.Count; i++)
            {
                //on utilise l'ordre de l'énum JointType, correspondant à l'ordre des valeurs dans le fichier
                jointsStatus.Add(new Tuple<LeapMotionJointType, bool>((LeapMotionJointType)i,jointsValue[i]));
            }

            return jointsStatus;
        }

        /// <summary>
        /// Modifie les jointures de la main à prendre en compte pour l'apprentissage
        /// !!! Attention : tous les JointType doivent être fournis !!! (14 en tout)
        /// Un réapprentissage est nécessaire après cette modification.
        /// </summary>
        /// <param name="jointsStatus"></param>
        public void SetJointsStatus(List<Tuple<LeapMotionJointType, bool>> jointsStatus)
        {
            string filePath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/"+RecoManager.GetInstance().DeviceInfo.PathRaw+"/Header.txt");

            string[] lines = System.IO.File.ReadAllLines(filePath);
            int i ;
            for(i =0;i<lines.Length;i++)
                if (lines[i].StartsWith("LEFT_PALM"))
                    break;
            string newLine = jointsStatus.Select(x => x.Item1.ToDescriptionString()+" "+(x.Item2?"1":"0")+" ") //LEFT_PALM 1 LEFT_WRIST 1 ...
                                            .Aggregate("", (acc, v) => acc + v);//reduce, construit une ligne
            lines[i] = newLine;
            System.IO.File.WriteAllLines(filePath,lines);
        }
        
        /// <summary>
        /// Donne la valeur actuelle du noise tolerance (phi)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">lance une exception si la ligne du fichier curDiThreshold décrivant le noise tolerance n'est pas trouvée</exception>
        public string GetNoiseToleranceValue()
        {
            string filePath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/"+RecoManager.GetInstance().DeviceInfo.PathModel+"/curDiThreshold.txt");
            string[] lines = System.IO.File.ReadAllLines(filePath);
            
            foreach (var line in lines)
            {
                if (line.StartsWith("NOISE_TOLERANCE "))
                {
                    string noise = line.Remove(0,"NOISE_TOLERANCE ".Length);
                    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.CurrencyDecimalSeparator = ".";
                    return noise;  
                }
            }
            throw new Exception("Noise Tolerance non trouvé");
        }
        
        
        /// <summary>
        /// Modifie le fichier curDiTreshold et le fichier Header pour modifier le seuil de tolérance
        /// Seul ce seuill peut être modifier dans ce fichier
        /// Pas besoin de réapprentissage
        /// </summary>
        /// <param name="value"></param>
        public void SetNoiseToleranceValue(string value)
        {
            //fichier Curdithreshold
            string filePath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/"+RecoManager.GetInstance().DeviceInfo.PathModel+"/curDiThreshold.txt");
            int i ;
            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(filePath);
                for(i =0;i<lines.Length;i++)
                    if (lines[i].StartsWith("NOISE_TOLERANCE "))
                        break;

                lines[i]="NOISE_TOLERANCE "+value.ToString().Replace(",",".");
                System.IO.File.WriteAllLines(filePath,lines);
            }
            catch (Exception e)
            {
                Debug.LogWarning("No model found");
            } 
         
            
            //fichier Header
            filePath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/"+RecoManager.GetInstance().DeviceInfo.PathRaw+"/Header.txt");
             lines = System.IO.File.ReadAllLines(filePath);
            for(i =0;i<lines.Length;i++)
                if (lines[i].StartsWith("NOISE_TOLERANCE "))
                    break;

            lines[i]="NOISE_TOLERANCE "+value.ToString().Replace(",",".");
            System.IO.File.WriteAllLines(filePath,lines);
        }
        
        /// <summary>
        /// Donne la valeur du pourcentage de la distance curviligne actuelle
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">lance une exception si la ligne du fichier curDiThreshold décrivant la distance curviligne n'est pas trouvée</exception>
        public float GetCurvlinearDistanceValue()
        {
            string filePath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/"+RecoManager.GetInstance().DeviceInfo.PathModel+"/curDiThreshold.txt");
            string[] lines = System.IO.File.ReadAllLines(filePath);
            
            foreach (var line in lines)
            {
                if (line.StartsWith("CURVILINEAR_DISTANCE_PERCENTAGE "))
                {
                    string curvi = line.Remove(0,"CURVILINEAR_DISTANCE_PERCENTAGE ".Length);
                    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.CurrencyDecimalSeparator = ".";
                    return float.Parse(curvi,NumberStyles.Any,ci);
                }
            }
            throw new Exception("Distance curviligne non trouvée");
        }
        
        /// <summary>
        /// Modifie le fichier header pour modifier le pourcentage de distance curviligne utilisé par le moteur
        /// Nécessite un réapprentissage du moteur après cette opération pour être opérationnel
        /// </summary>
        /// <param name="purcentage">le pourcentage de distance curviligne</param>
        public void SetCurvilineDistancePurcentage(float purcentage)
        {
            string filePath = System.IO.Path.GetFullPath("./RecognitionServer/AppRecognizer/"+RecoManager.GetInstance().DeviceInfo.PathRaw+"/Header.txt");
            string[] lines = System.IO.File.ReadAllLines(filePath);
            int i ;
            for(i =0;i<lines.Length;i++)
                if (lines[i].StartsWith("CURVILINEAR_DISTANCE_PERCENTAGE "))
                    break;

            lines[i]="CURVILINEAR_DISTANCE_PERCENTAGE "+purcentage.ToString("0.00").Replace(",",".");
            System.IO.File.WriteAllLines(filePath,lines);
        }
    }
}