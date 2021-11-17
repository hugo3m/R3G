using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Menus.CreateGesture.Model;
using Recognizer;
using UnityEngine;
using System.Xml;
using System.Runtime.Serialization;
/**
Singleton which manages UserData instances and the location of the data.
*/
public class UserManager 
{
 private static UserManager INSTANCE = new UserManager(); 
 private static string _path; //path of the repertory where user data is located

 public static UserManager GetInstance(){
     _path="./Assets/Scripts/Recognizer/DataTools/UsersData";     //Path : to determine
     return UserManager.INSTANCE;
 }


  /**
  Add a user in the folders of the project
  */
  public bool AddUser(UserData user){
      string fullPath = System.IO.Path.GetFullPath(_path+"/"+user.Id+".xml");
      if (System.IO.File.Exists(fullPath)){
          File.Delete(fullPath);
      }
      user.SerializeToXml(fullPath);
    return true;
  }

  /**
  Get all users which are in the folder located at the path of this class, return a list of UserData
  */
  public List<UserData> GetUsers(){
    List<UserData> usersList = new List<UserData>();
    foreach(string file in Directory.EnumerateFiles(_path,"*.xml")){ 
        FileStream fs = new FileStream(file, FileMode.Open);
        XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
        DataContractSerializer ser = new DataContractSerializer(typeof(UserData));
        UserData user = (UserData) ser.ReadObject(reader,true);
        reader.Close();
        fs.Close();
        usersList.Add(user);
    }
    return usersList;
  }

    /**
      Delete a user in the folders of the project
      */
    public bool DeleteUser(UserData user)
    {
        string fullPath = System.IO.Path.GetFullPath(_path + "/" + user.Id + ".xml");
        if (System.IO.File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return true;
        }
        return false;
    }

}

