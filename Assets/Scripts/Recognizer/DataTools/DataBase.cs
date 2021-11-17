using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Recognizer.DataTools
{
    /// <summary>
    /// Object to represent a database with its characteristics
    /// </summary>
    [DataContract]
    public class DataBase
    {
        [DataMember] public string Name { get; set; }
        [DataMember] public string DataBasePath { get; set; }
        [DataMember] public string AIPath { get; set; }
        [DataMember] public string DeviceType { get; private set; }
        
        /// <summary>
        /// Constructor with the IAPath optional
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="deviceType"></param>
        /// <param name="iaPath"></param>
        public DataBase(string name="DefaultDB",string dbPath="./Assets/Scripts/Recognizer/DataTools/DataBases", string deviceType="KinectV1", string iaPath = "./Assets/Scripts/Recognizer/DataTools/Model")
        {
            Regex r = new Regex(@"^\w+");
            Regex regexPath = new Regex(@"^(.+)\/([^\/]+)$");
            this.Name = r.Match(name).ToString().Replace("\u200B","");
            this.DataBasePath =(regexPath.Match(dbPath).ToString()).Replace("\u200B","");;
            this.AIPath = (regexPath.Match(iaPath).ToString()).Replace("\u200B","");
            this.DeviceType = (r.Match(deviceType).ToString()).Replace("\u200B","");
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(this.GetFullPath()))
                {
                    Console.WriteLine("That path exists already.");
                    return;
                }

                // Try to create the directory.
                Directory.CreateDirectory(GetFullPath());
                Directory.CreateDirectory(GetFullPathInkml());
                Directory.CreateDirectory(GetFullPathData());
                Directory.CreateDirectory(GetFullPathDataClasses());
                FileStream fs = new FileStream(GetFullPathActionsCSV(), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Close();
                Directory.CreateDirectory(GetFullPathDemo());
                Directory.CreateDirectory(GetFullPathDemoData());
                Directory.CreateDirectory(GetFullPathDemoInkml());
                Directory.CreateDirectory(GetFullPathDemoDataClasses());
                FileStream fss = new FileStream(GetFullPathDemoActionsCSV(), FileMode.OpenOrCreate,
                    FileAccess.ReadWrite);
                fss.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public void SerializeToJson(string confPath)
        {
            string fullPath = System.IO.Path.GetFullPath(confPath+"/"+this.Name+".json");
            var serializer = new DataContractJsonSerializer(typeof(DataBase));
            FileStream fileStream = new FileStream(fullPath,FileMode.Create);
            serializer.WriteObject(fileStream,this);
            fileStream.Close();
        }
        
        /** 
Gives a list of string elements, each one represents an attribute of the DataBase Class and it's value
*/
        public Dictionary<string,string> GetListOfAttributes(){
            Dictionary<string,string> res = new Dictionary<string,string>();
            PropertyInfo[] propertyInfos = this.GetType().GetProperties();
            foreach (var info in propertyInfos)
            {
                var value = info.GetValue(this,null) ?? "(null)";
                res.Add(info.Name,value.ToString());
            }
            return res;
        }

        public string GetFullPath()
        {
            return Path.GetFullPath(DataBasePath+"/"+Name);
        }

        public string GetFullAIPath()
        {
            Regex r = new Regex(@"^\w+");
            return Path.GetFullPath(GetFullPath() +"/" +r.Match(AIPath).ToString());
        }

        public string GetFullPathInkml()
        {
            return Path.GetFullPath(DataBasePath+"/"+Name+"/Inkml");
        }

        public string GetFullPathData()
        {
            return Path.GetFullPath(DataBasePath+"/"+Name+"/Data");
        }
        public string GetFullPathDataClasses()
        {
            return Path.GetFullPath(DataBasePath+"/"+Name+"/DataClasses"); 
        }
        
        public string GetFullPathActionsCSV()
        {
            return Path.GetFullPath(DataBasePath + "/" + Name + "/Actions.csv");
        }

        public string GetFullPathDemo()
        {
            return Path.GetFullPath(GetFullPath() + "/Demonstration");
        }

        public string GetFullPathDemoData()
        {
            return Path.GetFullPath(GetFullPathDemo() + "/Data");
        }

        public string GetFullPathDemoInkml()
        {
            return Path.GetFullPath(GetFullPathDemo() + "/Inkml");
        }

        public string GetFullPathDemoDataClasses()
        {
            return Path.GetFullPath(GetFullPathDemo() + "/DataClasses");
        }
        public string GetFullPathDemoActionsCSV()
        {
            return Path.GetFullPath(GetFullPathDemoDataClasses() + "/Actions.csv");
        }
        
    }
}
