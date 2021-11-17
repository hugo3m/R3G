using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Menus.CreateGesture.Model;
using Recognizer;
using UnityEngine;
using System.Reflection;

namespace Menus.UsersManagement.Controller
{
    /// <summary>
    /// cf. diagramme de classe "Users"
    /// </summary>
    public class UsersManagementController : MenuController<UsersManagementApplication>
    {

        protected override void Start()
        {
            app.model.SetUsers(UserManager.GetInstance().GetUsers());
            app.view.InitView();
           /* Type userArgs =(typeof(UserData));
            var properties = userArgs.GetProperties(BindingFlags.Public|BindingFlags.Instance);
            foreach (var item in properties)
            {
                print(item);
            }*/
        }

        protected override void OnEnable()
        {
            UsersManagementNotification.AddUserClicked += OnAddUserClicked;
            UsersManagementNotification.OnSelectedUser += OnSelectedUser;
            UsersManagementNotification.DeleteUserClicked += OnDeleteUserClicked;
            UsersManagementNotification.SaveUserClicked += OnSaveUserClicked;
        }

        protected override void OnDisable()
        {

            UsersManagementNotification.AddUserClicked -= OnAddUserClicked;
            UsersManagementNotification.OnSelectedUser -= OnSelectedUser;
            UsersManagementNotification.DeleteUserClicked -= OnDeleteUserClicked;
            UsersManagementNotification.SaveUserClicked -= OnSaveUserClicked;
        }

        protected override void MoveRight()
        {
           
        }

        protected override void MoveLeft()
        {
           
        }
        
        protected override void Enter()
        {
            
        }

        protected override void Update()
        {

        }


        protected void OnAddUserClicked()
        {
            UserManager.GetInstance().AddUser(new UserData("Jean", "Dupont", "30", "183", "72", "right", "true"));
            app.model.GetUsers().Clear();
            app.model.SetUsers(UserManager.GetInstance().GetUsers());
            app.view.RefreshUserList();
        }

        protected void OnDeleteUserClicked()
        {
            if(app.model.GetCurrentUser() != null)
            {
                UserManager.GetInstance().DeleteUser(app.model.GetCurrentUser());
                app.model.SetCurrentUser(null);
                app.model.GetUsers().Clear();
                app.model.SetUsers(UserManager.GetInstance().GetUsers());
                app.view.RefreshUserList();
                app.view.RefreshCurrentUser();
            }
        }

        protected void OnSelectedUser(UserData selectedUser)
        {
            app.model.SetCurrentUser(selectedUser);
            app.view.RefreshCurrentUser();
        }

        protected void OnSaveUserClicked(UserData user)
        {
            UserManager.GetInstance().AddUser(user);
            app.model.GetUsers().Clear();
            app.model.SetUsers(UserManager.GetInstance().GetUsers());
            app.view.RefreshUserList();
            app.model.SetCurrentUser(user);
            app.view.RefreshCurrentUser();
        }

    }
}