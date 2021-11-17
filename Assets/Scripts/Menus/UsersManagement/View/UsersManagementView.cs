using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Recognizer;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Menus.UsersManagement.View
{
    public class UsersManagementView : UsersManagementElement
    {

        public Canvas canvas;
        public RectTransform currentUserContent;
        public RectTransform listUsersContent;
        public InputAttribute prefabAttributeInput;
        /*[FormerlySerializedAs("prefabButtonUser")]*/ public UserButton userButton;

        public Button add;
        public Button delete;
        public Button save;

        [FormerlySerializedAs("currentButtonUser")] public UserButton currentUserButton;
        public Dictionary<string,InputAttribute> dicInputAttribute = new Dictionary<string, InputAttribute>();

        public void InitView()
        {
            this.RefreshUserList();
            add.onClick.AddListener(() => UsersManagementNotification.AddUserClicked());
            delete.onClick.AddListener(() => UsersManagementNotification.DeleteUserClicked());
            save.onClick.AddListener(() => this.SaveClicked());
        }

        public void RefreshCurrentUser()
        {
           
            UserData currentUser = app.model.GetCurrentUser();

            foreach (Transform child in currentUserContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            dicInputAttribute.Clear();
            if (currentUser != null)
            {
                foreach (KeyValuePair<string, string> item in currentUser.GetListOfAttributes())
                {
                        if (!item.Key.Equals("Id"))
                        {
                            dicInputAttribute.Add(item.Key,Instantiate(prefabAttributeInput, currentUserContent.transform, false));
                            dicInputAttribute[item.Key].gameObject.name = item.Key;
                            dicInputAttribute[item.Key].title_text.text = item.Key;
                            dicInputAttribute[item.Key].placeholder_text.text = "";
                            dicInputAttribute[item.Key].gameObject.name = item.Key;
                            dicInputAttribute[item.Key].input_text.text = item.Value;
                        }
                } 
            }
            
        }

        public void RefreshUserList()
        {
            foreach (Transform child in listUsersContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            List<UserData> listUsers = app.model.GetUsers();
            foreach (UserData user in listUsers)
            {
                UserButton newButton = Instantiate(userButton, listUsersContent.transform, false);
                newButton.gameObject.name = user.FirstName + " " + user.LastName;
                newButton.text.text = user.FirstName + " " + user.LastName;
                newButton.user = user;
                newButton.button.onClick.AddListener(() => this.SelectedUser(newButton));
            }
        }

        public void SelectedUser(UserButton button)
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
            UsersManagementNotification.OnSelectedUser(button.user);
        }

        public void SaveClicked()
        {
            UserData saveUser = new UserData(app.model.GetCurrentUser().Id,
                dicInputAttribute["FirstName"].input_text.text,
                dicInputAttribute["LastName"].input_text.text,
                dicInputAttribute["Age"].input_text.text,
                dicInputAttribute["Weight"].input_text.text,
                dicInputAttribute["Size"].input_text.text,
                dicInputAttribute["Hand"].input_text.text,
                dicInputAttribute["Beginner"].input_text.text
                );
            UsersManagementNotification.SaveUserClicked(saveUser);
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