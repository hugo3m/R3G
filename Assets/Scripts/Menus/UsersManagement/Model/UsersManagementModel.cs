using System;
using System.Collections.Generic;
using Recognizer;
using UnityEngine;

namespace Menus.UsersManagement.Model
{
    public class UsersManagementModel
    {
        private List<UserData> _users= new List<UserData>();
        private UserData _currentUser;

        /**
        Set the users with a list of UserData in parameter 
        */
        public void SetUsers(List<UserData> users){
            foreach (UserData user in users)
            {
                this._users.Add(user);
            }
        }

        /**
        Parameter : UserData the new current user
        Goal : Set the new current User of the model
        */
        public void SetCurrentUser(UserData newCurrentUser){
            this._currentUser = newCurrentUser;
        }

        public List<UserData> GetUsers()
        {
            return this._users;
        }

        /**
        return the current user of the model
        */
        public UserData GetCurrentUser(){
            return this._currentUser;
        }

    }
}