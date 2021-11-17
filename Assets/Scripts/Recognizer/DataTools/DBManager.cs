using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Recognizer.DataTools;
using UnityEngine;

/**
Singleton which manages UserData instances and the location of the data.
*/
public class DBManager
{
    private static DBManager INSTANCE = new DBManager();
    public static string CONFIG_PATH = "./Assets/Scripts/Recognizer/DataTools/DataBases/DataBasesConfig";
    private DataBase dataBaseSelected;

    public static DBManager GetInstance()
    {
        return DBManager.INSTANCE;
    }

    /**
  Add a dbConfig with default values in the folders of the project
  */
    public bool AddDB(DataBase db)
    {
        if (Directory.Exists(db.GetFullPath()))
        {
            db.SerializeToJson(CONFIG_PATH);
            return true;
        }

        return false;
    }

    /**
  Get all db which are in the folder located at the path of this class, return a list of DataBase
  */
    public List<DataBase> GetDBs()
    {
        List<DataBase> dbList = new List<DataBase>();
        foreach (string db in Directory.EnumerateFiles(CONFIG_PATH, "*.json"))
        {
            string jsonString = File.ReadAllText(db);
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(DataBase));
                DataBase dbObject = (DataBase) deserializer.ReadObject(ms);
                dbList.Add(dbObject);
            }
        }

        return dbList;
    }

    public void SetCurrentDB(DataBase db)
    {
        dataBaseSelected = db;
    }

    /**
      Delete a db in the folders of the project
      */
    public bool DeleteDB(DataBase db)
    {
        string fullPathConfig = System.IO.Path.GetFullPath(CONFIG_PATH + "/" + db.Name + ".json");
        if (System.IO.File.Exists(fullPathConfig))
        {
            File.Delete(fullPathConfig);
            if (Directory.Exists(db.GetFullPath()))
            {
                Directory.Delete(db.GetFullPath(), true);
                return true;
            }
        }

        return false;
    }
}