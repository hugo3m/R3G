using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
/**
Class to create a User profile with all the data
*/
[DataContract]
public class UserData 
{
    [DataMember] 
   public string Hand{get; set;} //left-handed, right-handed or both
   [DataMember] 
   public string Beginner{get; set;} //tell if the user is a beginner or not
   [DataMember] 
   public string Id{get; set;}
   [DataMember] 
   public string FirstName{get; set;}
   [DataMember] 
   public string LastName{get; set;}
   [DataMember] 
   public string Age{get; set;}
   [DataMember] 
   public string Weight{get; set;}
    [DataMember] 
   public string Size{get; set;}
   
   /**
   Constructor for UserData
   */
   
   public UserData(string firstName, string lastName, string age, string weight, string size, string hand, string beginner)
   {
       Id = Guid.NewGuid().ToString("N");
       this.FirstName=firstName;
       this.LastName=lastName;
       this.Age=age;
       this.Weight=weight;
       this.Size=size;
       this.Hand=hand;
       this.Beginner=beginner;
   }

    public UserData(string id ,string firstName, string lastName, string age, string weight, string size, string hand, string beginner)
    {
        this.Id = id;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Age = age;
        this.Weight = weight;
        this.Size = size;
        this.Hand = hand;
        this.Beginner = beginner;
    }
    /**
Serialize a user into Xml
*/
    public void SerializeToXml(string path){
       var ds = new DataContractSerializer(typeof(UserData));
       var settings = new XmlWriterSettings { Indent = true };
       using (var w = XmlWriter.Create(path, settings))
        ds.WriteObject(w, this);
    }

    /** 
Gives a list of string elements, each one represents an attribute of the UserData Class and it's value
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
}
