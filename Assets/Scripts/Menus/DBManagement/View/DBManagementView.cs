using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Menus.UsersManagement.View;
using Recognizer.DataTools;
using SFB;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Menus.DBManagement.View
{
    public class DBManagementView : DBManagementElement
    {
        
        
        public Canvas canvas;
        public RectTransform currentDBContent;
        public RectTransform listDBContent;
        public InputAttribute prefabAttributeInput;
        public DBButton DBButton;

        public Button add;
        public Button delete;
        public Button save;

        public PathButton pathButtonDB;
        public PathButton pathButtonIA;
        [FormerlySerializedAs("currentButtonUser")] public UserButton currentUserButton;
        public Dictionary<string,MonoBehaviour> dicInputAttribute = new Dictionary<string, MonoBehaviour>();

        
        
        
        public void InitView()
        {
            this.RefreshDBList();
            add.onClick.AddListener(() =>
            {
                DBManagementNotification.AddDBClicked();
            });
            delete.onClick.AddListener(() => DBManagementNotification.DeleteDBClicked());
            save.onClick.AddListener(() => this.SaveClicked());
            
        }
        
        public void RefreshCurrentDB()
        {
           
            DataBase currentDB = app.model.GetCurrentDB();

            foreach (Transform child in currentDBContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            dicInputAttribute.Clear();
            if (currentDB != null)
            {
                foreach (KeyValuePair<string, string> item in currentDB.GetListOfAttributes())
                {
                    if (item.Key == "DataBasePath")
                    {
                        pathButtonIA.text.text = "Change DB path";
                        dicInputAttribute.Add(item.Key,
                            Instantiate(pathButtonDB, currentDBContent.transform, false));
                        dicInputAttribute[item.Key].gameObject.name = item.Key;
                        ((PathButton) dicInputAttribute[item.Key]).pathDB =
                            "./Assets/Scripts/Recognizer/DataTools/DataBases";
                            ((PathButton)dicInputAttribute[item.Key]).button.onClick.AddListener(()=> GetPathFromExplorer((PathButton)dicInputAttribute[item.Key]));
                    }
                    else
                    {
                        if (item.Key == "AIPath")
                        {
                            pathButtonIA.text.text = "Change AI path";
                            dicInputAttribute.Add(item.Key,
                                Instantiate(pathButtonIA, currentDBContent.transform, false));
                            dicInputAttribute[item.Key].gameObject.name = item.Key;
                            ((PathButton) dicInputAttribute[item.Key]).pathIA =
                                "./Assets/Scripts/Recognizer/DataTools/Model";
                            ((PathButton)dicInputAttribute[item.Key]).button.onClick.AddListener(()=>GetPathFromExplorer((PathButton)dicInputAttribute[item.Key],true));
                        }
                        else
                        {
                            dicInputAttribute.Add(item.Key,
                                Instantiate(prefabAttributeInput, currentDBContent.transform, false));
                            dicInputAttribute[item.Key].gameObject.name = item.Key;
                            ((InputAttribute) dicInputAttribute[item.Key]).title_text.text = item.Key;
                            ((InputAttribute) dicInputAttribute[item.Key]).placeholder_text.text = "";
                            ((InputAttribute) dicInputAttribute[item.Key]).input_text.text = item.Value;
                        }
                    }
                } 
            }
            
        }
        
        public void RefreshDBList()
        {
            foreach (Transform child in listDBContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            List<DataBase> listDB = app.model.GetDBList();
            foreach (DataBase db in listDB)
            {
                DBButton newButton = Instantiate(DBButton, listDBContent.transform, false);
                newButton.gameObject.name = db.Name;
                newButton.text.text = db.Name;
                newButton.db = db;
                newButton.button.onClick.AddListener(() => this.SelectedDB(newButton));
            }
        }
        
        public void SelectedDB(DBButton button)
        {
            /***
            ColorBlock colors;
            print(button.user.firstName);
            colors = button.button.colors;
            colors.normalColor = new Color32(47, 138, 0, 255);
            colors.highlightedColor = new Color32(41, 217, 0, 255);
            colors.pressedColor = new Color32(20, 97, 0, 255);
            button.button.colors = colors;
            ***/
            DBManagementNotification.OnSelectedDB(button.db);
        }
        public void SaveClicked()
        {
            if (((InputAttribute) dicInputAttribute["Name"]).input_text.text.Replace("\u200B", "") == "")
            {
                DataBase saveDB = new DataBase
                    ("DefaultDB",
                    ((PathButton)dicInputAttribute["DataBasePath"]).pathDB, 
                    ((InputAttribute)dicInputAttribute["DeviceType"]).input_text.text,
                    ((PathButton)dicInputAttribute["AIPath"]).pathIA
                );
                DBManagementNotification.SaveDBClicked(saveDB);
            }
            else
            {
                DataBase saveDB = new DataBase(
                    ((InputAttribute) dicInputAttribute["Name"]).input_text.text,
                    ((PathButton) dicInputAttribute["DataBasePath"]).pathDB,
                    ((InputAttribute) dicInputAttribute["DeviceType"]).input_text.text,
                    ((PathButton) dicInputAttribute["AIPath"]).pathIA
                );
                DBManagementNotification.SaveDBClicked(saveDB);
            }
        }

        public void GetPathFromExplorer(PathButton pathButton,bool ia=false)
        {
            if (ia)
            {
                pathButton.pathIA = StandaloneFileBrowser.OpenFolderPanel("","",true)[0];
                Debug.Log(pathButton.pathIA);
            }
            else
            {
                pathButton.pathDB = StandaloneFileBrowser.OpenFolderPanel("","",true)[0];
                Debug.Log(pathButton.pathDB); 
            }
        }
        public void ChangeToSceneAcquisition()
        {
            switch (InformationContainer.INCOMMING_SCENE)
            {
                case "Acquisition":
                    SceneManager.LoadScene(("Acquisition"));
                    break;
                case "Demonstration":
                    SceneManager.LoadScene(("Demonstration"));
                    break;
                default:
                    break;
            }
        }
        
    }
}